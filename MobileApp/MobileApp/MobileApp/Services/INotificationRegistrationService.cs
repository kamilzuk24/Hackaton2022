using System.Threading.Tasks;

namespace MobileApp.Services
{
    public interface INotificationRegistrationService
    {
        Task DeregisterDeviceAsync();

        Task RegisterDeviceAsync(params string[] tags);

        Task RefreshRegistrationAsync();
    }
}