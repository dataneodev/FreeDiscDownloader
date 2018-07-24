using FreeDiscDownloader.Models;
using FreeDiscDownloader.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace FreeDiscDownloader.ViewModels
{
    class DownloadViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public readonly ObservableCollection<FreeDiscItemDownload> DownloadItemList = new ObservableCollection<FreeDiscItemDownload>();
        private readonly IFreeDiscItemDownloadRepository _freeDiscItemDownloadRepository = ViewModelLocator.IFreeDiscItemDownloadRepository;

        public DownloadViewModel()
        {
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

        }

        public bool IsFreeDiscItemDownloadExists(FreeDiscItem itemToCheck)
        {
            if(DownloadItemList.Count > 0)
            {
                for (int i = 0; i < DownloadItemList.Count - 1; i++)
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
