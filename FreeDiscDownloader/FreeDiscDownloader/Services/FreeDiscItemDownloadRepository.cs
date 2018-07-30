using FreeDiscDownloader.Models;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace FreeDiscDownloader.Services
{
    class FreeDiscItemDownloadRepository : IFreeDiscItemDownloadRepository
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
                    var list = conn.Table<FreeDiscItemDownload>().OrderByDescending(x => x.Id);
                    foreach (var item in list)
                    {
                        freeDiscDownloader.Add(item);
                    }
                }
            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("Error", e.Message.ToString(), "Anuluj");
            }
            return true;
        }
        
        public bool SaveToDB(FreeDiscItemDownload freeDiscDownloader)
        {
            if (freeDiscDownloader == null) return false;
            using (var conn = new SQLite.SQLiteConnection(App.AppSetting.DBDownloadPath))
            {
                conn.CreateTable<FreeDiscItemDownload>();
                freeDiscDownloader.Id = conn.Insert(freeDiscDownloader);
            }
            return true;
        }
        
        public bool DeleteFromDB(FreeDiscItemDownload freeDiscDownloader)
        {
            if (freeDiscDownloader == null) return false;
            using (var conn = new SQLite.SQLiteConnection(App.AppSetting.DBDownloadPath))
            {
                conn.CreateTable<FreeDiscItemDownload>();
                int rowsCount = conn.Delete(freeDiscDownloader);
                if (rowsCount == 0) { return false; }
            }
            return true;
        }
        public bool UpdateDB(FreeDiscItemDownload freeDiscDownloader)
        {
            if (freeDiscDownloader == null) return false;
            if (freeDiscDownloader.Id == 0) return false;
            using (var conn = new SQLite.SQLiteConnection(App.AppSetting.DBDownloadPath))
            {
                conn.CreateTable<FreeDiscItemDownload>();
                conn.Update(freeDiscDownloader);
            }
            return true;
        }
        // download sections
        public FreeDiscItemDownloadRepository()  {  }

        protected FileDownload CurrentFileDownload;
        protected FreeDiscItemDownload CurrentfreeDiscDownloader;

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
                UpdateDB(freeDiscDownloader);
                return false;
            }

            //checking input data
            if (IsDownloadInProgress())
            {
                Debug.Write("DownloadItemAsync: IsDownloadInProgress()");
                return false;
            }

            if(freeDiscDownloader.Url.Length < 5)
            {
                Debug.Write("DownloadItemAsync: freeDiscDownloader.Url.Length < 5");
                return false;
            }

            if(freeDiscDownloader.FilePath.Length == 0)
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
            freeDiscDownloader.ItemStatus = DownloadStatus.DownloadInProgress;
            UpdateDB(freeDiscDownloader);

            CurrentFileDownload = new FileDownload(freeDiscDownloader.Url, freeDiscDownloader.FilePath, 64*1024);

            CurrentFileDownload.DownloadProgressChanged += (e, a) =>
            {
                var inst = e as FileDownload;
                    if (inst.ContentLength > 0)
                        freeDiscDownloader.DownloadProgres = (double)inst.BytesWritten / inst.ContentLength;
            };

            CurrentFileDownload.DownloadCompleted += (e, a) =>
            {
                freeDiscDownloader.DownloadProgres = 1;
            };

            await CurrentFileDownload.Start();
            Debug.Write("DownloadItemAsync: ContentLength:" + CurrentFileDownload.ContentLength);
            if (CurrentFileDownload.Done)
            {
                freeDiscDownloader.ItemStatus = DownloadStatus.DownloadFinish;
               // freeDiscDownloader.FileSizeBytes = CurrentFileDownload.ContentLength;
                UpdateDB(freeDiscDownloader);
                Debug.Write("DownloadItemAsync: CurrentFileDownload.Done");
                return true;
            }

            freeDiscDownloader.ItemStatus = DownloadStatus.DownloadInterrupted;
            freeDiscDownloader.DownloadProgres = 0;
            UpdateDB(freeDiscDownloader);
            Debug.Write("DownloadItemAsync:  DownloadStatus.DownloadInterrupted;");
            return false;
        }

        public void AbortDownloadItem()
        {
            if (CurrentFileDownload != null)
                CurrentFileDownload.Abort();
        }
        public bool IsDownloadInProgress()
        {
            if (CurrentfreeDiscDownloader == null) return false;
            if (CurrentfreeDiscDownloader.ItemStatus == DownloadStatus.DownloadInProgress) return true;
            return false;
        }
    }

    public class FileDownload
    {
        private volatile bool _allowedToRun;
        public volatile bool _downloadPending;

        private readonly string _source;
        private readonly string _destination;
        private readonly int _chunkSize;

        public long BytesWritten { get; private set; }
        public long ContentLength { get; private set; }

        public bool Done => ContentLength == BytesWritten;

        public event EventHandler DownloadProgressChanged;
        public event EventHandler DownloadCompleted;

        private WebResponse CurrentResponse;

        public FileDownload(string source, string destination, int chunkSize)
        {
            _allowedToRun = true;
            _source = source;
            _destination = destination;
            _chunkSize = chunkSize;

            ContentLength = 0;
            BytesWritten = 0;
        }

        private async Task Start(long range)
        {
            if (!_allowedToRun)
                throw new InvalidOperationException();

            var request = (HttpWebRequest)WebRequest.Create(_source);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:53.0) Gecko/20100101 Firefox/53.0";
            request.Referer = @"http://reseton.pl/static/player/v612/jwplayer.flash.swf";
            request.AddRange(range);

            try
            {
                using (var response = await request.GetResponseAsync())
                {
                    CurrentResponse = response;
                    ContentLength = response.ContentLength;

                    using (var responseStream = response.GetResponseStream())
                    {
                        bool isDone = false;
                        using (var fs = new FileStream(_destination, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                        {
                            while (_allowedToRun)
                            {
                                var buffer = new byte[_chunkSize];
                                var bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                                if (bytesRead == 0)
                                {
                                    isDone = true;
                                    break;
                                }
                                await fs.WriteAsync(buffer, 0, bytesRead);
                                BytesWritten += bytesRead;

                                DownloadProgressChanged?.Invoke(this, null);
                            }
                            await fs.FlushAsync();
                        }
                        if (isDone)
                            DownloadCompleted?.Invoke(this, null);
                    }
                }
            }
            catch(Exception e)
            {
                Debug.Write("Download file exceptions: " + e.ToString());
                ContentLength = -1;
            }
        }

        public Task Start()
        {
            _allowedToRun = true;
            return Start(BytesWritten);
        }

        public void Pause()
        {
            _allowedToRun = false;
        }

        public void Abort()
        {
            if(CurrentResponse != null)
            {
               // CurrentResponse.Close();
                _allowedToRun = false;
            }
        }
    }
}
