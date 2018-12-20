using System;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Theta.Platform.Order.Read.Service
{
    public static class Extensions
    {
        public static T ParseJson<T>(this RecordedEvent data)
        {
            if (data == null) throw new ArgumentNullException("data");

            var value = Encoding.UTF8.GetString(data.Data);

            return JsonConvert.DeserializeObject<T>(value);
        }

        public static T ParseJson<T>(this string value)
        {
            if (value == null) throw new ArgumentNullException("value");

            return JsonConvert.DeserializeObject<T>(value);
        }

        public static T ParseJson<T>(this ResolvedEvent data)
        {
            var value = Encoding.UTF8.GetString(data.Event.Data);

            //Console.WriteLine("RAW Event ({0}):{1}", data.OriginalEvent.EventType, value);

            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
