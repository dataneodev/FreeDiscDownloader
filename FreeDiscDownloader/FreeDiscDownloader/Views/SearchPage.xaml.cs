using Xamarin.Forms;
using FreeDiscDownloader.ViewModels;
using FreeDiscDownloader.Services;

namespace FreeDiscDownloader
{
    public partial class SearchPage : ContentPage
    {
        public SearchPage()
        {
            InitializeComponent();
            BindingContext = new SearchViewModel(new FreeDiscItemRepository());
        }

        private void listItem_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {

        }
    }
}
