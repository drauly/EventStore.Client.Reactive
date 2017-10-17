using Newtonsoft.Json;
using System.Text;

namespace EventStore.Client.Reactive
{
    public class EventSerializer : IEventSerializer
    {
        public TEvent Deserialize<TEvent>(Event @event)
        {
            return JsonConvert.DeserializeObject<TEvent>(Encoding.UTF8.GetString(@event.Data));
        }

        public byte[] Serialize(object @event)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));
        }
    }
}