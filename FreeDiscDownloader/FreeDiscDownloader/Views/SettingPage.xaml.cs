using FreeDiscDownloader.Services;
using FreeDiscDownloader.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FreeDiscDownloader
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingPage : ContentPage
	{
		public SettingPage (IAppSettingRepository AppSetting)
		{
			InitializeComponent ();
            BindingContext = new SettingViewModel(AppSetting);
            tableSetting.BindingContext = AppSetting;
        }
	}
}