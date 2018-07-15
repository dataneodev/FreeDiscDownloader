using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FreeDiscDownloader.Models;

namespace FreeDiscDownloader.Services
{
    public interface IFreeDiscItemRepository
    {
        Task<SearchItemWebResult> SearchItemWebAsync(SearchItemWebRequest searchItem, IList<FreeDiscItem> OutCollection, Action<string> statusLog);
        /*Task<FreeDiscItem> GetItemFormDbByIdAsync(int id, Action<string> statusLog);
        Task<bool> AddItemToDbAsync(FreeDiscItem freeDiscItem, Action<string> statusLog);
        Task<bool> UpdateItemDbAsync(FreeDiscItem freeDiscItem, Action<string> statusLog);
        Task<bool> DeleteItemDbAsync(int id, Action<string> statusLog);*/
     }
}
