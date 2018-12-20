using System;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Order.Seed.Console.Commands;

namespace Theta.Platform.Order.Seed.Console
{
    public class DatastoreInitializer
    {
	    private readonly string _queueName;
	    private readonly ICommandQueueClient _commandQueueClient;

        public DatastoreInitializer(string queueName, ICommandQueueClient commandQueueClient)
        {
	        _queueName = queueName;
	        _commandQueueClient = commandQueueClient;
        }

        public async Task Seed()
        {
	        while (true)
	        {
		        var instrumentIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
		        var markupUnitValues = new[] { "PercentageOfWhole", "BasisPoints", "ActualValue" };
		        var orderTypeValues = new[] { "Market", "Limit" };
		        var currencies = new[] { "GBP", "USD", "NOK" };

		        for (int i = 0; i < 1; i++)
		        {
			        var deskId = Guid.NewGuid();
			        var ownerIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

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
					        var randomExpiration = GetGTDExpiration();

					        var timeInForce = "GoodTillCancel";

					        if (randomExpiration.HasValue)
					        {
						        if (randomExpiration.Value.Hour == 0 && randomExpiration.Value.Month == 0)
							        timeInForce = "GoodTillDay";
						        else
							        timeInForce = "GoodTillDate";
					        }

					        await SeedOrder(
						        instrumentId, currency, quantity, randomLimitPrice,
						        randomMarkupValue, orderId, deskId, ownerId, markUpUnit,
						        orderType, randomExpiration, timeInForce);
				        }
			        }
		        }
			}
		}

        private async Task SeedOrder(Guid instrumentId, string currency, decimal quantity, decimal limitPrice,
            decimal markupValue, Guid orderId, Guid deskId, Guid ownerId,
            string markupUnit, string orderType, DateTime? expiration, string timeInForce)
        {
	        var createOrderCommand = new CreateOrderCommand(
		        deskId, orderId, null, instrumentId, ownerId, quantity, orderType, limitPrice, currency,
		        markupUnit, markupValue, expiration, timeInForce);

            await DispatchCommand(createOrderCommand);
			System.Console.WriteLine($"Sent CreateOrderCommand, OrderId=[{orderId}]");
			System.Console.ReadKey();

			var pickupOrderCommand = new PickupOrderCommand() { OrderId = orderId, OwnerId = Guid.NewGuid() };
			await DispatchCommand(pickupOrderCommand);
			System.Console.WriteLine($"Sent PickupOrderCommand (1), OrderId=[{orderId}]");
			System.Console.ReadKey();

			var putdownOrderCommand = new PutDownOrderCommand() { OrderId = orderId };
			await DispatchCommand(putdownOrderCommand);
			System.Console.WriteLine($"Sent PutDownOrderCommand, OrderId=[{orderId}]");
			System.Console.ReadKey();

			var pickupOrderCommand2 = new PickupOrderCommand() { OrderId = orderId, OwnerId = Guid.NewGuid() };
			await DispatchCommand(pickupOrderCommand2);
			System.Console.WriteLine($"Sent PickupOrderCommand (2), OrderId=[{orderId}]");
			System.Console.ReadKey();

			await Fill(orderId, createOrderCommand);

			var completeOrderCommand = new CompleteOrderCommand() { OrderId = orderId };

			await DispatchCommand(completeOrderCommand);
			System.Console.WriteLine($"Sent CompleteOrderCommand, OrderId=[{orderId}]");
			System.Console.ReadKey();
		}

		private async Task Fill(Guid orderId, CreateOrderCommand createOrderCommand)
        {
            var price = RandomDecimal(0.1M, 3.25M);

            if (!FillPartial())
            {
                var fillOrderCommand = new FillOrderCommand() { OrderId = orderId, RFQId = Guid.NewGuid(), Price = price, Quantity = createOrderCommand.Quantity };

                await DispatchCommand(fillOrderCommand);
				System.Console.WriteLine($"Sent FillOrderCommand (0%->100%), OrderId=[{orderId}]");
				System.Console.ReadKey();
				return;
            }

            var fillOrderCommand1 = new FillOrderCommand() { OrderId = orderId, RFQId = Guid.NewGuid(), Price = price, Quantity = (createOrderCommand.Quantity / 2) };
            await DispatchCommand(fillOrderCommand1);
			System.Console.WriteLine($"Sent FillOrderCommand (0%->50%), OrderId=[{orderId}]");
            System.Console.ReadKey();
			
            
            var fillOrderCommand2 = new FillOrderCommand() { OrderId = orderId, RFQId = Guid.NewGuid(), Price = price, Quantity = createOrderCommand.Quantity - fillOrderCommand1.Quantity };

            await DispatchCommand(fillOrderCommand2);
			System.Console.WriteLine($"Sent FillOrderCommand (50%->100%), OrderId=[{orderId}]");
			System.Console.ReadKey();
		}

        private async Task DispatchCommand(ICommand command)
        {
            await _commandQueueClient.Send(_queueName, command);
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

		private static decimal RandomQuantity()
        {
            Random r = new Random();

            int quantity = r.Next(50000, 20000000);
            return Math.Round(quantity / 100M, 0) * 100;
        }

        private static decimal RandomDecimal(decimal from, decimal to)
        {
            Random rnd = new Random();

			byte fromScale = new System.Data.SqlTypes.SqlDecimal(from).Scale;
			byte toScale = new System.Data.SqlTypes.SqlDecimal(to).Scale;

			byte scale = (byte)(fromScale + toScale);
			if (scale > 28)
				scale = 28;

			decimal r = new decimal(rnd.Next(), rnd.Next(), rnd.Next(), false, scale);
			if (Math.Sign(from) == Math.Sign(to) || from == 0 || to == 0)
				return decimal.Remainder(r, to - from) + from;

			bool getFromNegativeRange = (double)from + rnd.NextDouble() * ((double)to - (double)from) < 0;
			return getFromNegativeRange ? decimal.Remainder(r, -from) + from : decimal.Remainder(r, to);
		}
    }
}
