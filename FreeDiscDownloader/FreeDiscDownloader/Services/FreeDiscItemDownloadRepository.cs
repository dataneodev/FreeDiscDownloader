using FreeDiscDownloader.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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

            using (var conn = new SQLite.SQLiteConnection(App.AppSetting.DBDownloadPath))
            {
                conn.CreateTable<FreeDiscItemDownload>();
                var list = conn.Table<FreeDiscItemDownload>().Take(App.AppSetting.MaxDownloadRecInDB);
                foreach (var item in list)
                {
                    freeDiscDownloader.Add(item);
                }
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
                if(rowsCount == 0)
                {

                }
            }
            return true;
        }
        /*
        Task<bool> DeleteFromDB(int id);
        Task<bool> UpdateDB(FreeDiscItemDownload freeDiscDownloader);
        */
    }
}
