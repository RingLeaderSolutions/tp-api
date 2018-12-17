using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Theta.Paltform.Order.Read.Service.Framework.Helpers
{
    public static class EventStoreHelpers
    {
        public static object Deserialize(this ResolvedEvent resolvedEvent)
        {
            return JsonConvert
                 .DeserializeObject(Encoding.UTF8.GetString(resolvedEvent.Event.Data), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
        }

        public static byte[] Serialize(this object e)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects }));
        }
    }
}
