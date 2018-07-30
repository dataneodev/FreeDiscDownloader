﻿using FreeDiscDownloader.Models;
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
        bool DeleteFromDB(FreeDiscItemDownload freeDiscDownloader);
        bool UpdateDB(FreeDiscItemDownload freeDiscDownloader);
       
        // dwonload
        Task<bool> DownloadItemAsync(FreeDiscItemDownload freeDiscDownloader);
        void AbortDownloadItem();
        bool IsDownloadInProgress();
       
    }
}