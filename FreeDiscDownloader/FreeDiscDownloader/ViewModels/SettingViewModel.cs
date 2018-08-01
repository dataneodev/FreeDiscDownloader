using FreeDiscDownloader.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace FreeDiscDownloader.ViewModels
{
    class SettingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand UpdateButtonCmd { get; private set; }

        private bool updateButtonEnabled = true;

        public bool UpdateButtonEnabled
        {
            get { return updateButtonEnabled; }
            set
            {
                updateButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        public SettingViewModel()
        {
            App.AppSetting.OnPropertyChangeSet( OnPropertyChanged );

            UpdateButtonCmd = new Command(async ()=> 
                {
                    UpdateButtonEnabled = false;
                    UpdateRepository updateService = new UpdateRepository();
                    var checkStatus = await updateService.CheckUpdate();
                    UpdateButtonEnabled = true;
                    if (!checkStatus)
                    {
                        await Application.Current.MainPage.DisplayAlert(App.AppSetting.AppNameVersion,
                                            "Wystąpił nieoczekiwany błąd podczas sprawdzania istniejącej wersji", "OK");
                        return;
                    }

                    if (!updateService.IsNewVersion())
                    {
                        await Application.Current.MainPage.DisplayAlert(App.AppSetting.AppNameVersion,
                                            "Posiadasz już najaktualniejszą wersje aplikacji", "OK");
                        return;
                    }

                    var response = await Application.Current.MainPage.DisplayAlert(App.AppSetting.AppNameVersion,
                          $"Znaleziono nową wersje aplikacji: "+ String.Format("{0:0.#}", updateService.versionServer) +"\nCzy chcesz przejść do strony aktualizacji aplikacji?", 
                          "Otwórz strone", "Anuluj");

                    if(response)
                        try
                        {
                            Device.OpenUri(new Uri(updateService.versionUpdateUrl));
                        }
                        catch (Exception) { }
                });
        }

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
