using System;
using System.Reactive.Linq;
using Microsoft.AspNetCore.SignalR;

namespace Theta.Platform.UI.Pricing.Streaming.Services
{
    public class RandomPriceGenerator
    {
        private readonly IHubContext<PricesHub> _hub;

        public RandomPriceGenerator(IHubContext<PricesHub> hub)
        {
            _hub = hub;
            GeneratePrices();
        }


        private void GeneratePrices()
        {
            var random = new Random();

            Observable.Generate(
                0,
                _ => true,
                x => x + 1,
                _ => random.Next(100, 200),
                _ => TimeSpan.FromMilliseconds(random.Next(1, 3) * 1000))
            .Subscribe(x =>
                {
                    _hub.Clients.All.SendAsync("new-price", Convert.ToDecimal(x/100m));
                });
        }
    }
}