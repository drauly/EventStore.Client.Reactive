using System.Collections.Generic;

namespace EventStore.Client.Reactive
{
    public struct EventWithPosition<TEvent>
    {
        public TEvent Event { get; }
        public long Position { get; }

        public EventWithPosition(TEvent @event, long position)
        {
            Event = @event;
            Position = position;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EventWithPosition<TEvent>))
            {
                return false;
            }

            var position = (EventWithPosition<TEvent>)obj;
            return EqualityComparer<TEvent>.Default.Equals(Event, position.Event) &&
                   this.Position == position.Position;
        }

        public override int GetHashCode()
        {
            var hashCode = 1043366529;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TEvent>.Default.GetHashCode(Event);
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            return hashCode;
        }
    }
}