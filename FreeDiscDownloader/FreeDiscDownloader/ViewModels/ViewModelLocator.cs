using CommonServiceLocator;
using FreeDiscDownloader.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Lifetime;
using Unity.ServiceLocation;

namespace FreeDiscDownloader.ViewModels
{
    static class ViewModelLocator
    {
        static private readonly UnityContainer unityContainer;

        static public SearchViewModel SearchViewModel {
            get { return unityContainer.Resolve<SearchViewModel>(); }
        }

        static public SettingViewModel SettingViewModel {
            get { return unityContainer.Resolve<SettingViewModel>(); }
        }

        static public DownloadViewModel DownloadViewModel {
            get { return unityContainer.Resolve<DownloadViewModel>(); }
        }

        static public IFreeDiscItemRepository IFreeDiscItemRepository {
            get { return unityContainer.Resolve<IFreeDiscItemRepository>(); }
        }

        static public IFreeDiscItemDownloadRepository IFreeDiscItemDownloadRepository
        {
            get { return unityContainer.Resolve<IFreeDiscItemDownloadRepository>(); }
        }

        static ViewModelLocator()
        {
            unityContainer = new UnityContainer();

            unityContainer.RegisterType<SearchViewModel>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<SettingViewModel>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<DownloadViewModel>(new ContainerControlledLifetimeManager());

            unityContainer.RegisterType<IFreeDiscItemRepository, FreeDiscItemRepository>();
            unityContainer.RegisterType<IFreeDiscItemDownloadRepository, FreeDiscItemDownloadRepository>();

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
        }
    }
}
