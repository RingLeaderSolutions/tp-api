using EventStore.ClientAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theta.Paltform.Order.Read.Service
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
