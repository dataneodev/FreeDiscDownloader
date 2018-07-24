using FreeDiscDownloader.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FreeDiscDownloader.Services
{
    interface IFreeDiscItemDownloadRepository
    {
        bool LoadFromDB(IList<FreeDiscItemDownload> freeDiscDownloader);
        /*
        Task<bool> SaveToDB(FreeDiscItemDownload freeDiscDownloader);
        Task<bool> DeleteFromDB(int id);
        Task<bool> UpdateDB(FreeDiscItemDownload freeDiscDownloader);
        */
    }
}
