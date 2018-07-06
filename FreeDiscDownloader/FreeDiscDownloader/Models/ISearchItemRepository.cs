using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FreeDiscDownloader.Models
{
    public interface IFreeDiscItemRepository
    {
        Task<IEnumerable<FreeDiscItem>> SearchItemWebAsync(SearchItem searchItem);
        Task<FreeDiscItem> GetItemFormDbByIdAsync(int id);
        Task<bool> AddItemToDbAsync(FreeDiscItem freeDiscItem);
        Task<bool> UpdateItemDbAsync(FreeDiscItem freeDiscItem);
        Task<bool> DeleteItemDbAsync(int id);
     }
}
