using System;
using System.Collections.Generic;
using System.Text;

namespace FreeDiscDownloader
{
    public class SearchItem
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public string Size { get; set; }
        public string Autor { get; set; }
        public string TypeImage { get; set; }
        public string Date { get; set; }
    }

    public class SearchItemType
    {
        public string Name { get; set; }
    }
}
