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

        private ViewCell _previousCell;
        private Color _previousColor;
        private void ViewCell_Tapped(object sender, System.EventArgs e)
        {
            if (_previousCell != null)
            {
                _previousCell.View.BackgroundColor = _previousColor;
            }

            var viewCell = (ViewCell)sender;
            if (viewCell.View != null)
            {
                _previousColor = viewCell.View.BackgroundColor;
                _previousCell = viewCell;
                viewCell.View.BackgroundColor = App.selectedRow;
            }
        }
    }
}
