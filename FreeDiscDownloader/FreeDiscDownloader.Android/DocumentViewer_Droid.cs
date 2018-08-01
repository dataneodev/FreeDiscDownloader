using System;
using Android.Content;
using Xamarin.Forms;
using FreeDiscDownloader.Services;
using FreeDiscDownloader.Droid;
using FreeDiscDownloader.Extends;
using Android.Webkit;
using System.IO;
using Android.OS;

[assembly: Dependency(typeof(DocumentViewer_Droid))]
namespace FreeDiscDownloader.Droid
{
    public class DocumentViewer_Droid : IDocumentViewer
    {
        public void ShowDocumentFile(string filepath)
        {
            if (!ExtensionMethods.IsValidPath(filepath, true))
                return;
            if (!File.Exists(filepath))
                return;
            string fileExtension = String.Empty;
            try { 
                fileExtension = System.IO.Path.GetExtension(filepath);
            }
            catch (ArgumentException)
            {
                return;
            }
            string mimeType = null;
            if (!string.IsNullOrWhiteSpace(fileExtension))
            {
                fileExtension = fileExtension.Trim('.', ' ');
                mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(fileExtension);
            }
            else return;

            var uri = Android.Net.Uri.Parse("file://" + filepath);
            var intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(uri, mimeType);

            intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);

            try
            {
                global::Xamarin.Forms.Forms.Context.StartActivity(Intent.CreateChooser(intent, "Select App"));
            }
            catch (Exception ex)
            {
               // Application.Current.MainPage.DisplayAlert("Test", ex.ToString(), "OK");
                //Let the user know when something went wrong
            }
        }
    }
}