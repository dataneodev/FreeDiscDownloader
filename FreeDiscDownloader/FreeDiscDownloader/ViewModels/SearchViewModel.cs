using FreeDiscDownloader.Models;
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
        private readonly SearchPage searchPageReference;
        private readonly IFreeDiscItemRepository dataRepository;
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<FreeDiscItem> SearchItemList { get; private set; } = new ObservableCollection<FreeDiscItem>();
        
        private string searchText = String.Empty;
        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                if(value != searchText)
                {
                    searchText = value;
                    OnPropertyChanged("SearchText");
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
                OnPropertyChanged("FotterText");
            }
        }

        private readonly Action<string> setUserStatus;
        public ICommand FilterChooseButton { get; private set; }
        public ICommand SearchtextChange{ get; private set; }
        public ICommand SearchItemClicked { get; private set; }

        public int ItemImageHeight { get; private set; }
        public int ItemImageWidth { get; private set; }
        public int ItemRowHeight { get { return ItemImageHeight + 8; } }

        private struct ItemTypeUser
        {
            public ItemType ItemType;
            public string displayText;
        }

        private List<ItemTypeUser> ItemTypeTranslate = new List<ItemTypeUser>()
        {
            new ItemTypeUser{ ItemType = ItemType.all, displayText = "Wszystko" },
            new ItemTypeUser{ ItemType = ItemType.movies, displayText = "Filmy" },
            new ItemTypeUser{ ItemType = ItemType.music, displayText = "Music" },
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

        public SearchViewModel(SearchPage searchPageReference, IFreeDiscItemRepository _dataRepository)
        {
            this.searchPageReference = searchPageReference;
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
                setUserStatus($@"Szukam ""{searchText}"" [{GetItemTypeUserText(DefaultItemType)}]...");
                var searchRecord = new SearchItem
                {
                    SearchPatern = searchText,
                    SearchType = DefaultItemType,
                };

                var result = await dataRepository.SearchItemWebAsync(searchRecord, SearchItemList, setUserStatus);
                if (result) {
                    setUserStatus("Zakończono wyszukiwanie");
                } else {
                    setUserStatus("Wystąpił błąd podczas wyszukiwania.");
                }
            });

            SearchItemClicked = new Command<FreeDiscItem>( (selectedItem) =>
            {

            });
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
