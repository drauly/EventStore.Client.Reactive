using EventStore.Client.Reactive;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using System;
using System.Net;

namespace Sample
{
    class Program
    {
        private const string StreamId = "myStreamId";

        static void Main(string[] args)
        {
            var settings = ConnectionSettings.Create()
                .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"))
                .Build();
            var connection = EventStoreConnection.Create(settings, new IPEndPoint(IPAddress.Loopback, 1113));
            connection.ConnectAsync().Wait();

            connection.CreateObservable(StreamId)
                      .DeserializeWithPosition<MyEvent>()
                      .HandleEvent(e => Console.WriteLine("handled : " + e.Property))
                      .Subscribe(pos => Console.WriteLine("last position handled : " + pos));
           
            var serializer = new EventSerializer();

            while (true)
            {
                var key = Console.ReadKey().KeyChar;
                Console.Write("\b");
                var @event = new MyEvent { Property = key.ToString() };
                var eventData = new EventData(Guid.NewGuid(), nameof(MyEvent), true, serializer.Serialize(@event), null);
                connection.AppendToStreamAsync(StreamId, ExpectedVersion.Any, eventData).Wait();
            }
        }

        private struct MyEvent
        {
            public string Property { get; set; }
        }
    }


}
