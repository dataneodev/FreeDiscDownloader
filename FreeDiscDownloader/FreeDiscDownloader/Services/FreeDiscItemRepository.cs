﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FreeDiscDownloader.Models;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace FreeDiscDownloader.Services
{
    public class SearchItemWebRequest
    {
        public string SearchPatern { get; set; }
        public int Page { get; set; }
        public int AllPages { get; set; }
        public ItemType SearchType { get; set; }
    }

    public class SearchItemWebResult
    {
        public bool Correct { get; set; }
        public int Page { get; set; }
        public int Allpages { get; set; }
        public IList<FreeDiscItem> CollectionResult { get; set; }
    }

    class FreeDiscItemRepository : IFreeDiscItemRepository
    {
        public async Task<SearchItemWebResult> SearchItemWebAsync(SearchItemWebRequest searchItem, bool nexRowEven, Action<string> statusLog)
        {
            const string freeDiscSearchUrl = "https://freedisc.pl/search/get";
            SearchItemWebResult result = new SearchItemWebResult()
            {
                Correct = false,
                Page = 0,
                Allpages = 0,
                CollectionResult = new List<FreeDiscItem>(),
            };

            if(searchItem == null)
            {
                Debug.WriteLine("SearchItemWebAsync: searchItem == null");
                return result;
            }

            var searchObj = new
            {
                search_phrase = searchItem.SearchPatern,
                search_type = searchItem.SearchType.ToString(),
                search_saved = "0",
                search_page = searchItem.Page.ToString(),
                pages = searchItem.AllPages,
                limit = 25,
                search_size_condition = "gte",
                search_size_value = "0"
            };

            var postData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(searchObj) );

            HttpWebRequest webRequest = null;
            try
            {
                webRequest = WebRequest.CreateHttp(freeDiscSearchUrl);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:53.0) Gecko/20100101 Firefox/53.0";
                webRequest.Accept = "application/json, text/javascript, */*; q=0.01";
                webRequest.Referer = $@"https://freedisc.pl/search/{searchObj.search_type}/{WebUtility.UrlEncode(searchItem.SearchPatern)}/";
                webRequest.ContentLength = postData.Length;
                webRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
                webRequest.Headers.Add("Accept-Language", "pl,en-US;q=0.7,en;q=0.3");
                webRequest.Headers.Add("Accept-Encoding", "");
                webRequest.Timeout = 12000; //12sec
                using (var stream = webRequest.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }
            }
            catch (System.Exception e)
            {
                Debug.WriteLine("SearchItemWebAsync: WebRequest Exception: " + e.Message.ToString());
                return result;
            }

            WebResponse webResponse = null;
            String responseString = String.Empty;
            try
            {
                webResponse = await webRequest.GetResponseAsync().ConfigureAwait(false);
                using (var sr = new System.IO.StreamReader(webResponse.GetResponseStream()))
                {
                    responseString = await sr.ReadToEndAsync();
                }
            }
            catch (System.Exception e)
            {
                Debug.WriteLine("SearchItemWebAsync: WebResponse Exception: " + e.Message.ToString());
                return result;
            }
            //Debug.WriteLine("responseString: " + responseString);

            if (responseString.Length == 0)
            {
                Debug.WriteLine("SearchItemWebAsync: data.Length == 0 ");
                return result;
            }

            FreeDiscWebSearchResponseModel responseModel;
            try
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                responseModel = JsonConvert.DeserializeObject<FreeDiscWebSearchResponseModel>(responseString, settings);
            }
            catch(System.Exception e)
            {
                Debug.WriteLine("SearchItemWebAsync: DeserializeObject<FreeDiscWebSearchResponseModel> problem: " +  e.Message.ToString());
                return result;
            }

            if(! responseModel?.success ?? false)
            {
                Debug.WriteLine("SearchItemWebAsync: !responseModel.success");
                return result;
            }

            if((responseModel?.response?.data_files?.hits ?? 0) == 0 )
            {
                result.Correct = true;
                return result; // noting found
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

            Func<FreeDiscWebSearchResponseModel, Datum, string> getAuthorName = (model, item) =>
            {
                foreach (var userData in model?.response?.logins_translated)
                {
                    if (item?.user_id == userData.Value?.userID)
                    {
                        return userData.Value?.display ?? String.Empty;
                    }
                }
                return String.Empty;
            };

            Func<FreeDiscWebSearchResponseModel, Datum, string> getFolderName = (model, item) =>
            {
                foreach (var folder in model?.response?.directories_translated)
                {
                    if (item?.parent_id == folder.Value?.id)
                    {
                        return folder.Value?.name ?? String.Empty;
                    }
                }
                return String.Empty;
            };

            bool rowEven = nexRowEven;
            foreach (var item in responseModel?.response?.data_files?.data)
            {
                result.CollectionResult.Add(
                    new FreeDiscItem
                    {
                        IdFreedisc = item?.id ?? 0,
                        Title = item?.extension?.Length > 0 ? String.Concat(item?.name ?? String.Empty, ".", item?.extension ?? String.Empty) : item?.name ?? String.Empty,
                        ImageUrl = $@"https://img.freedisc.pl/photo/{item.id}/1/2/{item.name_url}.png",
                        SizeFormat = item?.size_format ?? "-",
                        DateFormat = item?.date_add_format ?? "-",
                        FolderDesc = getAuthorName(responseModel, item) + " - " + getFolderName(responseModel, item),
                        TypeImage = getImageType(item?.icon),
                        UrlSite = @"https://freedisc.pl/" + getAuthorName(responseModel, item) + $@",f-{item?.id},{item?.name_url}",
                        Url = $"http://stream.freedisc.pl/video/{item?.id}/{item?.name_url}",
                        RowEven = rowEven,
                    }
                );
                rowEven = !rowEven;
            }

            result.Correct = true;
            result.Page = responseModel?.response?.page ?? 0;
            result.Allpages = responseModel?.response?.pages ?? 0;
            return result;
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
