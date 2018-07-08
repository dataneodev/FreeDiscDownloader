using FreeDiscDownloader.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace FreeDiscDownloader.ViewModels
{
    
    public class ToggledButtonDef
    {
        private Action<string> PropertyChange;
        private string propertyName;
        public ItemType ButtonItemType { get; set; }
        private bool toggleStage = false;
        public bool ToggleStage
        {
            get { return toggleStage; }
            set
            {
                toggleStage = value;
                PropertyChange(propertyName);
            }
        }
        public ToggledButtonDef(Action<string> PropertyChange, string propertyName, ItemType ButtonItemType)
        {
            this.PropertyChange = PropertyChange;
            this.propertyName = propertyName;
            this.ButtonItemType = ButtonItemType;
        }
    }
    
    public class ToggledButtonPara
    {
        public bool IsToggledB { get; set; }
        public ItemType ItemTypeB { get; set; }
    }

    public sealed class SearchViewModel : INotifyPropertyChanged
    {
        private readonly SearchPage searchPageReference;
        private readonly IFreeDiscItemRepository dataRepository;
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<FreeDiscItem> SearchItemList { get; private set; } = new ObservableCollection<FreeDiscItem>();
        
        private bool allToggleState;
        public bool AllToggleState
        {
            get { return allToggleState; }
            set
            {
                if (value != allToggleState)
                {
                    allToggleState = value;
                    OnPropertyChanged("AllToggleState");
                }
            }
        }

        private bool movieToggleState;
        public bool MovieToggleState
        {
            get { return movieToggleState; }
            set
            {
                if (value != movieToggleState)
                {
                    movieToggleState = value;
                    OnPropertyChanged("MovieToggleState");
                }
            }
        }

        private bool musicToggleState;
        public bool MusicToggleState
        {
            get { return musicToggleState; }
            set
            {
                if (value != musicToggleState)
                {
                    musicToggleState = value;
                    OnPropertyChanged("MusicToggleState");
                }
            }
        }

        private bool pictureToggleState;
        public bool PictureToggleState
        {
            get { return pictureToggleState; }
            set
            {
                if (value != pictureToggleState)
                {
                    pictureToggleState = value;
                    OnPropertyChanged("PictureToggleState");
                }
            }
        }

        private bool otherToggleState;
        public bool OtherToggleState
        {
            get { return otherToggleState; }
            set
            {
                if (value != otherToggleState)
                {
                    otherToggleState = value;
                    OnPropertyChanged("OtherToggleState");
                }
            }
        }

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

        public ICommand ToggleButtonItemTypeSelect { get; private set; }
        public ICommand SearchtextChange{ get; private set; }
        public ICommand SearchItemClicked { get; private set; }

        public SearchViewModel(SearchPage searchPageReference, IFreeDiscItemRepository _dataRepository)
        {
            this.searchPageReference = searchPageReference;
            this.dataRepository = _dataRepository;
            setUserStatus = msg => FotterText = msg;
            AllToggleState = true;

            Func<ItemType, bool> getToogle = (item) =>
            {
                switch (item)
                {
                    case ItemType.all:
                        return AllToggleState;
                    case ItemType.movies:
                        return MovieToggleState;
                    case ItemType.music:
                        return MusicToggleState;
                    case ItemType.photos:
                        return PictureToggleState;
                    case ItemType.other:
                        return OtherToggleState;
                    default:
                        return OtherToggleState;
                };
            };

            Action<ItemType, bool> setToogle = (item, value) =>
            {
                switch (item)
                {
                    case ItemType.all:
                        AllToggleState = value;
                        break;
                    case ItemType.movies:
                        MovieToggleState = value;
                        break;
                    case ItemType.music:
                        MusicToggleState = value;
                        break;
                    case ItemType.photos:
                        PictureToggleState = value;
                        break;
                    case ItemType.other:
                        OtherToggleState = value;
                        break;
                    default:
                        OtherToggleState = value;
                        break;
                };
            };

            ToggleButtonItemTypeSelect = new Command<ItemType>( (ItemType) =>
            {
                foreach (ItemType itemcur in Enum.GetValues(typeof(ItemType)))
                {
                    if (ItemType == itemcur)
                    {
                        setToogle(itemcur, true);
                    }
                    else
                    {
                        setToogle(itemcur, false);
                    }  
                }

                if (SearchText.Length > 0) { SearchtextChange.Execute(SearchText); }
            });

            SearchtextChange = new Command<string>(async (searchText) =>
            {
                setUserStatus($@"Szukam ""{searchText}""...");

                Func<ItemType> getType = () =>
                {
                    foreach (ItemType itemcur in Enum.GetValues(typeof(ItemType)))
                    {
                        if (getToogle(itemcur)) { return itemcur; }
                    }
                    return ItemType.photos;
                };

                var searchRecord = new SearchItem
                {
                    SearchPatern = searchText,
                    SearchType = getType(),
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
