using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Platform.Order.Management.Service.Domain.Commands
{
    public class CreateOrderCommand
    {
        public CreateOrderCommand(
                    Guid deskId,
                    Guid orderId,
                    Guid? parentOrderId,
                    Guid instrumentId,
                    Guid ownerId,
                    decimal quantity,
                    OrderType type,
                    decimal limitPrice,
                   string currencyCode,
                   MarkupUnit markupUnit,
                   decimal markupValue)
        {
            DeskId = deskId;
            OrderId = orderId;
            ParentOrderId = parentOrderId;
            InstrumentId = instrumentId;
            OwnerId = ownerId;
            Quantity = quantity;
            Type = type;
            LimitPrice = limitPrice;
            CurrencyCode = currencyCode;
            MarkupUnit = markupUnit;
            MarkupValue = markupValue;
        }

        public Guid DeskId { get; set; }

        public Guid? ParentOrderId { get; set; }

        public Guid InstrumentId { get; set; }

        public Guid OwnerId { get; set; }

        public Guid OrderId { get; set; }

        public decimal Quantity { get; set; }

        public OrderType Type { get; set; }

        public decimal LimitPrice { get; set; }

        public string CurrencyCode { get; set; }

        public MarkupUnit MarkupUnit { get; set; }

        public decimal MarkupValue { get; set; }
    }
}
