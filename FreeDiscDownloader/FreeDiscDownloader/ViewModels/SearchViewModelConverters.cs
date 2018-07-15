using FreeDiscDownloader.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace FreeDiscDownloader.ViewModels
{
    public class RowColorFromBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if( (bool) value) 
            { return App.normalRow; }
            else
            { return App.highlightRow; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class ButtonColorFromItemType : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((ItemType)value == ItemType.all)
            {
                return App.highlightRow;
            }
            else
            {
                return App.buttonToggled;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class ItemTypeToImageResource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return String.Format("resource://FreeDiscDownloader.img.{0}.png", ((ItemType)value).ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
