using FreeDiscDownloader.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace FreeDiscDownloader.ViewModels
{
    public class ButtonTextFromItemStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch((DownloadStatus)value)
            {
                case DownloadStatus.DownloadFinish:
                    return "Zakończono";
                case DownloadStatus.DownloadInProgress:
                    return "Pobieranie w trakcie";
                case DownloadStatus.DownloadInterrupted:
                    return "Bład pobrrania";
                case DownloadStatus.WaitingForDownload:
                    return "Oczekuje na pobranie";
                default:
                    return "Oczekuje na pobranie";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class DownloadOptionsItemStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((DownloadStatus)value == DownloadStatus.DownloadInProgress || 
                (DownloadStatus)value == DownloadStatus.DownloadInterrupted)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
