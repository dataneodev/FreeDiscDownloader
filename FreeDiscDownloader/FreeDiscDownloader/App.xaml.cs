using FFImageLoading;
using FFImageLoading.Config;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace FreeDiscDownloader
{
	public partial class App : Application
	{
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

            //
            InitializeComponent();
            MainPage = new TabbedPage
            {
                Children = {
                    new SearchPage(),
                    new DonwloadPage(),
                    new SettingPage()
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
