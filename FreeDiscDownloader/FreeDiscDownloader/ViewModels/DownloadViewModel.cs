using FreeDiscDownloader.Extends;
using FreeDiscDownloader.Models;
using FreeDiscDownloader.Services;
using Plugin.DownloadManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
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
            ItemImageWidth = (int)Math.Ceiling(App.DisplayScreenWidth / 3.4);
            ItemImageHeight = (int)Math.Ceiling((double)ItemImageWidth * 0.6875);
            _freeDiscItemDownloadRepository.LoadFromDB(DownloadItemList);

            ItemDownloadButton = new Command<FreeDiscItemDownload>((item) =>
            {
                switch(item?.ItemStatus){
                    case DownloadStatus.DownloadInProgress:
                        _freeDiscItemDownloadRepository.AbortDownloadItem();
                        break;
                    case DownloadStatus.DownloadInterrupted:
                    case DownloadStatus.WaitingForDownload:
                        StartDownload(item);
                        break;
                }
            });

            ItemSelected = new Command<FreeDiscItemDownload>((item) =>
            {
                if(item != null && !IsDownloadInProgress())
                {
                   // item.
                }
            });
        }

        // add new item from search model
        public void AddNewItemToDownload(FreeDiscItem itemToAdd)
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

            _freeDiscItemDownloadRepository.SaveToDB(downloaditem);
            DownloadItemList.Insert(0, downloaditem);
            DownloadQueueProcess();
        }

        protected bool IsFreeDiscItemDownloadOnQueue(FreeDiscItem itemToCheck)
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
        protected void DownloadQueueProcess()
        {
            Debug.WriteLine("DownloadQueueProcess()");
            if (IsDownloadInProgress()) return;
           
            for (int i = 0; i < DownloadItemList.Count; i++)
                if (DownloadItemList[i].ItemStatus == DownloadStatus.DownloadInterrupted ||
                    DownloadItemList[i].ItemStatus == DownloadStatus.WaitingForDownload)
                {
                    StartDownload(DownloadItemList[i]);
                    break;
                }
        }
        //
        protected void StartDownload(FreeDiscItemDownload idItemToDownload)
        {
            Debug.WriteLine("StartDownload: downloaded item: " + idItemToDownload.Title);
            _freeDiscItemDownloadRepository.DownloadItem( idItemToDownload );
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
