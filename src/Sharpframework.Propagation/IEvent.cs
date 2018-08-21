using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpframework.Propagation
{
    public interface IEvent { object SourceObject { get; } }
    public interface IEvent<EventType,SourceType, ContextType> : ITopicMessage<EventType, ContextType>, ITopicMessageContract, IEvent  where EventType : IEvent
    {
        SourceType Source { get; }
    }
    public interface IEvent<EventType, ContextType> : ITopicMessage<EventType, ContextType>, ITopicMessageContract, IEvent where EventType : IEvent
    {

    }
    public interface IEvent<EventType> : ITopicMessage<EventType, object>, ITopicMessageContract, IEvent where EventType : IEvent
    {

    }

    public interface ISystemEvent<EventType, ContextType> : ITopicMessage<EventType, ContextType>, ITopicMessageContract
    {
        int Priority { get; }
    }
    //public interface IDomainEvent<EventType, ContextType, DomainSourceType> : ISkilMessage<EventType, ContextType>, ISkilM where DomainSourceType :
    //{
    //    int Priority { get; }
    //}

}