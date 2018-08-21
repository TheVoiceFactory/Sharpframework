using System.Collections.Generic;

namespace Sharpframework.Propagation
{
    public abstract class Event<EventType, SourceType, ContextType> :
         TopicMessage<EventType, ContextType>,
         IEvent<EventType, SourceType, ContextType>
         where EventType : IEvent
    {
        public SourceType Source { get; set; }

        public object SourceObject { get; set; }

        protected override Dictionary<string, string> HeaderProperties
        {
            get
            {
                var prop = base.HeaderProperties;
                //here properties to be added

                return prop;
            }
        }
    }

    public abstract class Event<EventType, ContextType> : TopicMessage<EventType, ContextType>, IEvent<EventType, ContextType> where EventType : IEvent
    {
        public object SourceObject { get; set; }

        protected override Dictionary<string, string> HeaderProperties
        {
            get
            {
                var prop = base.HeaderProperties;
                //here properties to be added

                return prop;
            }
        }
    }
}