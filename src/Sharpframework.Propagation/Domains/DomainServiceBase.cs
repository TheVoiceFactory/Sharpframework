using Sharpframework.Propagation.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpframework.Propagation.Domains
{
    public abstract class DomainServiceBase : UnitOfWorkBase, IDomainService, IServiceBus
    {
        private static Dictionary<string, Func<IDomainEvent, object>> _handlersK = new Dictionary<string, Func<IDomainEvent, object>>();

        public void Publish(IDomainEvent domainEvent)
        {
            //throw new NotImplementedException();
            var handlers = from hand in _handlersK
                           where hand.Key == domainEvent.GetType().Name
                           select hand;

            foreach (var f in handlers)
            {
                (f.Value).Invoke(domainEvent);
            }
        }

        public void Subscribe<domevent>(Func<IDomainEvent, object> eventManager) where domevent : IDomainEvent
        {
            string s = typeof(domevent).Name;

            _handlersK.Add(s, eventManager);
            // throw new NotImplementedException();
        }
    }
}