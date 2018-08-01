using System;
using UIKit;
using Xamarin.Forms;
using FreeDiscDownloader.iOS;
using FreeDiscDownloader.Services;
using System.IO;
using QuickLook;
using Foundation;

[assembly: Dependency(typeof(DocumentViewer_iOS))]
namespace FreeDiscDownloader.iOS
{
    public class DocumentViewer_iOS : IDocumentViewer
    {
        public void ShowDocumentFile(string filepath)
        {
            var fileinfo = new FileInfo(filepath);
            var previewController = new QLPreviewController();
            previewController.DataSource = new PreviewControllerDataSource(fileinfo.FullName, fileinfo.Name);

            UINavigationController controller = FindNavigationController();

            if (controller != null)
            {
                controller.PresentViewController((UIViewController)previewController, true, (Action)null);
            }
        }

        private UINavigationController FindNavigationController()
        {
            foreach (var window in UIApplication.SharedApplication.Windows)
            {
                if (window.RootViewController.NavigationController != null)
                {
                    return window.RootViewController.NavigationController;
                }
                else
                {
                    UINavigationController value = CheckSubs(window.RootViewController.ChildViewControllers);
                    if (value != null)
                    {
                        return value;
                    }
                }
            }
            return null;
        }

        private UINavigationController CheckSubs(UIViewController[] controllers)
        {
            foreach (var controller in controllers)
            {
                if (controller.NavigationController != null)
                {
                    return controller.NavigationController;
                }
                else
                {
                    UINavigationController value = CheckSubs(controller.ChildViewControllers);
                    if (value != null)
                    {
                        return value;
                    }
                }

                return null;
            }
            return null;
        }
    }

    public class DocumentItem : QLPreviewItem
    {
        private string _title;
        private string _uri;

        public DocumentItem(string title, string uri)
        {
            _title = title;
            _uri = uri;
        }

        public override string ItemTitle
        { get { return _title; } }

        public override NSUrl ItemUrl
        { get { return NSUrl.FromFilename(_uri); } }
    }

    public class PreviewControllerDataSource : QLPreviewControllerDataSource
    {
        private string _url;
        private string _filename;

        public PreviewControllerDataSource(string url, string filename)
        {
            _url = url;
            _filename = filename;
        }

        public override IQLPreviewItem GetPreviewItem(QLPreviewController controller, nint index)
        {
            return (IQLPreviewItem)new DocumentItem(_filename, _url);
        }

        public override nint PreviewItemCount(QLPreviewController controller)
        { return (nint)1; }
    }
}
