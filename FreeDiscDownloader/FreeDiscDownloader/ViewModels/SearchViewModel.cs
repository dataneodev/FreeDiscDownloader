using FreeDiscDownloader.Models;
using FreeDiscDownloader.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
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

        public ItemType DefaultItemType { get; set; }
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

        public SearchViewModel(IFreeDiscItemRepository _dataRepository)
        {
            this.dataRepository = _dataRepository;
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
                string optionChoose = await Application.Current.MainPage.DisplayActionSheet("Filtruj wg typ:", "Anuluj", "OK", option);
                if(optionChoose == null || optionChoose.Length == 0 || optionChoose == "Anuluj") { return;  }
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

            SearchtextChange = new Command<string>(async (searchText) =>
            {
                await SearchExecute(searchText, DefaultItemType, 0);
            });

            SearchItemClicked = new Command<FreeDiscItem>( (selectedItem) =>
            {

            });

            LoadNextItem = new Command(async () =>
            {
                if (lastItemsSearchResult.Allpages > 1 && lastItemsSearchResult.Page < lastItemsSearchResult.Allpages - 1)
                {
                   await SearchExecute(SearchText, DefaultItemType, lastItemsSearchResult.Page + 1, lastItemsSearchResult.Allpages);
                }
            });
        }

        private async Task<bool> SearchExecute(string searchText, ItemType itemType, int page, int pages = 0)
        {
            string status = $@"Szukam ""{searchText}"" [{GetItemTypeUserText(itemType)}] ...";
            if(page > 0)
            {
                status = $@"Ładuje [{GetItemTypeUserText(itemType)}] Strona {page+1} z {pages}  ...";
            }
            setUserStatus(status);
            
            var searchRecord = new SearchItemWebRequest()
            {
                SearchPatern = searchText,
                SearchType = itemType,
                Page = page,
                AllPages = pages
            };

            SearchEnable = false;
            lastItemsSearchResult = await dataRepository.SearchItemWebAsync(searchRecord, SearchItemList, setUserStatus);
            SearchEnable = true;

            if (lastItemsSearchResult.Correct)
            {
                setUserStatus("Zakończono wyszukiwanie");
                return true;
            }
            else
            {
                setUserStatus("Wystąpił błąd podczas wyszukiwania.");
            }
            return false;
        }
        /*
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        */
        // Create the OnPropertyChanged method to raise the event
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }


    }
}
