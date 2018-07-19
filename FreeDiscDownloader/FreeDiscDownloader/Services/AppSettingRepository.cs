using FreeDiscDownloader.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreeDiscDownloader.Services
{
    public class AppSettingRepository : AppSettings, IAppSettingRepository
    {
        public bool AutoSave { get; set; } = false;
        public AppSettingRepository(string dbpath, string defaultStoragePath) : base(dbpath, defaultStoragePath)
        {
           LoadSettingAsync();
        }

        public override void LoadSettingAsync()
        {
            using (var conn = new SQLite.SQLiteConnection(DBSettingPath))
            {
                conn.CreateTable<AppSettings>();
                if(conn.Table<AppSettings>().Count() == 0) { return; }

                var res = conn.Get<AppSettings>(1);

                foreach (PropertyInfo property in typeof(AppSettings).GetProperties())
                { 
                    if (property.CanWrite)
                    {
                        property.SetValue(this, property.GetValue(res, null), null);
                    }
                }  
            }
        }

        public override void SaveSetting()
        {
            if (!AutoSave) return;
            using (var conn = new SQLite.SQLiteConnection(DBSettingPath))
            {
                conn.CreateTable<AppSettings>();
                AppSettings save = new AppSettings();
                foreach (PropertyInfo property in typeof(AppSettings).GetProperties())
                {
                    if (property.CanWrite)
                    {
                        property.SetValue(save, property.GetValue(this, null), null);
                    }
                }

                int count;
                if (conn.Table<AppSettings>().Count() == 0)
                {
                    count = conn.Insert(save);
                }
                else
                {
                    count = conn.Update(save);
                }
                Debug.WriteLine("SaveSetting: " + DBSettingPath);
                Debug.WriteLine("SaveSettingCount : " + count);
            }
        }
    }
}
