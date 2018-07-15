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
                if(OnChange != null && OnChange.GetInvocationList().Length > 0)
                {
                    OnChange("DownloadFilePath");
                }
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
                if (OnChange != null && OnChange.GetInvocationList().Length > 0)
                {
                    OnChange("LoggedIn");
                }
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
                if (OnChange != null && OnChange.GetInvocationList().Length > 0)
                {
                    OnChange("UserLogin");
                }
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
                if (OnChange != null && OnChange.GetInvocationList().Length > 0)
                {
                    OnChange("UserPassword");
                }
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
                if (OnChange != null && OnChange.GetInvocationList().Length > 0)
                {
                    OnChange("CookieSession");
                }
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
                if (OnChange != null && OnChange.GetInvocationList().Length > 0)
                {
                    OnChange("ListLoadCount");
                }
            }
        }

        public string DBPath { get; private set; }
        private Action<string> OnChange;
    
        public AppSettings(string dbPath, Action<string> onChangeProp)
        {
            this.DBPath = dbPath;
            this.OnChange = onChangeProp;
        }

        public abstract void LoadSetting();
        public abstract void SaveSetting();
    }
}
