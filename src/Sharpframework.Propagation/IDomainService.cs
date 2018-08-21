using Sharpframework.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpframework.Propagation.Domains
{
    public interface IDomain
    {
        string DomainID { get; }
        string DomainName { get; }
    }

    public interface IUnitOfWork
    {
        void Start();
        void Commit();
        void Rollback();
    }
    public interface  IDomainService
    {

    }
    public interface IApplicationService
    {

    }

    public interface IServiceBus
    {
        void Publish(IDomainEvent domainEvent);
        void Subscribe<domevent>(Func<IDomainEvent ,object> eventManager) where domevent : IDomainEvent;


    }
    public interface IChangeTracker : IEnumerable<IChange> 
    {
        void AddChange(IChange change);
        IEnumerable<IChange > SuspendedChanges { get; }
     
        void CommitAll();



    }
    public interface IChangeTracked
    { IChangeTracker Changes { get; } }

    public interface IChange:IDomainEvent 
    {
        IEntity Source { get; }
    }

    public interface IDomainEvent
    {
       
         string EventID { get; }
        string TransactionID { get; }
        int SequencePosition { get; }
    }

    public interface IIntegrationEvent:IDomainEvent 
    { }
}
