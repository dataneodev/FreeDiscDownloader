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
        [PrimaryKey, AutoIncrement, NotNull]
        public int Id { get; set; } // db id

        [NotNull]
        public Int64 FileSizeBytes { get; set; } = 0;
        
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

        [NotNull]
        public string FileName { get; set; } = String.Empty;
        [NotNull]
        public string FileDirectory { get; set; } = String.Empty;
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

            FileDirectory = App.AppSetting.DBDownloadPath;
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
