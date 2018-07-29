using FreeDiscDownloader.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FreeDiscDownloader.Services
{
    interface IFreeDiscItemDownloadRepository
    {
        // sql
        bool LoadFromDB(IList<FreeDiscItemDownload> freeDiscDownloader);
        bool SaveToDB(FreeDiscItemDownload freeDiscDownloader);

        /*
        Task<bool> DeleteFromDB(int id);
        Task<bool> UpdateDB(FreeDiscItemDownload freeDiscDownloader);
        */
        // dwonload
        void DownloadItem(FreeDiscItemDownload freeDiscDownloader);
        void AbortDownloadItem();
        bool IsDownloadInProgress();
       
    }
}
