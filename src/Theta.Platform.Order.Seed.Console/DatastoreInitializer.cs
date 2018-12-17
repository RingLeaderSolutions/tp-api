using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Theta.Platform.Order.Seed.Console.Messaging;
using Theta.Platform.Order.Seed.Console.Messaging.Commands;

namespace Theta.Platform.Order.Seed.Console
{
    public class DatastoreInitializer
    {
        private readonly TopicClientProvider _topicClientProvider;

        public DatastoreInitializer(TopicClientProvider topicClientProvider)
        {
            _topicClientProvider = topicClientProvider;
        }

        public async Task Seed()
        {
            await SeedOrders();
        }

        private async Task SeedOrders()
        {
            var instrumentIds = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var markupUnitValues = new string[] { "PercentageOfWhole", "BasisPoints", "ActualValue" };
            var orderTypeValues = new string[] { "Market", "Fill" };
            var currencies = new string[] { "GBP", "USD", "NOK" };

            for (int i = 0; i < 1; i++)
            {
                var entityId = Guid.NewGuid();
                var ownerIds = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

                foreach (var ownerId in ownerIds)
                {
                    for (int x = 0; x < 1; x++)
                    {
                        var instrumentId = instrumentIds.OrderBy(inst => Guid.NewGuid()).First();
                        var markUpUnit = markupUnitValues.OrderBy(mku => Guid.NewGuid()).First();
                        var orderType = orderTypeValues.OrderBy(ot => Guid.NewGuid()).First();
                        var currency = currencies.OrderBy(curr => Guid.NewGuid()).First();

                        var orderId = Guid.NewGuid();

                        var quantity = RandomQuantity();
                        var randomLimitPrice = RandomDecimal(15.71M, 312.78M);
                        var randomMarkupValue = RandomDecimal(2.1M, 12.9M);
                        var randomeExpiration = GetGTDExpiration();

                        var timeInForce = "GoodTillCancel";

                        if (randomeExpiration.HasValue)
                        {
                            if (randomeExpiration.Value.Hour == 0 && randomeExpiration.Value.Month == 0)
                                timeInForce = "GoodTillDay";
                            else
                                timeInForce = "GoodTillDate";
                        }

                        await SeedOrder(
                            instrumentId, currency, quantity, randomLimitPrice,
                            randomMarkupValue, orderId, entityId, ownerId, markUpUnit,
                            orderType, randomeExpiration, timeInForce);
                        //throw new Exception();
                    }
                }
            }
        }

        private bool FillPartial()
        {
            Random gen = new Random();
            int prob = gen.Next(100);

            if (prob <= 25)
            {
                return false;
            }


            return true;
        }

        private DateTime? GetGTDExpiration()
        {
            Random gen = new Random();
            int prob = gen.Next(100);

            if (prob <= 20)
            {
                return null;
            }

            if (prob < 30)
            {
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }

            return DateTime.Now.AddDays(1);
        }

        private async Task SeedOrder(Guid instrumentId, string currency, decimal quantity, decimal randomLimitPrice,
            decimal randomMarkupValue, Guid orderId, Guid entityId, Guid ownerId,
            string markupUnit, string orderType, DateTime? expiration, string timeInForce)
        {
            var createOrderCommand = new CreateOrderCommand()
            {
                CurrencyCode = currency,
                OrderId = orderId,
                InstrumentId = instrumentId,
                LimitPrice = randomLimitPrice,
                MarkupUnit = markupUnit,
                MarkupValue = randomMarkupValue,
                Quantity = quantity,
                Type = orderType,
                OwnerId = ownerId,
                DeskId = entityId,
                GoodTillDate = expiration,
                TimeInForce = timeInForce
            };

            await Post(createOrderCommand);

            await Task.Delay(15);



            var pickupOrderCommand = new PickupOrderCommand() { OrderId = orderId, OwnerId = Guid.NewGuid() };

            await Post(pickupOrderCommand);
            await Task.Delay(15);



            var putdownOrderCommand = new PutDownOrderCommand() { OrderId = orderId };

            await Post(putdownOrderCommand);
            await Task.Delay(15);



            var pickupOrderCommand2 = new PickupOrderCommand() { OrderId = orderId, OwnerId = Guid.NewGuid() };

            await Post(pickupOrderCommand2);
            await Task.Delay(15);

            await Fill(orderId, createOrderCommand);

            var completeOrderCommand = new CompleteOrderCommand() { OrderId = orderId };

            await Post(completeOrderCommand);
        }

        private async Task Fill(Guid orderId, CreateOrderCommand createOrderCommand)
        {
            var price = RandomDecimal(0.1M, 3.25M);

            if (!FillPartial())
            {
                var fillOrderCommand = new FillOrderCommand() { OrderId = orderId, RFQId = Guid.NewGuid(), Price = price, Quantity = createOrderCommand.Quantity };

                await Post(fillOrderCommand);
                await Task.Delay(15);
                return;
            }

            var fillOrderCommand1 = new FillOrderCommand() { OrderId = orderId, RFQId = Guid.NewGuid(), Price = price, Quantity = (createOrderCommand.Quantity / 2) };

            await Post(fillOrderCommand1);
            await Task.Delay(15);

            var fillOrderCommand2 = new FillOrderCommand() { OrderId = orderId, RFQId = Guid.NewGuid(), Price = price, Quantity = createOrderCommand.Quantity - fillOrderCommand1.Quantity };

            await Post(fillOrderCommand2);
            await Task.Delay(15);
        }

        private async Task Post<T>(T command)
        {
            var messageText = JsonConvert.SerializeObject(command);

            var message = new Message(Encoding.UTF8.GetBytes(messageText));

            var client = _topicClientProvider.GetTopicClient(typeof(T));

            await client.SendAsync(message);
        }

        private static decimal RandomQuantity()
        {
            Random r = new Random();

            int quantity = r.Next(50000, 20000000);
            return Math.Round(quantity / 100M, 0) * 100;
        }

        private static decimal RandomDecimal(decimal from, decimal to)
        {
            Random r = new Random();

            return r.NextDecimal(from, to);
        }
    }
}
