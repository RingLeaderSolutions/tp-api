﻿using System;
using System.Reactive.Linq;
using Microsoft.AspNetCore.SignalR;
using Theta.Platform.UI.Pricing.Streaming.Domain.Instruments;

namespace Theta.Platform.UI.Pricing.Streaming.Services
{
    public class RandomPriceGenerator
    {
        private readonly IHubContext<PricesHub> _hub;
        private readonly Random random = new Random();

        public RandomPriceGenerator(IHubContext<PricesHub> hub)
        {
            _hub = hub;
            GeneratePrices();
        }


        private void GeneratePrices()
        {
            Observable.Generate(
                0,
                _ => true,
                x => x + 1,
                _ => random.Next(100, 200),
                _ => TimeSpan.FromMilliseconds(random.Next(1, 2) * 1000))
            .Subscribe(x =>
                {
                    var instrument = GetRandomInstrument();
                    var instrumentId = instrument.Id.ToString();
                    var priceUpdate = new
                    {
                        instrumentId,
                        price = Convert.ToDecimal(x / 100m)
                    };

                    _hub.Clients.Groups(instrumentId).SendAsync("price-updated", priceUpdate);
                });
        }


        private Instrument GetRandomInstrument()
        {
            var all = InstrumentSampleData.All;
            var index = random.Next(0, all.Count);

            return all[index];
        }
    }
}