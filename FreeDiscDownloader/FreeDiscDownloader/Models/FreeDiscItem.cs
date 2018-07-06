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

    public class SearchItem
    {
        public string Name { get; set; }
        public bool AllItems { get; set; }
        public bool Movie { get; set; }
        public bool Music { get; set; }
        public bool Picture { get; set; }
        public bool Other { get; set; }
    }
}
