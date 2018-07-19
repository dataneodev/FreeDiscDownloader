using FreeDiscDownloader.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FreeDiscDownloader.ViewModels
{
    class SettingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public IAppSettingRepository AppSetting;

        public SettingViewModel(IAppSettingRepository _appsetting)
        {
            this.AppSetting = _appsetting;
            AppSetting.OnPropertyChangeSet( OnPropertyChanged );
        }

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
