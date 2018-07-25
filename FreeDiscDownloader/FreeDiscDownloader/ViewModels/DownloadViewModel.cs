using FreeDiscDownloader.Models;
using FreeDiscDownloader.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace FreeDiscDownloader.ViewModels
{
    class DownloadViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<FreeDiscItemDownload> DownloadItemList { get; private set; } = new ObservableCollection<FreeDiscItemDownload>();
        private readonly IFreeDiscItemDownloadRepository _freeDiscItemDownloadRepository = ViewModelLocator.IFreeDiscItemDownloadRepository;

        public ICommand ItemDownloadButton { get; private set; }

        public int ItemImageHeight { get; private set; }
        public int ItemImageWidth { get; private set; }
        public int ItemRowHeight { get { return ItemImageHeight + 8; } }

        public DownloadViewModel()
        {
            ItemImageWidth = (int)Math.Ceiling(App.DisplayScreenWidth / 3.4);
            ItemImageHeight = (int)Math.Ceiling((double)ItemImageWidth * 0.6875);

            ItemDownloadButton = new Command<FreeDiscItemDownload>((item) =>
            {

            });

            _freeDiscItemDownloadRepository.LoadFromDB(DownloadItemList);
        }

        public void AddNewItemToDownload(FreeDiscItem itemToAdd)
        {
            if (IsFreeDiscItemDownloadExists(itemToAdd))
            {
                Application.Current.MainPage.DisplayAlert(itemToAdd.Title, "Element już istnieje na liście do pobrania","Anuluj");
                return;
            }

            var downloaditem = new FreeDiscItemDownload(itemToAdd);
            DownloadItemList.Add(downloaditem);
            Application.Current.MainPage.DisplayAlert(downloaditem.Title, downloaditem.Title, "Anuluj");
        }

        public bool IsFreeDiscItemDownloadExists(FreeDiscItem itemToCheck)
        {
            if(DownloadItemList.Count > 0)
            {
                for (int i = 0; i <= DownloadItemList.Count - 1; i++)
                {
                    if(DownloadItemList[i].IdFreedisc == itemToCheck.IdFreedisc)
                    {
                        return true;
                    }
                }
            }
            return false;
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
