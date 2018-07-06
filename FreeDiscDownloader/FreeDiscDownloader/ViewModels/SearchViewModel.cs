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
    public sealed class SearchViewModel : INotifyPropertyChanged
    {
        private SearchPage searchPage;
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<FreeDiscItem> SearchItemList { get; set; } = new ObservableCollection<FreeDiscItem>();

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

        public ICommand ToggleButtonItemTypeSelect { get; private set; }
        public ICommand SearchtextChange{ get; private set; }
        public ICommand SearchItemClicked { get; private set; }

        public SearchViewModel(SearchPage searchPageReference)
        {
            this.searchPage = searchPageReference;
            AllToggleState = true;

            ToggleButtonItemTypeSelect = new Command<ToggleButton>((button) =>
            {
               /// button.@class.
            });

            SearchtextChange = new Command<string>((searchText) =>
            {

            });

            SearchItemClicked = new Command<FreeDiscItem>((selectedItem) =>
            {

            });

            // temp
            for (int i = 0; i < 12; i++)
            {
                SearchItemList.Add(ExampleList());
            }

 
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static FreeDiscItem ExampleList()
        {
            return new FreeDiscItem()
            {
                Title = "Test 1",
                ImageUrl = "https://img.freedisc.pl/photo/10472186/2/2/thor-s-jpg.png",
                Size = "Rozmiar: 14.7 MB",
                Autor = "Dodał: achromski ",
                Date = "02 sie 17 21:53",
                TypeImage = "A",

            };

        }
    }
}
