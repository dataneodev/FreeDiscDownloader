
using SQLite;

namespace FreeDiscDownloader.Models
{
    public class FreeDiscItem
    {
        [Unique]
        public int IdFreedisc { get; set; } = 0;
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string SizeFormat { get; set; } = string.Empty;
        public string FolderDesc { get; set; } = string.Empty;
        public ItemType TypeImage { get; set; } = ItemType.all;
        public string DateFormat { get; set; } = string.Empty;
        public bool RowEven { get; set; } = false;
    }

    public enum ItemType { all, movies, music, photos, other}
}
