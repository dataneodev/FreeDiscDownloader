using System;
using System.Collections.Generic;
using System.Text;

namespace FreeDiscDownloader.Models
{
    public class FreeDiscItem
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Size { get; set; }
        public UInt64 SizeFile { get; set; }
        public bool DownloadStarted { get; set; }
        public bool DownloadComplet { get; set; }
        public string Autor { get; set; }
        public string TypeImage { get; set; }
        public string Date { get; set; }
    }

    public enum ItemType { all, movies, music, photos, other}

    public class SearchItem
    {
        public string SearchPatern { get; set; }
        public ItemType SearchType { get; set; }
    }
}
