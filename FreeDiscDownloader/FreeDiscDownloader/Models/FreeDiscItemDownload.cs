using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace FreeDiscDownloader.Models
{
    public class FreeDiscItemDownload : FreeDiscItem, INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // db id
      
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

        [Ignore]
        public Int64 FileSizeBytes { get; set; } = 0;

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

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
