using System;
using System.Collections.Generic;
using System.Text;

namespace FreeDiscDownloader.Models
{
    public class FreeDiscItem
    {
        public int IdDb { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string SizeFormat { get; set; }
        public UInt64 SizeFile { get; set; }
        public bool DownloadStarted { get; set; }
        public bool DownloadComplet { get; set; }
        public string FolderDesc { get; set; }
        public ItemType TypeImage { get; set; }
        public string ItemTypRresorce {
            get
            {
                return String.Format("resource://FreeDiscDownloader.Droid.img.{0}.png", TypeImage.ToString());
            }
        }
        public string DateFormat { get; set; }
    }

    public enum ItemType { all, movies, music, photos, other}

    public class SearchItem
    {
        public string SearchPatern { get; set; }
        public ItemType SearchType { get; set; }
    }
}
