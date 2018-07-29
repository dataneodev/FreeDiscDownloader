using FreeDiscDownloader.Models;
using Plugin.DownloadManager;
using Plugin.DownloadManager.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
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
                var rowsCount = conn.Insert(freeDiscDownloader);
                if(rowsCount == 0) { return false; }
            }
            return true;
        }
        /*
        Task<bool> DeleteFromDB(int id);
        Task<bool> UpdateDB(FreeDiscItemDownload freeDiscDownloader);
        */

        // download sections
        protected IDownloadFile CurrentDownloadFile;

        public FreeDiscItemDownloadRepository()
        {
            CrossDownloadManager.Current.CollectionChanged += (sender, e) =>
                System.Diagnostics.Debug.WriteLine(
                    "[DownloadManager] " + e.Action +
                    " -> New items: " + (e.NewItems?.Count ?? 0) +
                    " at " + e.NewStartingIndex +
                    " || Old items: " + (e.OldItems?.Count ?? 0) +
                    " at " + e.OldStartingIndex
                );
        }
        
        public async void DownloadItem(FreeDiscItemDownload freeDiscDownloader)
        {
            if(CurrentDownloadFile != null)
            {
                if (IsDownloadInProgress())
                {
                    Debug.WriteLine("DownloadItem: CurrentDownloadFile is active!");
                    return;
                }
            }

            if(freeDiscDownloader == null)
            {
                Debug.WriteLine("DownloadItem: freeDiscDownloader == null");
                return;
            }

            if(freeDiscDownloader.Url.Length < 6)
            {
                Debug.WriteLine("DownloadItem: freeDiscDownloader.Url.Length < 6");
                return;
            }

            await Task.Yield();
            await Task.Run(async () =>
            {
                Debug.WriteLine("DownloadItem: Downloading file: " + freeDiscDownloader.Url);
                freeDiscDownloader.ItemStatus = DownloadStatus.DownloadInProgress;
                var downloadManager = CrossDownloadManager.Current;
                downloadManager.PathNameForDownloadedFile = new System.Func<IDownloadFile, string>(file => { return freeDiscDownloader.FilePath; });

                CurrentDownloadFile = downloadManager.CreateDownloadFile(freeDiscDownloader.Url);

                CurrentDownloadFile.Headers.Add("Host", @"stream.freedisc.pl");
                CurrentDownloadFile.Headers.Add("Accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                CurrentDownloadFile.Headers.Add("Accept-Language", @"pl,en-US;q=0.7,en;q=0.3");
                CurrentDownloadFile.Headers.Add("Accept-Encoding", @"");
                CurrentDownloadFile.Headers.Add("Connection", @"keep-alive");
                CurrentDownloadFile.Headers.Add("Pragma", @"no-cache");
                CurrentDownloadFile.Headers.Add("DNT", @"1");
                CurrentDownloadFile.Headers.Add("Cache-Control", @"no-cache");
                CurrentDownloadFile.Headers.Add("Referer", @"http://reseton.pl/static/player/v612/jwplayer.flash.swf");
                CurrentDownloadFile.Headers.Add("User-Agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:53.0) Gecko/20100101 Firefox/53.0");

                downloadManager.Start(CurrentDownloadFile, true);
                var isDownloading = true;
                while (isDownloading)
                {
                    isDownloading = IsDownloadInProgress();
                    if(isDownloading)
                    {
                        if(CurrentDownloadFile.TotalBytesExpected > 0)
                            freeDiscDownloader.DownloadProgres = CurrentDownloadFile.TotalBytesWritten / CurrentDownloadFile.TotalBytesExpected;
                    }
                    await Task.Delay(500);
                }
            });

            switch (CurrentDownloadFile.Status)
            {
                case DownloadFileStatus.COMPLETED:
                    freeDiscDownloader.ItemStatus = DownloadStatus.DownloadFinish;
                    break;
                case DownloadFileStatus.CANCELED:
                    freeDiscDownloader.ItemStatus = DownloadStatus.WaitingForDownload;
                    break;
                default:
                    freeDiscDownloader.ItemStatus = DownloadStatus.DownloadInterrupted;
                    break;
            }
        }

        public void AbortDownloadItem()
        {
            CrossDownloadManager.Current.AbortAll();
        }
        public bool IsDownloadInProgress()
        {
            if (CurrentDownloadFile == null) return false;

            switch (CurrentDownloadFile.Status)
            {
                case DownloadFileStatus.INITIALIZED:
                case DownloadFileStatus.PAUSED:
                case DownloadFileStatus.PENDING:
                case DownloadFileStatus.RUNNING:
                    return true;

                case DownloadFileStatus.COMPLETED:
                case DownloadFileStatus.CANCELED:
                case DownloadFileStatus.FAILED:
                    return false;
                default:
                    return false;
            }
        }
    }
}
