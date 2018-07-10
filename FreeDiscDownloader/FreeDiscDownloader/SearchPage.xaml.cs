using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FFImageLoading;
using FreeDiscDownloader.ViewModels;
using FreeDiscDownloader.Models;

namespace FreeDiscDownloader
{
    public partial class SearchPage : ContentPage
    {
        readonly SearchViewModel SearchVM;

        public SearchPage()
        {
            InitializeComponent();
            SearchVM = new SearchViewModel(this, new FreeDiscItemRepository());
            BindingContext = SearchVM;
        }
    }
}
