using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace FreeDiscDownloader.ViewModels
{
    public class SearchViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public readonly ICollection<SearchItem> searchItemList = new ObservableCollection<SearchItem>();
        public readonly ICollection<SearchItemType> searchItemTypeList = new ObservableCollection<SearchItemType>();
        public string SearchText { get; set; } = String.Empty;



        public ICommand SelectItemTypeTB { get; private set; }

        public SearchViewModel()
        {
            SelectItemTypeTB = new Command<string>((type) =>
            {

            });

            // temp
            for (int i = 0; i < 12; i++)
            {
                searchItemList.Add(ExampleList());
            }

            searchItemTypeList.Add(new SearchItemType { Name = "Wszystko" });
            searchItemTypeList.Add(new SearchItemType { Name = "Filmy" });
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static SearchItem ExampleList()
        {
            return new SearchItem()
            {
                Title = "Test 1",
                Image = "https://img.freedisc.pl/photo/10472186/2/2/thor-s-jpg.png",
                Size = "Rozmiar: 14.7 MB",
                Autor = "Dodał: achromski ",
                Date = "02 sie 17 21:53",
                TypeImage = "A",

            };

        }
    }
}
