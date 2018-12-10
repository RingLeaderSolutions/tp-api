using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Theta.Platform.Order.Management.Service.Data.Entities;

namespace Theta.Platform.Order.Management.Service
{
    public static class DataMappingExtensions
    {
        public static OrderEntity ToOrderEntity(this Order order, string messageId)
        {
            return new OrderEntity(order.Id, messageId)
            {
                CurrencyCode = order.CurrencyCode,
                LimitPrice = order.LimitPrice,
                MarkupUnit = order.MarkupUnit,
                MarkupValue = order.MarkupValue,
                ParentOrderId = order.ParentOrderId,
                Quantity = order.Quantity,
                Status = order.Status,
                Type = order.Type,
                InstrumentId = order.InstrumentId,
                OwnerId = order.OwnerId,
                EntityId = order.EntityId
            };
        }

        public static Order ToOrder(this OrderEntity orderEntity)
        {
            return new Order()
            {
                Id = new Guid(orderEntity.PartitionKey),
                InstrumentId = orderEntity.InstrumentId,
                CurrencyCode = orderEntity.CurrencyCode,
                LimitPrice = orderEntity.LimitPrice,
                MarkupUnit = orderEntity.MarkupUnit,
                MarkupValue = orderEntity.MarkupValue,
                ParentOrderId = orderEntity.ParentOrderId,
                Quantity = orderEntity.Quantity,
                Status = orderEntity.Status,
                Type = orderEntity.Type,
                OwnerId = orderEntity.OwnerId,
                EntityId = orderEntity.EntityId
            };
        }

        public static async Task<IList<T>> ExecuteQueryAsync<T>(this CloudTable table, TableQuery<T> query, CancellationToken ct = default(CancellationToken), Action<IList<T>> onProgress = null) where T : ITableEntity, new()
        {
            var runningQuery = new TableQuery<T>()
            {
                FilterString = query.FilterString,
                SelectColumns = query.SelectColumns
            };

            var items = new List<T>();
            TableContinuationToken token = null;

            do
            {
                runningQuery.TakeCount = query.TakeCount - items.Count;

                TableQuerySegment<T> seg = await table.ExecuteQuerySegmentedAsync<T>(runningQuery, token);
                token = seg.ContinuationToken;
                items.AddRange(seg);
                onProgress?.Invoke(items);

            } while (token != null && !ct.IsCancellationRequested && (query.TakeCount == null || items.Count < query.TakeCount.Value));

            return items;
        }
    }
}
