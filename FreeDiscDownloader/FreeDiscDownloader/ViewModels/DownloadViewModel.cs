using FreeDiscDownloader.Extends;
using FreeDiscDownloader.Models;
using FreeDiscDownloader.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FreeDiscDownloader.ViewModels
{
    class DownloadViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public TrulyObservableCollection<FreeDiscItemDownload> DownloadItemList { get; private set; } = new TrulyObservableCollection<FreeDiscItemDownload>();
        private readonly IFreeDiscItemDownloadRepository _freeDiscItemDownloadRepository = ViewModelLocator.IFreeDiscItemDownloadRepository;

        public ICommand ItemDownloadButton { get; private set; }
        public ICommand ItemSelected { get; private set; }

        public int ItemImageHeight { get; private set; }
        public int ItemImageWidth { get; private set; }
        public int ItemRowHeight { get { return ItemImageHeight + 8; } }

        public DownloadViewModel()
        {
            Debug.Write("DownloadViewModel()");

            ItemImageWidth = (int)Math.Ceiling(App.DisplayScreenWidth / 3.4);
            ItemImageHeight = (int)Math.Ceiling((double)ItemImageWidth * 0.6875);
            _freeDiscItemDownloadRepository.LoadFromDB(DownloadItemList);

            ItemDownloadButton = new Command<FreeDiscItemDownload>(async (item) =>
            {
                switch(item?.ItemStatus){
                    case DownloadStatus.DownloadInProgress:
                        _freeDiscItemDownloadRepository.AbortDownloadItem();
                        break;
                    case DownloadStatus.DownloadInterrupted:
                    case DownloadStatus.WaitingForDownload:
                        if(!IsDownloadInProgress())
                            await _freeDiscItemDownloadRepository.DownloadItemAsync(item);
                        break;
                }
            });

            ItemSelected = new Command<FreeDiscItemDownload>(async (item) =>
            {
                List<Tuple<int, string>> Options = new List<Tuple<int, string>>();

                bool isDownloadingNow = IsDownloadInProgress();
                switch (item.ItemStatus)
                {
                    case DownloadStatus.DownloadFinish:
                        if(ExtensionMethods.IsValidPath(item?.FilePath, true) && File.Exists(item?.FilePath))
                            Options.Add(new Tuple<int, string>(9, "\u2022 Otwórz plik"));
                        break;
                    case DownloadStatus.DownloadInProgress:
                        Options.Add(new Tuple<int, string>(2, "\u2022 Anuluj pobieranie"));
                        break;
                    case DownloadStatus.WaitingForDownload:
                        if (!isDownloadingNow) Options.Add(new Tuple<int, string>(3, "\u2022 POBIERZ \u2022"));
                        break;
                }

                Options.Add(new Tuple<int, string>(7, "\u2022 Otwórz strone pliku"));
                Options.Add(new Tuple<int, string>(8, "\u2022 Kopiuj tytuł do schowka"));

                if (item.ItemStatus != DownloadStatus.DownloadInProgress)
                    Options.Add(new Tuple<int, string>(4, "\u2022 Usuń element z listy"));
                Options.Add(new Tuple<int, string>(5, "\u2022 Usuń wszystkie pobrane z listy"));
                Options.Add(new Tuple<int, string>(6, "\u2022 Usuń wszystko z listy"));

                if(item.ItemStatus == DownloadStatus.DownloadInterrupted || 
                    item.ItemStatus == DownloadStatus.DownloadFinish)
                        if (!isDownloadingNow) Options.Add(new Tuple<int, string>(1, "\u2022 Pobierz ponownie"));

                var optionsArray = new string[Options.Count];
                for (int i = 0; i < optionsArray.Length; i++)
                {
                    optionsArray[i] = Options[i].Item2;
                }

                var userChoose = await Application.Current.MainPage.DisplayActionSheet(item?.Title, "Anuluj", String.Empty, optionsArray);
                var userActionID = 0;
                foreach (var itemO in Options)
                    if(itemO.Item2 == userChoose)
                    {
                        userActionID = itemO.Item1;
                        break;
                    }

                Action<IList<FreeDiscItemDownload>> rowEve = (DownloadItemList) =>
                {
                    if (DownloadItemList.Count >= 2)
                    {
                        var firstEven = DownloadItemList[0].RowEven;
                        for (int i = 1; i < DownloadItemList.Count; i++)
                        {
                            DownloadItemList[i].RowEven = !firstEven;
                            firstEven = !firstEven;
                        }
                    }
                };
            
                switch (userActionID)
                {
                    case 1:
                    case 3:
                        if (!isDownloadingNow)
                            await _freeDiscItemDownloadRepository.DownloadItemAsync(item);
                        break;
                    case 2:
                        _freeDiscItemDownloadRepository.AbortDownloadItem();
                        break;
                    case 4:
                        if(item.ItemStatus != DownloadStatus.DownloadInProgress)
                        {
                            DownloadItemList.Remove(item);
                            await _freeDiscItemDownloadRepository.DeleteFromDBAsync(item);
                            rowEve(DownloadItemList);
                        }
                        break;
                    case 5:
                        for (int i = DownloadItemList.Count - 1; i >= 0; i--)
                            if(DownloadItemList[i].ItemStatus == DownloadStatus.DownloadFinish)
                            {
                                await _freeDiscItemDownloadRepository.DeleteFromDBAsync(DownloadItemList[i]);
                                DownloadItemList.RemoveAt(i);
                            }
                        rowEve(DownloadItemList);
                        break;
                    case 6:
                        for (int i = DownloadItemList.Count - 1; i >= 0; i--)
                            if (DownloadItemList[i].ItemStatus != DownloadStatus.DownloadInProgress)
                            {
                                await _freeDiscItemDownloadRepository.DeleteFromDBAsync(DownloadItemList[i]);
                                DownloadItemList.RemoveAt(i);
                            }
                        rowEve(DownloadItemList);
                        break;
                    case 7:
                        try
                        { Device.OpenUri(new Uri(item?.UrlSite)); }
                        catch { }
                        break;
                    case 8:
                        Plugin.Clipboard.CrossClipboard.Current.SetText(item?.Title);
                        break;
                    case 9:
                        DependencyService.Get<IDocumentViewer>().ShowDocumentFile(item?.FilePath);
                        break;
                }
            });
        }

        // add new item from search model
        public async Task AddNewItemToDownloadAsync(FreeDiscItem itemToAdd)
        {
            Debug.WriteLine("AddNewItemToDownload()");
            if (itemToAdd == null)
            {
                Debug.WriteLine("AddNewItemToDownload: itemToAdd = null");
                return;
            }
            if (IsFreeDiscItemDownloadOnQueue(itemToAdd))
            {
                Debug.WriteLine( "Element już istnieje na liście do pobrania" + itemToAdd.Title );
                return;
            }

            var downloaditem = new FreeDiscItemDownload(itemToAdd)
            {
                RowEven = (DownloadItemList?.Count ?? 0) > 0 ? !DownloadItemList[0]?.RowEven ?? true : true
            };

            DownloadItemList.Insert(0, downloaditem);
            await _freeDiscItemDownloadRepository.SaveToDBAsync(downloaditem);
            await DownloadQueueProcessAsync();
        }

        public bool IsFreeDiscItemDownloadOnQueue(FreeDiscItem itemToCheck)
        {
            if(itemToCheck == null) return false;
            if(DownloadItemList.Count > 0)
                for (int i = 0; i < DownloadItemList.Count; i++)
                    if(DownloadItemList[i].IdFreedisc == itemToCheck.IdFreedisc) return true;
            return false;
        }

        protected bool IsDownloadInProgress()
        {
            for (int i = 0; i < DownloadItemList.Count; i++)
                if (DownloadItemList[i].ItemStatus == DownloadStatus.DownloadInProgress) return true;
            return false;
        }

        // process download queue
        protected async Task DownloadQueueProcessAsync(bool processAll = false)
        {
            Debug.WriteLine("DownloadQueueProcess()");
            if (IsDownloadInProgress()) return;
            if (DownloadItemList.Count == 0) return;

            for (int i = 0; i < DownloadItemList.Count; i++)
                if (DownloadItemList[i].ItemStatus == DownloadStatus.WaitingForDownload)
                {
                    await _freeDiscItemDownloadRepository.DownloadItemAsync(DownloadItemList[i]);
                    if(processAll)
                        await DownloadQueueProcessAsync();
                    return;
                }
        }
  
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
