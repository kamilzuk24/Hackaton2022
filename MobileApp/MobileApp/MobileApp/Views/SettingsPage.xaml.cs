using MobileApp.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private readonly INotificationRegistrationService _notificationRegistrationService;

        public SettingsPage()
        {
            InitializeComponent();

            _notificationRegistrationService = ServiceContainer.Resolve<INotificationRegistrationService>();
        }

        private void notificationsOnOff_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                _notificationRegistrationService.RegisterDeviceAsync().ContinueWith((task) =>
                {
                    ShowAlert(task.IsFaulted ?
                        task.Exception.Message :
                        $"Powiadomienia włączone");
                });
            }
            else
            {
                _notificationRegistrationService.DeregisterDeviceAsync().ContinueWith((task)
                =>
                {
                    ShowAlert(task.IsFaulted ?
                        task.Exception.Message :
                        $"Powiadomienia wyłączone");
                });
            }
        }

        private void ShowAlert(string message)
            => MainThread.BeginInvokeOnMainThread(()
                => DisplayAlert("Informacja", message, "OK").ContinueWith((task)
                    =>
                { if (task.IsFaulted) throw task.Exception; }));
    }
}