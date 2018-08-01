using FreeDiscDownloader.Extends;
using FreeDiscDownloader.Models;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FreeDiscDownloader.Services
{
    class FreeDiscItemDownloadRepositoryHttp : IFreeDiscItemDownloadRepository
    {
        public bool LoadFromDB(IList<FreeDiscItemDownload> freeDiscDownloader)
        {
            if (freeDiscDownloader == null)
            { freeDiscDownloader = new List<FreeDiscItemDownload>(); }
            else
            { freeDiscDownloader?.Clear();  }

            try
            {
                using (var conn = new SQLite.SQLiteConnection(App.AppSetting.DBDownloadPath))
                {
                    conn.CreateTable<FreeDiscItemDownload>();
                  
                    var list = conn.Table<FreeDiscItemDownload>().OrderByDescending(x => x.DBID);
                    bool rowEven = false;
                    foreach (var item in list)
                    {   // download pending on load from db?
                        if (item.ItemStatus == DownloadStatus.DownloadInProgress)
                        {
                            item.ItemStatus = DownloadStatus.DownloadInterrupted;
                            conn.Update(item);
                        }
                        item.RowEven = !rowEven;
                        rowEven = !rowEven;
                        freeDiscDownloader.Add(item);
                    }
                }
            }
            catch (Exception e) { return false; }
            return true;
        }
        
        public async Task<bool> SaveToDBAsync(FreeDiscItemDownload freeDiscDownloader)
        {
            if (freeDiscDownloader == null) return false;
            #if DEBUG
            Debug.Write("SaveToDB: ID" + freeDiscDownloader.DBID + " Title: " + freeDiscDownloader?.Title+" Status: "+ freeDiscDownloader?.ItemStatus.ToString());
            #endif
            try
            {
                var conn = new SQLite.SQLiteAsyncConnection(App.AppSetting.DBDownloadPath);
                await conn.CreateTableAsync<FreeDiscItemDownload>();
                await conn.InsertAsync(freeDiscDownloader);
            }
            catch(Exception e)
            {
                #if DEBUG
                Debug.Write("SaveToDB: Save error !");
                #endif
                return false;
            }
            #if DEBUG
            Debug.Write("SaveToDB: Result ID" + freeDiscDownloader?.DBID ?? "NULL");
            #endif
            return true;
        }
        
        public async Task<bool> DeleteFromDBAsync(FreeDiscItemDownload freeDiscDownloader)
        {
            #if DEBUG
            Debug.Write("DeleteFromDB: ID" + freeDiscDownloader.DBID + " Title: "+ freeDiscDownloader?.Title + " Status: " + freeDiscDownloader?.ItemStatus.ToString());
            #endif
            if (freeDiscDownloader == null) return false;
            if(freeDiscDownloader.DBID == 0)
            {
                #if DEBUG
                Debug.Write("DeleteFromDB: freeDiscDownloader.DBID == 0 !");
                #endif
                return false;
            }
            try
            {
                var conn = new SQLite.SQLiteAsyncConnection(App.AppSetting.DBDownloadPath);
                await conn.CreateTableAsync<FreeDiscItemDownload>();
                await conn.DeleteAsync(freeDiscDownloader);
            }
            catch (Exception e)
            {
                #if DEBUG
                Debug.Write("DeleteFromDB: Delete error ! : " + e.ToString());
                #endif
                return false;
            }
            return true;
        }
        public async Task<bool> UpdateDBAsync(FreeDiscItemDownload freeDiscDownloader)
        {
            Debug.Write("UpdateDB: ID" + freeDiscDownloader.DBID + " Title: " + freeDiscDownloader?.Title + " Status: " + freeDiscDownloader?.ItemStatus.ToString());
            if (freeDiscDownloader == null) return false;
            if (freeDiscDownloader.DBID == 0)
            {
                #if DEBUG
                Debug.Write("UpdateDBAsync: freeDiscDownloader.DBID == 0 !");
                #endif
                return false;
            }
            try
            {
                var conn = new SQLite.SQLiteAsyncConnection(App.AppSetting.DBDownloadPath);
                await conn.CreateTableAsync<FreeDiscItemDownload>();
                await conn.UpdateAsync(freeDiscDownloader);
            }
            catch (Exception e)
            {
                #if DEBUG
                Debug.Write("UpdateDBAsync: Update error !: " + e.ToString());
                #endif
                return false;
            }
            return true;
        }
        // download sections
        //protected FileDownloadHttp CurrentFileDownload;
        protected FreeDiscItemDownload CurrentfreeDiscDownloader;
        protected CancellationTokenSource CurrentCancellationToken;

        public async Task<bool> DownloadItemAsync(FreeDiscItemDownload freeDiscDownloader)
        {
            CurrentfreeDiscDownloader = freeDiscDownloader;
            // checking permission
            bool havePermisson = false;
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (status != PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                    if (results.ContainsKey(Permission.Storage))
                        status = results[Permission.Storage];
                }
                if (status == PermissionStatus.Granted) havePermisson = true;
            }
            catch (Exception ex)
            {
                #if DEBUG
                Debug.Write("DownloadItemAsync: Exception: " + ex.ToString());
                #endif
            }

            if (! havePermisson)
            {
                await Application.Current.MainPage.DisplayAlert(freeDiscDownloader?.Title, 
                    "Nie można pobrać pliku. Aplikacja nie posiada uprawnień do zapisu plików na urządzeniu", "OK");
                Device.BeginInvokeOnMainThread(() =>
                {
                    freeDiscDownloader.ItemStatus = DownloadStatus.DownloadInterrupted;
                    freeDiscDownloader.DownloadProgres = 0;
                });
                await UpdateDBAsync(freeDiscDownloader);
                return false;
            }

            //checking input data
            if (IsDownloadInProgress())
            {
                #if DEBUG
                Debug.Write("DownloadItemAsync: IsDownloadInProgress()");
                #endif
                return false;
            }

            if(!Uri.IsWellFormedUriString(freeDiscDownloader.Url, UriKind.Absolute))
            {
                #if DEBUG
                Debug.Write("DownloadItemAsync: URI is incorrect");
                #endif
                return false;
            }
            

            if (!ExtensionMethods.IsValidPath(freeDiscDownloader.FilePath, true))
            {
                #if DEBUG
                Debug.Write("DownloadItemAsync: freeDiscDownloader.FilePath is incorrect");
                #endif
                return false;
            }

            if (!Directory.Exists(freeDiscDownloader.FileDirectory)){
                #if DEBUG
                Debug.Write("DownloadItemAsync: !Directory.Exists(freeDiscDownloader.FileDirectory: " + freeDiscDownloader.FileDirectory);
                #endif
                try
                {
                    Directory.CreateDirectory(freeDiscDownloader.FileDirectory);
                }
                catch(Exception e)
                {
                    #if DEBUG
                    Debug.Write("DownloadItemAsync: CreateDirectory error: " + e.ToString());
                    #endif
                    return false;
                }
            }

            if (File.Exists(freeDiscDownloader.FilePath))
            {
                Debug.Write("DownloadItemAsync: File.Exists(freeDiscDownloader.FilePath)");
                try
                {
                    File.Delete(freeDiscDownloader.FilePath);
                }
                catch(Exception e)
                {
                    Debug.Write("DownloadItemAsync: File.Delete error: " + e.ToString());
                    return false;
                }
            }

            // begin download
            #if DEBUG
            Debug.Write("DownloadItemAsync: begin download");
            #endif
            Device.BeginInvokeOnMainThread(() =>
            {
                freeDiscDownloader.DownloadProgres = 0;
                freeDiscDownloader.ItemStatus = DownloadStatus.DownloadInProgress;
            });
            await UpdateDBAsync(freeDiscDownloader);

            CurrentCancellationToken = new CancellationTokenSource();

            bool result = false;
            using (var client = new HttpClientDownloadWithProgress(freeDiscDownloader.Url, freeDiscDownloader.FilePath, CurrentCancellationToken))
            {
                client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) => {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        freeDiscDownloader.DownloadProgres = (double)(progressPercentage / 100);
                    });
                };

                await client.StartDownload();
                result = client.Done;
            }

            if (result)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    freeDiscDownloader.ItemStatus = DownloadStatus.DownloadFinish;
                    freeDiscDownloader.DownloadProgres = 1;
                });
                await UpdateDBAsync(freeDiscDownloader);
                #if DEBUG
                Debug.Write("DownloadItemAsync: DownloadFinish");
                #endif
                return true;
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                freeDiscDownloader.ItemStatus = DownloadStatus.DownloadInterrupted;
                freeDiscDownloader.DownloadProgres = 0;
            });
            await UpdateDBAsync(freeDiscDownloader);
            #if DEBUG
            Debug.Write("DownloadItemAsync:  DownloadInterrupted"); 
            #endif
            return false;
        }

        public void AbortDownloadItem()
        {
            if ((CurrentCancellationToken != null) && (!CurrentCancellationToken.IsCancellationRequested))
                CurrentCancellationToken.Cancel();                
        }
        public bool IsDownloadInProgress()
        {
            if (CurrentfreeDiscDownloader == null) return false;
            if (CurrentfreeDiscDownloader.ItemStatus == DownloadStatus.DownloadInProgress) return true;
            return false;
        }
    }

    public class HttpClientDownloadWithProgress : IDisposable
    {
        public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage);

        public event ProgressChangedHandler ProgressChanged;
        private CancellationTokenSource _cancellationToken = default;

        public long BytesWritten { get; private set; } = 0;
        public long ContentLength { get; private set; } = 0;
        public bool Done => ContentLength == BytesWritten;

        private readonly string _downloadUrl;
        private readonly string _destinationFilePath;

        private HttpClient _httpClient;

        public HttpClientDownloadWithProgress(string downloadUrl, string destinationFilePath, CancellationTokenSource cancellationToken)
        {
            _downloadUrl = downloadUrl;
            _destinationFilePath = destinationFilePath;
            _cancellationToken = cancellationToken;
        }

        public async Task StartDownload()
        {
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:53.0) Gecko/20100101 Firefox/53.0");
            _httpClient.DefaultRequestHeaders.Add("Referer", "http://reseton.pl/static/player/v612/jwplayer.flash.swf");

            try
            {
                using (var response = await _httpClient.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead, _cancellationToken.Token))
                {
                    await DownloadFileFromHttpResponseMessage(response);
                }
            }
            catch(OperationCanceledException)
            {
                ContentLength = -1;
                return;
            }
            catch(Exception e)
            {
                ContentLength = -1;
                #if DEBUG
                Debug.Write("HttpClientDownloadWithProgress: " + e.ToString());
                #endif
                return;
            }     
        }

        private async Task DownloadFileFromHttpResponseMessage(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength;
            ContentLength = totalBytes ?? 0;

            using (var contentStream = await response.Content.ReadAsStreamAsync())
                await ProcessContentStream(totalBytes, contentStream);
        }

        private async Task ProcessContentStream(long? totalDownloadSize, Stream contentStream)
        {
            var totalBytesRead = 0L;
            var readCount = 0L;
            var buffer = new byte[8192];
            var isMoreToRead = true;
            bool[] progressEvent = new bool[101]; 

            using (var fileStream = new FileStream(_destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                do
                {
                    _cancellationToken.Token.ThrowIfCancellationRequested();
                    var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        isMoreToRead = false;
                        TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                        continue;
                    }

                    await fileStream.WriteAsync(buffer, 0, bytesRead);

                    totalBytesRead += bytesRead;
                    BytesWritten = totalBytesRead;
                    readCount += 1;

                    if (readCount % 10 == 0)
                    {   //invoke only 100 progress change event
                        int index = (int) Math.Ceiling((double)(totalBytesRead * 100 / totalDownloadSize.Value));
                        if(index >= 0 && index <= 100 && !progressEvent[index])
                        {
                            #if DEBUG
                            Debug.Write("Progress: " + index.ToString());
                            #endif
                            TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                            progressEvent[index] = true;
                        }
                    }     
                }
                while (isMoreToRead);
            }
        }

        private void TriggerProgressChanged(long? totalDownloadSize, long totalBytesRead)
        {
            if (ProgressChanged == null)
                return;

            double? progressPercentage = null;
            if (totalDownloadSize.HasValue)
                progressPercentage = Math.Round((double)totalBytesRead / totalDownloadSize.Value * 100, 2);

            ProgressChanged(totalDownloadSize, totalBytesRead, progressPercentage);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
