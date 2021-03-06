﻿using FreeDiscDownloader.Models;
using FreeDiscDownloader.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace FreeDiscDownloader.ViewModels
{
    public sealed class SearchViewModel : INotifyPropertyChanged
    {
        private readonly IFreeDiscItemRepository dataRepository;
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<FreeDiscItem> SearchItemList { get; private set; } = new ObservableCollection<FreeDiscItem>();
        
        private string searchText = String.Empty;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                if(value != searchText)
                {
                    searchText = value;
                    OnPropertyChanged();
                }
            }
        }

        private string fotterText = String.Empty;
        public string FotterText
        {
            get { return fotterText; }
            set
            {
                fotterText = value;
                OnPropertyChanged();
            }
        }

        private bool searchEnable = true;
        public bool SearchEnable
        {
            get { return searchEnable; }
            set
            {
                searchEnable = value;
                OnPropertyChanged();
            }
        }

        private readonly Action<string> setUserStatus;
        public ICommand FilterChooseButton { get; private set; }
        public ICommand SearchtextChange{ get; private set; }
        public ICommand SearchItemClicked { get; private set; }
        public ICommand LoadNextItem { get; private set; }

        public int ItemImageHeight { get; private set; }
        public int ItemImageWidth { get; private set; }
        public int ItemRowHeight { get { return ItemImageHeight + 8; } }

        public SearchItemWebResult lastItemsSearchResult = new SearchItemWebResult(); 

        private struct ItemTypeUser
        {
            public ItemType ItemType;
            public string displayText;
        }

        private List<ItemTypeUser> ItemTypeTranslate = new List<ItemTypeUser>()
        {
            new ItemTypeUser{ ItemType = ItemType.all, displayText = "Wszystko" },
            new ItemTypeUser{ ItemType = ItemType.movies, displayText = "Filmy" },
            new ItemTypeUser{ ItemType = ItemType.music, displayText = "Muzyka" },
            new ItemTypeUser{ ItemType = ItemType.photos, displayText = "Zdjęcia" },
            new ItemTypeUser{ ItemType = ItemType.other, displayText = "Pozostałe" },  
        };

        private ItemType defaultItemType;
        public ItemType DefaultItemType
        {
            get { return defaultItemType; }
            set
            {
                defaultItemType = value;
                OnPropertyChanged();
            }
        }

        private string GetItemTypeUserText(ItemType qitem)
        {
            foreach (var item in ItemTypeTranslate)
            {
                if (qitem == item.ItemType)
                {
                    return item.displayText;
                }
            }
            return String.Empty;
        }

        public SearchViewModel()
        {
            this.dataRepository = ViewModelLocator.IFreeDiscItemRepository;
            setUserStatus = msg => FotterText = msg;

            ItemImageWidth = (int) Math.Ceiling(App.DisplayScreenWidth / 3.4);
            ItemImageHeight = (int) Math.Ceiling((double)ItemImageWidth*0.6875);

            FilterChooseButton = new Command(async () =>
            {
                if((ItemTypeTranslate?.Count ?? 0) == 0) { return;  }
                String[] option = new String[ItemTypeTranslate?.Count ?? 0];
                for (int i = 0; i < (ItemTypeTranslate?.Count ?? 0); i++)
                {
                    option[i] = ItemTypeTranslate[i].displayText;
                }
                string optionChoose = await Application.Current.MainPage.DisplayActionSheet("Filtruj wg typu:", String.Empty, String.Empty, option);
                if (optionChoose == null || optionChoose.Length == 0) { return;  }
                DefaultItemType = ItemType.all;
                foreach (var item in ItemTypeTranslate)
                {
                    if(optionChoose == item.displayText)
                    {
                        DefaultItemType = item.ItemType;
                        break;
                    }
                }
                if (SearchText.Length > 0) { SearchtextChange.Execute(SearchText); }
            });

            SearchtextChange = new Command<string>((searchText) =>
            {
                SearchExecute(searchText, DefaultItemType, 0);
            });

            SearchItemClicked = new Command<FreeDiscItem>(async (selectedItem) =>
            {
                if (ViewModelLocator.DownloadViewModel.IsFreeDiscItemDownloadOnQueue(selectedItem))
                {
                    await Application.Current.MainPage.DisplayAlert(selectedItem?.Title, "Plik znajduje się już w zakładce POBRANE", "OK");
                    return;
                }

                List<Tuple<int, string>> Options = new List<Tuple<int, string>>
                {
                    new Tuple<int, string>(1, "\u2022 POBIERZ PLIK \u2022"),
                    new Tuple<int, string>(4, "\u2022 Otwórz strone pliku"),
                    new Tuple<int, string>(2, "\u2022 Kopiuj link strony do schowka"),
                    new Tuple<int, string>(3, "\u2022 Kopiuj tytuł do schowka")
                };

                var optionsArray = new string[Options.Count];
                for (int i = 0; i < optionsArray.Length; i++)
                {
                    optionsArray[i] = Options[i].Item2;
                }

                var userChoose = await Application.Current.MainPage.DisplayActionSheet(selectedItem?.Title, "Anuluj", String.Empty, optionsArray);
                var userActionID = 0;
                foreach (var itemO in Options)
                    if (itemO.Item2 == userChoose)
                    {
                        userActionID = itemO.Item1;
                        break;
                    }

                switch (userActionID)
                {
                    case 1:
                        var masterPage = Application.Current.MainPage as TabbedPage;
                        masterPage.CurrentPage = masterPage.Children[1];
                        await ViewModelLocator.DownloadViewModel.AddNewItemToDownloadAsync(selectedItem);
                        break;
                    case 2:
                        Plugin.Clipboard.CrossClipboard.Current.SetText(selectedItem?.UrlSite);
                        break;
                    case 3:
                        Plugin.Clipboard.CrossClipboard.Current.SetText(selectedItem?.Title);
                        break;
                    case 4:
                        try
                        {  Device.OpenUri(new Uri(selectedItem?.UrlSite)); }
                        catch (Exception) { }
                        break;
                }
            });

            LoadNextItem = new Command(() =>
            {
                if (lastItemsSearchResult.Allpages > 1 && lastItemsSearchResult.Page < lastItemsSearchResult.Allpages - 1 && 
                    lastItemsSearchResult.Page < App.AppSetting.ListLoadCount - 1 && SearchEnable)
                {
                   SearchExecute(SearchText, DefaultItemType, lastItemsSearchResult.Page + 1, lastItemsSearchResult.Allpages);
                }
            });
        }

        private async void SearchExecute(string searchText, ItemType itemType, int page, int pages = 0)
        {
            string status = $@"Szukam ""{searchText}"" [{GetItemTypeUserText(itemType)}] ...";
            if(page > 0)
                status = $@"Ładuje strone {page+1} z {pages} [{GetItemTypeUserText(itemType)}] ...";
            
            setUserStatus(status);
            
            var searchRecord = new SearchItemWebRequest()
            {
                SearchPatern = searchText,
                SearchType = itemType,
                Page = page,
                AllPages = pages
            };

            SearchEnable = false;
            lastItemsSearchResult = await dataRepository.SearchItemWebAsync(searchRecord, 
                                    (SearchItemList.Count>0)?!SearchItemList[SearchItemList.Count-1].RowEven:false, setUserStatus);
            SearchEnable = true;

            if (lastItemsSearchResult.Correct)
            {
                setUserStatus("Zakończono wyszukiwanie");
                if (pages == 0)
                    SearchItemList.Clear();
                foreach (var item in lastItemsSearchResult.CollectionResult)
                    SearchItemList.Add(item);
            }
            else
            {
                setUserStatus("Wystąpił błąd podczas wyszukiwania.");
            }
        }

        // Create the OnPropertyChanged method to raise the event
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }


    }
}
