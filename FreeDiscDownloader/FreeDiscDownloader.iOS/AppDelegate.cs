using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Foundation;
using Plugin.DownloadManager;
using UIKit;

namespace FreeDiscDownloader.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            App.DisplayScreenWidth = (double)UIScreen.MainScreen.Bounds.Width;
            App.DisplayScreenHeight = (double)UIScreen.MainScreen.Bounds.Height;
            App.DisplayScaleFactor = (double)UIScreen.MainScreen.Scale;

            global::Xamarin.Forms.Forms.Init();

            var dbpath = Path.Combine( System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".." , "Liblary");
            var storagepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            LoadApplication(new App(dbpath, storagepath));

            return base.FinishedLaunching(app, options);
        }

        public override void HandleEventsForBackgroundUrl(UIApplication application, string sessionIdentifier, Action completionHandler)
        {
            CrossDownloadManager.BackgroundSessionCompletionHandler = completionHandler;
        }
    }
}
