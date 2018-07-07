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
