using MobileApp.Models;
using MobileApp.Services;
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

        private void NotificationActionTriggered(object sender, PushDemoAction e) => ShowActionAlert(e);

        private void ShowActionAlert(PushDemoAction action)
            => MainThread.BeginInvokeOnMainThread(()
                => MainPage?.DisplayAlert("Nowa wiadomość", $"{action} action received", "OK")
                    .ContinueWith((task) => { if (task.IsFaulted) throw task.Exception; }));
    }
}