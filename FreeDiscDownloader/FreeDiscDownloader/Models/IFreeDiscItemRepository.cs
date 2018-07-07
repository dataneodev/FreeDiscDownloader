using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FreeDiscDownloader.Models
{
    public interface IFreeDiscItemRepository
    {
        Task<bool> SearchItemWebAsync(SearchItem searchItem, ICollection<FreeDiscItem> OutCollection, Action<string> statusLog);
        /*Task<FreeDiscItem> GetItemFormDbByIdAsync(int id, Action<string> statusLog);
        Task<bool> AddItemToDbAsync(FreeDiscItem freeDiscItem, Action<string> statusLog);
        Task<bool> UpdateItemDbAsync(FreeDiscItem freeDiscItem, Action<string> statusLog);
        Task<bool> DeleteItemDbAsync(int id, Action<string> statusLog);*/
     }
}
