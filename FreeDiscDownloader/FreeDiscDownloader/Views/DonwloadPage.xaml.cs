using FreeDiscDownloader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FreeDiscDownloader
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DonwloadPage : ContentPage
	{
		public DonwloadPage ()
		{
			InitializeComponent ();
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