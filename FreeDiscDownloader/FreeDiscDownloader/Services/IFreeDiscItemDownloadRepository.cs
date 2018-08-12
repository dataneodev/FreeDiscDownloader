using FreeDiscDownloader.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FreeDiscDownloader.Services
{
    public interface IFreeDiscItemDownloadRepository
    {
        // sql
        bool LoadFromDB(IList<FreeDiscItemDownload> freeDiscDownloader);
        Task<bool> SaveToDBAsync(FreeDiscItemDownload freeDiscDownloader);
        Task<bool> DeleteFromDBAsync(FreeDiscItemDownload freeDiscDownloader);
        Task<bool> UpdateDBAsync(FreeDiscItemDownload freeDiscDownloader);
       
        // dwonload
        Task<bool> DownloadItemAsync(FreeDiscItemDownload freeDiscDownloader);
        void AbortDownloadItem();
        bool IsDownloadInProgress();
       
    }
}
