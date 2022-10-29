using System;
using MobileApp.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileApp.Views
{
    public partial class AboutPage : ContentPage
    {
        private readonly INotificationRegistrationService _notificationRegistrationService;

        public AboutPage()
        {
            InitializeComponent();

            _notificationRegistrationService = ServiceContainer.Resolve<INotificationRegistrationService>();
        }

        private void RegisterButtonClicked(object sender, EventArgs e)
            => _notificationRegistrationService.RegisterDeviceAsync().ContinueWith((task)
            =>
            {
                ShowAlert(task.IsFaulted ?
                    task.Exception.Message :
                    $"Device registered");
            });

        private void DeregisterButtonClicked(object sender, EventArgs e)
            => _notificationRegistrationService.DeregisterDeviceAsync().ContinueWith((task)
                =>
                {
                    ShowAlert(task.IsFaulted ?
                        task.Exception.Message :
                        $"Device deregistered");
                });

        private void ShowAlert(string message)
            => MainThread.BeginInvokeOnMainThread(()
                => DisplayAlert("PushDemo", message, "OK").ContinueWith((task)
                    =>
                { if (task.IsFaulted) throw task.Exception; }));
    }
}