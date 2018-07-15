using FreeDiscDownloader.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FreeDiscDownloader.Services
{
    public class AppSettingRepository : AppSettings
    {
        public AppSettingRepository(string dbpath) : base(dbpath)
        {
            
        }
        public override void LoadSetting()
        {

        }
        public override void SaveSetting()
        {

        }
    }
}
