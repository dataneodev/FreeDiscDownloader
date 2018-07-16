using System;
using System.Collections.Generic;
using System.Text;

namespace FreeDiscDownloader.Models
{
    public abstract class AppSettings
    {
        private string downloadFilePath;
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

        private bool loggedIn;
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

        private string userLogin;
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

        private string userPassword;
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

        private string cookieSession;
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

        private byte listLoadCount;
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

        public string DBPath { get; private set; }
        private Action<string> OnChangeDelegate;
    
        public AppSettings(string dbPath)
        {
            this.DBPath = dbPath;
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

        public abstract void LoadSetting();
        public abstract void SaveSetting();
    }
}
