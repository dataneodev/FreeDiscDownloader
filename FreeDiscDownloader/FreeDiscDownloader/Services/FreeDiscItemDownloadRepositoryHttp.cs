﻿using CommonServiceLocator;
using FreeDiscDownloader.Models;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
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
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("Error", e.Message.ToString(), "Anuluj");
                return false;
            }
            return true;
        }
        
        public async Task<bool> SaveToDBAsync(FreeDiscItemDownload freeDiscDownloader)
        {
            if (freeDiscDownloader == null) return false;
            Debug.Write("SaveToDB: ID" + freeDiscDownloader.DBID + " Title: " + freeDiscDownloader?.Title+" Status: "+ freeDiscDownloader?.ItemStatus.ToString());
            try
            {
                var conn = new SQLite.SQLiteAsyncConnection(App.AppSetting.DBDownloadPath);
                await conn.CreateTableAsync<FreeDiscItemDownload>();
                await conn.InsertAsync(freeDiscDownloader);
            }
            catch(Exception e)
            {
                Debug.Write("SaveToDB: Save error !");
                return false;
            }           

            Debug.Write("SaveToDB: Result ID" + freeDiscDownloader?.DBID ?? "NULL");
            return true;
        }
        
        public async Task<bool> DeleteFromDBAsync(FreeDiscItemDownload freeDiscDownloader)
        {
            Debug.Write("DeleteFromDB: ID" + freeDiscDownloader.DBID + " Title: "+ freeDiscDownloader?.Title + " Status: " + freeDiscDownloader?.ItemStatus.ToString());
            if (freeDiscDownloader == null) return false;
            if(freeDiscDownloader.DBID == 0)
            {
                Debug.Write("DeleteFromDB: freeDiscDownloader.DBID == 0 !");
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
                Debug.Write("DeleteFromDB: Delete error !");
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
                Debug.Write("UpdateDBAsync: freeDiscDownloader.DBID == 0 !");
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
                Debug.Write("UpdateDBAsync: Update error !");
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
                Debug.Write("DownloadItemAsync: Exception: " + ex.ToString());
            }

            if (! havePermisson)
            {
                await Application.Current.MainPage.DisplayAlert(freeDiscDownloader?.Title, 
                    "Nie można pobrać pliku. Aplikacja nie posiada uprawnień do zapisu plików na urządzeniu", "OK");

                freeDiscDownloader.ItemStatus = DownloadStatus.DownloadInterrupted;
                freeDiscDownloader.DownloadProgres = 0;
                await UpdateDBAsync(freeDiscDownloader);
                return false;
            }

            //checking input data
            if (IsDownloadInProgress())
            {
                Debug.Write("DownloadItemAsync: IsDownloadInProgress()");
                return false;
            }

            if(!Uri.IsWellFormedUriString( freeDiscDownloader.Url, UriKind.Absolute))
            {
                Debug.Write("DownloadItemAsync: URI is incorrect");
                return false;
            }
            

            if (freeDiscDownloader.FilePath.Length == 0)
            {
                Debug.Write("DownloadItemAsync: freeDiscDownloader.FilePath.Length == 0");
                return false;
            }

            if (!Directory.Exists(freeDiscDownloader.FileDirectory)){
                Debug.Write("DownloadItemAsync: !Directory.Exists(freeDiscDownloader.FileDirectory: " + freeDiscDownloader.FileDirectory);
                try
                {
                    Directory.CreateDirectory(freeDiscDownloader.FileDirectory);
                }
                catch(Exception e)
                {
                    Debug.Write("DownloadItemAsync: CreateDirectory error: " + e.ToString());
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
            Debug.Write("DownloadItemAsync: begin download");
            freeDiscDownloader.DownloadProgres = 0;
            freeDiscDownloader.ItemStatus = DownloadStatus.DownloadInProgress;
            await UpdateDBAsync(freeDiscDownloader);

            CurrentCancellationToken = new CancellationTokenSource();

            bool result = false;
            using (var client = new HttpClientDownloadWithProgress(freeDiscDownloader.Url, freeDiscDownloader.FilePath, CurrentCancellationToken))
            {
                client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) => {
                    freeDiscDownloader.DownloadProgres = (double)(progressPercentage/100) ;
                };

                await client.StartDownload();
                result = client.Done;
            }

            if (result)
            {
                freeDiscDownloader.ItemStatus = DownloadStatus.DownloadFinish;
                freeDiscDownloader.DownloadProgres = 1;
                await UpdateDBAsync(freeDiscDownloader);
                Debug.Write("DownloadItemAsync: DownloadFinish");
                return true;
            }

            freeDiscDownloader.ItemStatus = DownloadStatus.DownloadInterrupted;
            freeDiscDownloader.DownloadProgres = 0;
            await UpdateDBAsync(freeDiscDownloader);
            Debug.Write("DownloadItemAsync:  DownloadInterrupted");
            return false;
        }

        public void AbortDownloadItem()
        {
            if ((CurrentCancellationToken != null) && (!CurrentCancellationToken.IsCancellationRequested))
            {
                CurrentCancellationToken.Cancel();
            }
                
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
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
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
                Debug.Write("HttpClientDownloadWithProgress: " + e.ToString());
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
                        TriggerProgressChanged(totalDownloadSize, totalBytesRead);
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
