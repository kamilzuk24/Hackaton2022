using System;
using System.Collections.Generic;
using System.Linq;
using MobileApp.Models;

namespace MobileApp.Services
{
    public class PushDemoNotificationActionService : IPushDemoNotificationActionService
    {
        private readonly Dictionary<string, PushAction> _actionMappings = new Dictionary<string, PushAction>
        {
            { "NewTransaction", PushAction.NewTransaction },
            { "RememberToPay", PushAction.RememberToPay }
        };

        public event EventHandler<(PushAction action, IDictionary<string, string> data)> ActionTriggered = delegate { };

        public void TriggerAction(IDictionary<string, string> data)
        {
            if (!data.TryGetValue("action", out var messageAction))
            {
                return;
            }

            if (!_actionMappings.TryGetValue(messageAction, out var pushAction))
            {
                return;
            }

            List<Exception> exceptions = new List<Exception>();

            foreach (var handler in ActionTriggered?.GetInvocationList())
            {
                try
                {
                    handler.DynamicInvoke(this, (pushAction, data));
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
                throw new AggregateException(exceptions);
        }
    }
}