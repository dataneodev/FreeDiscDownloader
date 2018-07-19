using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace FreeDiscDownloader.Droid
{
    [Activity(Label = "FreeDiscDownloader", 
        Icon = "@mipmap/icon", 
        Theme = "@style/MainTheme",
        MainLauncher = false, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            App.DisplayScreenWidth = (double)Resources.DisplayMetrics.WidthPixels / (double)Resources.DisplayMetrics.Density;
            App.DisplayScreenHeight = (double)Resources.DisplayMetrics.HeightPixels / (double)Resources.DisplayMetrics.Density;
            App.DisplayScaleFactor = (double)Resources.DisplayMetrics.Density;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            var dbpath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var storagepath = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

            LoadApplication(new App(dbpath, storagepath));
        }
    }
}

