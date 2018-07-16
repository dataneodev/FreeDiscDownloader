using FFImageLoading;
using FFImageLoading.Config;
using FreeDiscDownloader.Services;
using System;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace FreeDiscDownloader
{
	public partial class App : Application
	{
        public static double DisplayScreenWidth = 0f;
        public static double DisplayScreenHeight = 0f;
        public static double DisplayScaleFactor = 0f;
        public static Color highlightRow = Color.FromHex("#e8e8e8");
        public static Color normalRow = Color.FromHex("#FFFFFF0");
        public static Color selectedRow = Color.FromHex("#f9fca9");
        public static Color buttonToggled = Color.FromHex("#5b5b5b");

        public App ()
		{
            // Initialize Live Reload.
            #if DEBUG
                LiveReload.Init();
            #endif

            // Replace default user-agent for image web request
            var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");

            ImageService.Instance.Initialize(new Configuration
            {
                HttpClient = client
            });

			InitializeComponent();

            IAppSettingRepository AppSetting = new AppSettingRepository("test");

            MainPage = new TabbedPage
            {
                Children = {
                    new SearchPage(AppSetting),
                    new DonwloadPage(AppSetting),
                    new SettingPage(AppSetting)
                }
            };
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
