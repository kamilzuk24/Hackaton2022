using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Firebase.Iid;
using MobileApp.Droid.Services;
using MobileApp.Services;

namespace MobileApp.Droid
{
    [Activity(Label = "MobileApp", LaunchMode = LaunchMode.SingleTop, Icon = "@mipmap/icon",
        Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation
        | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, Android.Gms.Tasks.IOnSuccessListener
    {
        private IPushDemoNotificationActionService _notificationActionService;
        private IDeviceInstallationService _deviceInstallationService;

        private IPushDemoNotificationActionService NotificationActionService
            => _notificationActionService ??
                (_notificationActionService =
                ServiceContainer.Resolve<IPushDemoNotificationActionService>());

        private IDeviceInstallationService DeviceInstallationService
            => _deviceInstallationService ??
                (_deviceInstallationService =
                ServiceContainer.Resolve<IDeviceInstallationService>());

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Bootstrap.Begin(() => new DeviceInstallationService());

            if (DeviceInstallationService.NotificationsSupported)
            {

                FirebaseInstanceId.GetInstance(Firebase.FirebaseApp.Instance)
                .GetInstanceId()
                .AddOnSuccessListener(this);
            }

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            ProcessNotificationActions(Intent);
        }

        public void OnSuccess(Java.Lang.Object result)
       => DeviceInstallationService.Token =
           result.Class.GetMethod("getToken").Invoke(result).ToString();

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void ProcessNotificationActions(Intent intent)
        {
            try
            {
                if (intent?.HasExtra("action") == true)
                {
                    var action = intent.GetStringExtra("action");

                    if (!string.IsNullOrEmpty(action))
                        NotificationActionService.TriggerAction(action);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            ProcessNotificationActions(intent);
        }
    }
}