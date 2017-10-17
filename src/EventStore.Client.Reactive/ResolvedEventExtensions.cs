using EventStore.ClientAPI;

namespace EventStore.Client.Reactive
{
    public static class ResolvedEventExtensions
    {
        public static Event ToEvent(this ResolvedEvent resolvedEvent)
        {
            return new Event(
                resolvedEvent.OriginalEventNumber, 
                resolvedEvent.Event.EventType, 
                resolvedEvent.Event.Data);
        }
    }
}