namespace EventStore.Client.Reactive
{
    public interface IEventSerializer
    {
        TEvent Deserialize<TEvent>(Event @event);

        byte[] Serialize(object @event);
    }
}