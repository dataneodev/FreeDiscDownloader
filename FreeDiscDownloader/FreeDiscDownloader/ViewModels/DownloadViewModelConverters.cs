using FreeDiscDownloader.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace FreeDiscDownloader.ViewModels
{
    public class StatusButtonFromItemStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((DownloadStatus)value)
            {
                case DownloadStatus.DownloadFinish:
                    return "Otwórz";
                case DownloadStatus.DownloadInProgress:
                    return "Anuluj";
                case DownloadStatus.DownloadInterrupted:
                    return "Pobierz ponownie";
                case DownloadStatus.WaitingForDownload:
                    return "Rozpocznij";
                default:
                    return "-";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }


    public class StatusLabelFromItemStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var free = (FreeDiscItemDownload)value;
            switch (free?.ItemStatus ?? DownloadStatus.WaitingForDownload)
            {
                case DownloadStatus.DownloadFinish:
                    return "Zakończono";
                case DownloadStatus.DownloadInProgress:
                    return $"Pobieranie... ({free.DownloadProgres:0%})";
                case DownloadStatus.DownloadInterrupted:
                    return "Bład pobrrania";
                case DownloadStatus.WaitingForDownload:
                    return "Oczekuje na pobranie";
                default:
                    return "-";
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
