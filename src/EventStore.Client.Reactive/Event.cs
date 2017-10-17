using System.Collections.Generic;

namespace EventStore.Client.Reactive
{
    public struct Event
    {
        public Event(long position, string eventType, byte[] data)
        {
            Data = data;
            Position = position;
            EventType = eventType;
        }
        public byte[] Data { get; }
        public long Position { get; }
        public string EventType { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Event))
            {
                return false;
            }

            var @event = (Event)obj;
            return EqualityComparer<byte[]>.Default.Equals(Data, @event.Data) &&
                   Position == @event.Position &&
                   EventType == @event.EventType;
        }

        public override int GetHashCode()
        {
            var hashCode = 956585398;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(Data);
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EventType);
            return hashCode;
        }
    }
}