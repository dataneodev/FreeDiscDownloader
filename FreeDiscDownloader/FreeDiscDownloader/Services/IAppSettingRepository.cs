using System;

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
        string DBSettingPath { get; }
        void OnPropertyChangeSet(Action<string> onChangeProp);
        bool AutoSave { get; set; }
    }
}
