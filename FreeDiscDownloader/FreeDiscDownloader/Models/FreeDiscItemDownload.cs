﻿using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FreeDiscDownloader.Models
{
    public class FreeDiscItemDownload : FreeDiscItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; private set; }

        public UInt64 FileSizeBytes { get; set; }
        public bool DownloadComplete { get; set; }
        public string FileName { get; set; }
        public string FileDirectory { get; set; }
        public string FilePath
        {
            get { return Path.Combine(FileDirectory, FileName);  }
        }

    }
}
