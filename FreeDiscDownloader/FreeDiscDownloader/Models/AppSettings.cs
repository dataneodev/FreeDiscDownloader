﻿using FreeDiscDownloader.Services;
using SQLite;
using System;
using System.IO;
using Xamarin.Forms;

namespace FreeDiscDownloader.Models
{
    public class AppSettings
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = 1;

        [Ignore]
        public string AppName { get; } = "FreeDiscDownloader";
        [Ignore]
        public string AppNameVersion { get { return $"{AppName} {GetAppVersion}"; }}
        [Ignore]
        public string PageUrl { get; } = "https://sites.google.com/site/dataneosoftware/polski/freediscdownloader";
        [Ignore]
        public string UpdateServerUrl { get; } = "https://script.google.com/macros/s/AKfycbyNSAsatIhypuk3Y0Y5VYMkTfhu6-TJNzvdE32LKsErd2o059fZ/exec";

        private string downloadFilePath = String.Empty;
        public string DownloadFilePath
        {
            get { return downloadFilePath; }
            set
            {
                downloadFilePath = value;
                SaveSetting();
                OnChange("DownloadFilePath");
            }
        }

        private bool loggedIn = false;
        public bool LoggedIn
        {
            get { return loggedIn; }
            set
            {
                loggedIn = value;
                SaveSetting();
                OnChange("LoggedIn");
            }
        }

        private string userLogin = String.Empty;
        public string UserLogin
        {
            get { return userLogin; }
            set
            {
                userLogin = value;
                SaveSetting();
                OnChange("UserLogin");
            }
        }

        private string userPassword = String.Empty;
        public string UserPassword
        {
            get { return userPassword; }
            set
            {
                userPassword = value;
                SaveSetting();
                OnChange("UserPassword");
            }
        }

        private string cookieSession = String.Empty;
        public string CookieSession
        {
            get { return cookieSession; }
            set
            {
                cookieSession = value;
                SaveSetting();
                OnChange("CookieSession");
            }
        }

        private byte listLoadCount = 20;
        public byte ListLoadCount
        {
            get { return listLoadCount; }
            set
            {
                listLoadCount = value;
                SaveSetting();
                OnChange("ListLoadCount");
            }
        }

        private readonly string DBSettingName = "FDDSetting.sqlite";
        private readonly string DBDownloadsName = "FDDDownloads.sqlite";

        [Ignore]
        private string DBFolderPath { get; set; }
        [Ignore]
        public string DBSettingPath
        {
            get { return Path.Combine(DBFolderPath, DBSettingName); }
        }
        [Ignore]
        public string DBDownloadPath
        {
            get { return Path.Combine(DBFolderPath, DBDownloadsName);  }
        }
        [Ignore]
        public byte MaxDownloadRecInDB { get; } = 20;
        [Ignore]
        public string GetAppVersion { get; } = DependencyService.Get<IAppVersion>().GetVersion();
        [Ignore]
        public int GetAppBuild { get; } = DependencyService.Get<IAppVersion>().GetBuild();

        private Action<string> OnChangeDelegate;

        public AppSettings() { }
        public AppSettings(string dbPath, string defaultStoragePath)
        {
            this.DBFolderPath = dbPath;
            this.downloadFilePath = Path.Combine(defaultStoragePath, App.AppName);
        }

        public void OnPropertyChangeSet(Action<string> onChangeProp)
        {
            this.OnChangeDelegate = onChangeProp;
        }

        private void OnChange(string propName)
        {
            if (OnChangeDelegate != null && OnChangeDelegate.GetInvocationList().Length > 0)
            {
                OnChangeDelegate(propName);
            }
        }

        public virtual void LoadSettingAsync()
        {
            //to ovveride
        }
        public virtual void SaveSetting()
        {
            //to ovveride
        }
    }
}
