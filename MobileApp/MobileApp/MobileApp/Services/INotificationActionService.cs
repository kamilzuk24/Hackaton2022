using System.Collections.Generic;

namespace MobileApp.Services
{
    public interface INotificationActionService
    {
        void TriggerAction(IDictionary<string, string> data);
    }
}