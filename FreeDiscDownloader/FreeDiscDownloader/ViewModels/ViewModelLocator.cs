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
    public static class ViewModelLocator
    {
        public static SearchViewModel SearchViewModel {
            get { return ServiceLocator.Current.GetInstance<SearchViewModel>(); }
        }

        public static SettingViewModel SettingViewModel {
            get { return ServiceLocator.Current.GetInstance<SettingViewModel>(); }
        }

        public static DownloadViewModel DownloadViewModel {
            get { return ServiceLocator.Current.GetInstance<DownloadViewModel>(); }
        }

        public static IFreeDiscItemRepository IFreeDiscItemRepository {
            get { return ServiceLocator.Current.GetInstance<IFreeDiscItemRepository>(); }
        }

        public static IFreeDiscItemDownloadRepository IFreeDiscItemDownloadRepository
        {
            get { return ServiceLocator.Current.GetInstance<IFreeDiscItemDownloadRepository>(); }
        }

        static ViewModelLocator()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterType<SearchViewModel>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<SettingViewModel>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<DownloadViewModel>(new ContainerControlledLifetimeManager());

            unityContainer.RegisterType<IFreeDiscItemRepository, FreeDiscItemRepository>();
            unityContainer.RegisterType<IFreeDiscItemDownloadRepository, FreeDiscItemDownloadRepositoryHttp>();

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
        }
    }
}
