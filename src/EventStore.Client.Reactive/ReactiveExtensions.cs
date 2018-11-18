using System;
using System.Reactive;
using System.Reactive.Linq;
using EventStore.ClientAPI;
namespace EventStore.Client.Reactive
{
   public static class ReactiveExtensions
   {
        public static IObservable<Event> CreateObservable(
            this IEventStoreConnection connection, 
            string streamId, 
            long? position = null, 
            Action<long> liveProcessingStarted = null)
        {
            return new EventStoreObservable(connection, streamId, position, liveProcessingStarted).RefCount();
        }
   
        public static IObservable<TEvent> Deserialize<TEvent>(
            this IObservable<Event> source,
            Action<Exception> exceptionHandler = null,
            IEventSerializer serializer = null)
        {
            serializer = serializer ?? new EventSerializer();
            exceptionHandler = exceptionHandler ?? (_ => { });
            return
                source.Select(@event =>
                        {
                            if (!@event.EventType.Equals(typeof(TEvent).Name))
                            {
                                return default(TEvent);
                            }
                            try
                            {
                                return serializer.Deserialize<TEvent>(@event);
                            }
                            catch(Exception e)
                            {
                                exceptionHandler(e);
                                return default(TEvent);
                            }
                            
                        })
                       .Where(e => !Equals(e, default(TEvent)));
        }
   
        public static IObservable<EventWithPosition<TEvent>> DeserializeWithPosition<TEvent>(
            this IObservable<Event> source,
            Action<Exception> exceptionHandler = null,
            IEventSerializer serializer = null)
        {
            serializer = serializer ?? new EventSerializer();
            exceptionHandler = exceptionHandler ?? (_ => { });
            return
                source.Select(@event =>
                        {
                            if (!@event.EventType.Equals(typeof(TEvent).Name))
                            {
                                return default(EventWithPosition<TEvent>);
                            }
                            try
                            {
                                return new EventWithPosition<TEvent>(
                                            serializer.Deserialize<TEvent>(@event),
                                            @event.Position);
                            }
                            catch(Exception e)
                            {
                                exceptionHandler(e);
                                return default(EventWithPosition<TEvent>);
                            }
                            
                        })
                     .Where(e => !Equals(e, default(EventWithPosition<TEvent>)));
        }
        
        public static IObservable<long> HandleEvent<TEvent>(
            this IObservable<EventWithPosition<TEvent>> source,
            IObserver<TEvent> observer)
        {
            return source.Select(@event =>
            {
                observer.OnNext(@event.Event);
                return @event.Position;
            });
        }
        
        public static IObservable<long> HandleEvent<TEvent>(
            this IObservable<EventWithPosition<TEvent>> source,
            Action<TEvent> handler)
        {
            return source.HandleEvent(Observer.Create(handler));
        }
    }
}