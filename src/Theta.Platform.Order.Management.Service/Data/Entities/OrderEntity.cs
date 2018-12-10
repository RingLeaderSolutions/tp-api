using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Platform.Order.Management.Service.Data.Entities
{
    public class OrderEntity : CustomTableEntity
    {
        public OrderEntity(Guid orderId, string messageId)
        {
            this.PartitionKey = orderId.ToString();
            this.RowKey = messageId;
        }

        public OrderEntity() { }

        public Guid? ParentOrderId { get; set; }

        public Guid InstrumentId { get; set; }

        public Guid OwnerId { get; set; }

        public Guid EntityId { get; set; }

        public OrderStatus Status { get; set; }

        public decimal Quantity { get; set; }

        public OrderType Type { get; set; }

        public decimal LimitPrice { get; set; }

        public string CurrencyCode { get; set; }

        public MarkupUnit MarkupUnit { get; set; }

        public decimal MarkupValue { get; set; }
    }
}
