using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FreeDiscDownloader.Models
{
    public class FreeDiscItem
    {
        public int IdFreedisc { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string SizeFormat { get; set; }
        public string FolderDesc { get; set; }
        public ItemType TypeImage { get; set; }
        public string DateFormat { get; set; }
        public bool RowEven { get; set; }
    }

    public enum ItemType { all, movies, music, photos, other}
}
