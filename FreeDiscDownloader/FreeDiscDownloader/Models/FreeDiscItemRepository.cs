using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FreeDiscDownloader.Models
{
    class FreeDiscItemRepository : IFreeDiscItemRepository
    {
        protected struct SearchObj
        {
            public string search_phrase;
            public string search_type;
            public int search_saved;
            public int pages;
            public int limit;
        }

        public async Task<bool> SearchItemWebAsync(SearchItem searchItem, ICollection<FreeDiscItem> OutCollection, Action<string> statusLog)
        {
            const string freeDiscSearchUrl = "https://freedisc.pl/search/get";
            if(searchItem == null)
            {
                Debug.WriteLine("SearchItemWebAsync: searchItem == null");
                return false;
            }
            if(OutCollection == null) {
                OutCollection = new List<FreeDiscItem>();
            } else {
                OutCollection.Clear();
            }

            SearchObj searchObj = new SearchObj
            {
                search_phrase = searchItem.SearchPatern,
                search_type = searchItem.SearchType.ToString(),
                search_saved = 0,
                pages = 0,
                limit = 25,
            };

            var postData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(searchObj) );
            HttpWebRequest webRequest = null;
            try
            {
                webRequest = (HttpWebRequest) WebRequest.Create(freeDiscSearchUrl);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:53.0) Gecko/20100101 Firefox/53.0";
                webRequest.Accept = "application/json, text/javascript, */*; q=0.01";
                webRequest.Referer = $@"https://freedisc.pl/search/{searchObj.search_type}/{WebUtility.UrlEncode(searchItem.SearchPatern)}/";
                webRequest.ContentLength = postData.Length;
                webRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
                webRequest.Headers.Add("Accept-Language", "pl,en-US;q=0.7,en;q=0.3");
                webRequest.Headers.Add("Accept-Encoding", "");
                using (var stream = webRequest.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }
            }
            catch (System.Exception e)
            {
                Debug.WriteLine("SearchItemWebAsync: WebRequest Exception: " + e.Message.ToString());
                return false;
            }

            WebResponse webResponse = null;
            String responseString = String.Empty;
            try
            {
                Task<WebResponse> webResponseTask = webRequest.GetResponseAsync();
                await Task.WhenAll(webResponseTask);

                if (webResponseTask.IsFaulted)
                {
                    Debug.WriteLine("SearchItemWebAsync: webResponseTask.IsFaulted");
                    return false;
                }

                webResponse = webResponseTask.Result;
                
                using (var sr = new System.IO.StreamReader(webResponse.GetResponseStream()))
                {
                    responseString = sr.ReadToEnd();
                }
            }
            catch (System.Exception e)
            {
                Debug.WriteLine("SearchItemWebAsync: WebResponse Exception: " + e.Message.ToString());
                return false;
            }

            if (responseString.Length == 0)
            {
                Debug.WriteLine("SearchItemWebAsync: data.Length == 0 ");
                return false;
            }

            FreeDiscWebSearchResponseModel responseModel;
            try
            {
                responseModel = JsonConvert.DeserializeObject<FreeDiscWebSearchResponseModel>(responseString);
            }
            catch(System.Exception e)
            {
                Debug.WriteLine("SearchItemWebAsync: DeserializeObject<FreeDiscWebSearchResponseModel> problem: " +  e.Message.ToString());
                return false;
            }
            Debug.WriteLine(responseString);
            if (!responseModel.success)
            {
                Debug.WriteLine("SearchItemWebAsync: !responseModel.success");
                return false;
            }

            if(responseModel.response.data_files.hits == 0)
            {
                return true; // noting found
            }

            Func<string, ItemType> getImageType = icon =>
            {
                switch (icon)
                {
                    case "icon-headphones":
                        return ItemType.music;
                    case "icon-film":
                        return ItemType.movies;
                    case "icon-picture":
                        return ItemType.photos;
                    case "icon-paper-clip":
                        return ItemType.other;
                    default:
                        return ItemType.other;
                }
            };

            Func<FreeDiscWebSearchResponseModel, Datum, string> getAuthor = (model, item) =>
            {
                string result = String.Empty;
                foreach (var userData in model?.response?.logins_translated)
                {
                    if (item?.user_id == userData.Value?.userID)
                    {
                        result += userData.Value?.display ?? String.Empty;
                    }
                }
                foreach (var folder in model?.response?.directories_translated)
                {
                    if(item?.parent_id == folder.Value?.id)
                    {
                        result += (result.Length > 0) && (folder.Value?.name ?? String.Empty).Length > 0 ? 
                                String.Concat(" - " , folder.Value?.name ): folder.Value?.name ?? String.Empty;
                        break;
                    }
                }
                return result;
            };

            foreach (var item in responseModel?.response?.data_files?.data)
            {
                OutCollection.Add(
                    new FreeDiscItem
                    {
                        Title = item?.extension?.Length > 0 ? String.Concat(item?.name ?? String.Empty, ".", item?.extension ?? String.Empty) : item?.name ?? String.Empty,
                        ImageUrl = $@"https://img.freedisc.pl/photo/{item.id}/1/2/{item.name_url}.png",
                        SizeFormat = item?.size_format ?? "-",
                        DateFormat = item?.date_add_format ?? "-",
                        FolderDesc = getAuthor(responseModel,item),
                        TypeImage = getImageType(item?.icon),
                    }
                );
            }   



            Debug.WriteLine(responseString);
            return true;
        }
        /*Task<FreeDiscItem> GetItemFormDbByIdAsync(int id, Action<string> statusLog)
        {

        }
        Task<bool> AddItemToDbAsync(FreeDiscItem freeDiscItem, Action<string> statusLog)
        {

        }
        Task<bool> UpdateItemDbAsync(FreeDiscItem freeDiscItem, Action<string> statusLog)
        {

        }
        Task<bool> DeleteItemDbAsync(int id, Action<string> statusLog)
        {

        }*/
    }
}
