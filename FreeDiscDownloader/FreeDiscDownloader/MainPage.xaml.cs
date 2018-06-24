using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FreeDiscDownloader
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
            
			InitializeComponent();
            listItem.ItemsSource = ExampleList();

        }

        private static IEnumerable<Item> ExampleList()
        {
            return new List<Item>()
            {
                new Item()
                {
                    Title = "MICROSOFT_PRESS_EBOOK_XAMARIN_PREVIEW_2_PDF.PDF",
                    Image = @"‪https://placeimg.com/100/100/people/2",
                    Size = "Rozmiar: 14.7 MB",
                    Autor = "Dodał: achromski ",
                    Date = "02 sie 17 21:53",
                    TypeImage = "A",
                },
                new Item()
                {
                    Title = "MICROSOFT_PRESS_EBOOK_XAMARIN_PREVIEW_2_PDF.PDF",
                    Image = @"‪https://placeimg.com/100/100/people/2",
                    Size = "Rozmiar: 14.7 MB",
                    Autor = "Dodał: achromski ",
                    Date = "02 sie 17 21:53",
                    TypeImage = "A",
                },
                new Item()
                {
                    Title = "MICROSOFT_PRESS_EBOOK_XAMARIN_PREVIEW_2_PDF.PDF",
                    Image = @"‪https://placeimg.com/100/100/people/2",
                    Size = "Rozmiar: 14.7 MB",
                    Autor = "Dodał: achromski ",
                    Date = "02 sie 17 21:53",
                    TypeImage = "A",
                },
            };

        }

	}
}
