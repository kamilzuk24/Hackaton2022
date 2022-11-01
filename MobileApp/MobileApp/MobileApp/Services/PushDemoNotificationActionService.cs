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

        public event EventHandler<(PushAction action, string id)> ActionTriggered = delegate { };

        public void TriggerAction(IDictionary<string, string> data)
        {
            if (!data.TryGetValue("action", out var action))
            {
                return;
            }

            if (!_actionMappings.TryGetValue(action, out var pushAction))
            {
                return;
            }

            string additionalData = "";
            switch (pushAction)
            {
                case PushAction.NewTransaction:
                    if (data.TryGetValue("id", out var id))
                    {
                        additionalData = id;
                    }
                    break;

                default:
                    break;
            }

            List<Exception> exceptions = new List<Exception>();

            foreach (var handler in ActionTriggered?.GetInvocationList())
            {
                try
                {
                    handler.DynamicInvoke(this, (pushAction, additionalData));
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