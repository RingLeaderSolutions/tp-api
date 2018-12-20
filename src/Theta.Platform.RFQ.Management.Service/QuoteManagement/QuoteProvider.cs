using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.RFQ.Management.Service.Domain;
using Theta.Platform.RFQ.Management.Service.Domain.Events;

namespace Theta.Platform.RFQ.Management.Service.QuoteManagement
{
    // Black box - this is the implement 
    public class QuoteProvider : IQuoteProvider
    {
        private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _operations;
        private readonly IAggregateWriter<RequestForQuotes> _aggregateWriter;

        public QuoteProvider(IAggregateWriter<RequestForQuotes> aggregateWriter)
        {
            _operations = new ConcurrentDictionary<Guid, CancellationTokenSource>();
            _aggregateWriter = aggregateWriter;
        }

        public async void SubmitRFQ(Guid rfqIdentifier, Guid orderId, List<string> counterParties, Guid instrument, decimal originalPrice)
        {
            if (_operations.ContainsKey(rfqIdentifier))
            {
                // TODO - not sure if this is a problem, double click from the command i guess
                return;
            }

            var cts = new CancellationTokenSource();

            if (!_operations.TryAdd(rfqIdentifier, cts))
            {
                // TODO - would this ever be hit?
                return;
            }

            try
            {
                await GetQuotes(DateTimeOffset.Now, counterParties, rfqIdentifier, instrument, originalPrice, cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Cancelled RFQ for instrument {rfqIdentifier}");
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed RFQ for instrument {rfqIdentifier}");
            }

            _operations.TryRemove(rfqIdentifier, out CancellationTokenSource source);
        }

        public void CancelRFQ(Guid rfqIdentifier)
        {
            if (_operations.TryGetValue(rfqIdentifier, out CancellationTokenSource cts))
            {
                cts.Cancel();
            }
        }

        async Task GetQuotes(DateTimeOffset validUntil, List<string> counterParties, Guid rFQIdentifier, Guid instrument, decimal originalPrice, CancellationToken ct)
        {
            var downloadTasksQuery = from counterParty in counterParties
                                     select GetQuote(validUntil, counterParty, rFQIdentifier, instrument, originalPrice, ct);

            List<Task<RFQQuote>> downloadTasks = downloadTasksQuery.ToList();

            while (downloadTasks.Count > 0 && !ct.IsCancellationRequested)
            {
                Task<RFQQuote> task = await Task.WhenAny(downloadTasks);

                downloadTasks.Remove(task);

                RFQQuote quote = await task;

                await _aggregateWriter.Save(new RFQQuoteReceivedEvent(instrument, rFQIdentifier, quote.QuoteIdentifier, quote.CounterParty,
                    quote.ValidUntil, quote.Price));
            }
        }

        async Task<RFQQuote> GetQuote(DateTimeOffset validUntil, string counterParty, Guid identifier, Guid instrument, decimal originalPrice, CancellationToken ct)
        {
            Random rnd = new Random();
            var delay = rnd.Next(0, 29000);

            Random rndPrice = new Random();
            var price = rndPrice.NextDecimal(originalPrice - 2, originalPrice + 2);

            await Task.Delay(delay);

            return new RFQQuote(identifier, counterParty, validUntil, price);
        }
    }
}
