using FreeDiscDownloader.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FreeDiscDownloader.Services
{
    public interface IAppSettingRepository
    {
        string DownloadFilePath { get; set; }
        bool LoggedIn { get; set; }
        string UserLogin { get; set; }
        string UserPassword { get; set; }
        string CookieSession { get; set;  }
        byte ListLoadCount { get; set; }
        string DBPath { get; }
    }
}
