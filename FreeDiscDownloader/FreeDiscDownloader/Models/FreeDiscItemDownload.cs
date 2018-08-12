using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace FreeDiscDownloader.Models
{
    public class FreeDiscItemDownload : FreeDiscItem, INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int DBID { get; set; } // db id
      
        private DownloadStatus itemStatus = DownloadStatus.WaitingForDownload;
        [NotNull]
        public DownloadStatus ItemStatus
        {
            get { return itemStatus; }
            set
            {
                itemStatus = value;
                OnPropertyChanged();
            }
        }

        private double downloadProgres = 0;
        [Ignore]
        public double DownloadProgres
        {
            get { return downloadProgres; }
            set
            {
                downloadProgres = value;
                OnPropertyChanged();
            }
        }

        public string FileName { get; private set; } = String.Empty;
        public string FileDirectory { get; private set; } = String.Empty;
        [Ignore]
        public string FilePath
        {
            get { return Path.Combine(FileDirectory, FileName);  }
        }

        private bool rowEven = false;
        [Ignore]
        public new bool RowEven
        {
            get { return rowEven; }
            set
            {
                rowEven = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                });
            }
        }

        public FreeDiscItemDownload(){ }

        public FreeDiscItemDownload(FreeDiscItem freeDiscItem)
        {
            if (freeDiscItem == null) return;
            foreach (PropertyInfo property in typeof(FreeDiscItem).GetProperties())
            {
                if (property.CanWrite)
                {
                    property.SetValue(this, property.GetValue(freeDiscItem, null), null);
                }
            }

            FileDirectory = App.AppSetting.DownloadFilePath;
            FileName = freeDiscItem.Title;
        }
    }

    public enum DownloadStatus
    {
        DownloadFinish, 
        WaitingForDownload,
        DownloadInProgress,
        DownloadInterrupted
    }
}
