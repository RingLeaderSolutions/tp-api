using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Microsoft.AspNetCore.SignalR;
using Theta.Platform.UI.Pricing.Streaming.Hubs;

namespace Theta.Platform.UI.Pricing.Streaming.Services
{
    public class RandomNotificationGenerator
    {
        private IHubContext<NotificationsHub> _hub;


        public RandomNotificationGenerator(IHubContext<NotificationsHub> hub)
        {
            _hub = hub;
        }

        public void GenerateNotifications()
        {
            Observable.Generate(
                    0,
                    _ => true,
                    x => x + 1,
                    x => x,
                    _ => TimeSpan.FromMilliseconds(10000))
                .Subscribe(x =>
                {
                    var index = x % Notifications.Count;
                    var notification = Notifications[index];

                    _hub.Clients.All.SendAsync("notification", notification);
                });
        }


        private List<OrderNotification> Notifications = new List<OrderNotification>
        {
            new OrderNotification
            {
                Type = "PICKED_UP",
                OrderId = "0123456711",
                Event = "Order Picked-up",
                Time = "28/09/2018",
                Operation = "SELL 1M EURUSD, LIMIT @ 1.17124",
                ReceivedAt = "11:16:19.547121 GMT"
            },
            new OrderNotification
            {
                Type = "AMENDED",
                OrderId = "0123456789",
                Event = "Order Amended",
                Time = "3/08/2018",
                Operation = "Buy, 12000, VODAFONE GROUP, PLC 5.90%",
                ReceivedAt = "11:21:44.133427 GMT",
                AuditInfo = "NTS 26/11/32, Limit, FOK, Citi"
            },
            new OrderNotification
            {
                Type = "CANCELLED",
                OrderId = "0123456789",
                Event = "Order Cancelled",
                Time = "3/08/2018",
                Operation = "Buy, 12000, VODAFONE GROUP, PLC 5.90%",
                ReceivedAt = "11:26:34.421556 GMT",
                AuditInfo = "NTS 26/11/32, Limit, FOK, Citi"
            }
        };


        private class OrderNotification
        {
            public string Type { get; set; }

            public string OrderId { get; set; }

            public string Event { get; set; }

            public string Time { get; set; }

            public string Operation { get; set; }

            public string ReceivedAt { get; set; }

            public string AuditInfo { get; set; }
        }
    }
}