using System;
using MobileApp.Models;

namespace MobileApp.Services
{
    public interface IPushDemoNotificationActionService : INotificationActionService
    {
        event EventHandler<(PushAction action, string id)> ActionTriggered;
    }
}