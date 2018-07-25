using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace FreeDiscDownloader.Models
{
    public class FreeDiscItemDownload : FreeDiscItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; private set; } // db id

        public Int64 FileSizeBytes { get; set; } = 0;
        public DownloadStatus ItemStatus { get; set; } = DownloadStatus.WaitingForDownload;
        [Unique]
        public string FileName { get; set; } = String.Empty;
        public string FileDirectory { get; set; } = String.Empty;
        [Ignore]
        public string FilePath
        {
            get { return Path.Combine(FileDirectory, FileName);  }
        }

        public FreeDiscItemDownload()
        {

        }

        public FreeDiscItemDownload(FreeDiscItem freeDiscItem)
        {
            foreach (PropertyInfo property in typeof(FreeDiscItem).GetProperties())
            {
                if (property.CanWrite)
                {
                    property.SetValue(this, property.GetValue(freeDiscItem, null), null);
                }
            }
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
