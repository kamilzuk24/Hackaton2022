using System.Collections.Generic;
using MobileApp.Models;
using MobileApp.Services;
using MobileApp.ViewModels;
using MobileApp.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();

            ServiceContainer.Resolve<IPushDemoNotificationActionService>().ActionTriggered += NotificationActionTriggered;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private void NotificationActionTriggered(object sender, (PushAction action, IDictionary<string, string> data) a) => ShowActionAlert(a);

        private void ShowActionAlert((PushAction action, IDictionary<string, string> data) a)
            => MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(NewTransaction)}?{nameof(NewTransactionViewModel.Id)}={a.action}");
            });
    }
}