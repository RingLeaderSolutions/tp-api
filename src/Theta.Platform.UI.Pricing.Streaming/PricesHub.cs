using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Theta.Platform.Domain.Instruments;

namespace Theta.Platform.UI.Pricing.Streaming
{
    // TODO: Authentication
    //[Authorize]
    public class PricesHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }


        public async Task Subscribe(string instrumentId)
        {
            var instrumentResult = await $"http://theta.platform.ui.instruments.api/api/Instrument/{instrumentId}"
                .GetAsync();

            if (!instrumentResult.IsSuccessStatusCode)
            {
                Trace.Write($"Unable to subscribe to instrumentId [{instrumentId}], received unsuccessful status code from Instrument API.");
                return;
            }

            var instrumentJson = await instrumentResult.Content.ReadAsStringAsync();
            var instrument = JsonConvert.DeserializeObject<Instrument>(instrumentJson);

            Trace.Write($"Subscribing {Context.ConnectionId} to instrument [{instrument.Id.ToString()} - [{instrument.Category}/{instrument.AssetClass.ToString()}/{instrument.Name}`]");
            await Groups.AddToGroupAsync(Context.ConnectionId, instrument.Id.ToString());
        }


        public async Task Unsubscribe(string instrumentId)
        {
            Trace.Write($"Un-subscribing {Context.ConnectionId} from instrument [{instrumentId}]");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, instrumentId);
        }
    }
}