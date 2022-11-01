using System;
using System.Collections.Generic;
using MobileApp.Models;

namespace MobileApp.Services
{
    public interface IPushDemoNotificationActionService : INotificationActionService
    {
        event EventHandler<(PushAction action, IDictionary<string, string> data)> ActionTriggered;
    }
}