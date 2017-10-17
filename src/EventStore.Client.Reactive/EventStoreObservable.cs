using EventStore.ClientAPI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace EventStore.Client.Reactive
{
    internal class EventStoreObservable : IConnectableObservable<Event>
    {
        private readonly IEventStoreConnection connection;
        private readonly ISubject<Event> internalStream;
        private EventStoreCatchUpSubscription eventStoreSubscription;
        private readonly string streamId;
        private long position;

        public EventStoreObservable(IEventStoreConnection connection, string streamId, long? position)
        {
            this.connection = connection;
            this.streamId = streamId;
            this.position = position ?? GetLatestStreamPosition(connection, streamId);
            this.internalStream = new Subject<Event>();
        }

        private static long GetLatestStreamPosition(IEventStoreConnection connection, string streamId)
        {
            var position = connection
                                .ReadStreamEventsBackwardAsync(streamId, StreamPosition.End, 1, true)
                                .Result?.LastEventNumber ?? 0;
            return position;
        }
        public IDisposable Subscribe(IObserver<Event> observer)
        {
            return internalStream.Subscribe(observer);
        }
        public IDisposable Connect()
        {
            SubscribeToStream();
            return Disposable.Create(() =>
            {
                eventStoreSubscription.Stop();
            });
        }
        private void SubscribeToStream()
        {
            eventStoreSubscription = connection.SubscribeToStreamFrom(
                streamId,
                position,
                CatchUpSubscriptionSettings.Default,
                EventAppeared,
                subscriptionDropped: SubscriptionDropped);
        }
        private void SubscriptionDropped(EventStoreCatchUpSubscription esSubscription, SubscriptionDropReason dropReason, Exception error)
        {
            Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
            {
                SubscribeToStream();
            });
        }
        private void EventAppeared(EventStoreCatchUpSubscription esSubscription, ResolvedEvent @event)
        {
            position = @event.OriginalEventNumber;
            internalStream.OnNext(@event.ToEvent());
        }
    }
}