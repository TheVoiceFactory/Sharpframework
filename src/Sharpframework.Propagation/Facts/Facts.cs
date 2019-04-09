
using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sharpframework.Propagation.Facts
{
    using Factes.Customers;
    using Factes.Invoices;

    public abstract class UniversalTimeStamp
    {
        public abstract UniversalTimeStamp Generate();
    }

    public interface IFact {
        Fact ReactionTo { get; }
        UniversalTimeStamp When { get; }


    }

    public abstract class Fact : IFact
    {
        public abstract Fact ReactionTo { get; }

        public abstract UniversalTimeStamp When { get; }

       

    }

    public class Dropped : Fact
    {
        public override Fact ReactionTo => throw new NotImplementedException();

        public override UniversalTimeStamp When => throw new NotImplementedException();
    }
    public  class Facts { }

    namespace Factes.Customers
    {
        public static class FactsExtA
        {
            public class DroppedAFact : Fact
            {
                public override Fact ReactionTo => throw new NotImplementedException();

                public override UniversalTimeStamp When => throw new NotImplementedException();
            }
            public static DroppedAFact Dropped(this Facts f)
            {
                return new DroppedAFact();
            }
        }
    }
    namespace Factes.Invoices
    {
        public static class FactsExtB
        {
            public class DroppedBFact : Fact
            {
                public override Fact ReactionTo => throw new NotImplementedException();

                public override UniversalTimeStamp When => throw new NotImplementedException();
            }
            public static DroppedBFact DroppedB(this Facts f)
            {
                return new DroppedBFact();
            }
        }
    }
    
    public interface IFactProvider<FactType> where FactType : Fact { }
    public interface IFactSubscriber { void Subscribe(Fact fact); }

    namespace Events
    {
        public static class IFactConsumerExt
        {
            public static void AddBus(this IFactConsumer subs,      object bus) { }

        }

    }
    public class FactBus : IFactSubscriber//, IService
    {
        private object bus;

        public IReadOnlyList<IFact> EmittedFacts => throw new NotImplementedException();

        public IReadOnlyList<IFact> ReactTo => throw new NotImplementedException();

        public void Subscribe(Fact fact)
        {

            Facts f = new Facts();
            Fact f1 = f.Dropped();
            Fact f2 = f.DroppedB();

            throw new System.NotImplementedException();
        }
        public FactBus(object bus)
        {
            this.bus = bus;
        }
    }
    public interface IFactConsumer { }

    public interface IPlayGround
    {
        IReadOnlyList<IFactPublisher> Transports { get; }
    }
    public interface IPlayer
    {
        IReadOnlyList<IFact> AccomplishedFacts { get; } //can be void
        /// </summary>
        IReadOnlyList<IFact> ReactTo{ get; }
        IPlayGround Playground { get; }
    }

    public interface IFactPublisher : IPublisher<IFact> { }

    public interface IPublisher<PublishedType>
    {
        void ImplPublish(PublishedType fact);
    }

    public interface ITransport
    {
        void Publish(IFact fact);
    }

    public abstract class PlayerBase : IPlayer
    {
        public abstract IReadOnlyList<IFact> ReactTo { get; }
       
        public IPlayGround Playground => throw new NotImplementedException();

        public IReadOnlyList<IFact> AccomplishedFacts => throw new NotImplementedException();
    }

    public class BusPlayer : IPlayGround
    {
        public IReadOnlyList<IFactPublisher> Transports => throw new NotImplementedException();
    }

    public class MSMQPlayer : IPlayGround
    {
        

        public  IReadOnlyList<IFactPublisher> Transports => throw new NotImplementedException();
    }

    public interface IEntityFactory { }
    public class EntityFactory:IEntityFactory 
    {
        static IEntity Build() { return null; }
    }

    public static class StandardTransportExt
    {
         static void AddStandardTransport(this IEntityFactory factory) { }
    }
    //public class EnttityFactory:IPlyerFactory
    //{


    //}
        
    //public interface IPlyerFactory
    //{

    //}
    public interface IEntityService : IService
    {  IEnumerable<IEntity> Entities { get; } /* Placeholder: potrebbe non essere esposta */ }

    //public interface IService { }

    public interface IService : IPlayer { }

    public interface IEntity:IFactConsumer, IPlayer 
    { }
    public interface IDocument : IEntity { }

    public class Entity:IEntity
    {
        private FactBus bus;
    
        public Entity (FactBus bus)
        {
            this.bus = bus;
        }

        public IReadOnlyList<IFact> AccomplishedFacts => throw new NotImplementedException();

        public IReadOnlyList<IFact> ReactTo => throw new NotImplementedException();

        public IReadOnlyList<IFactPublisher> Transports => throw new NotImplementedException();

        public IPlayGround Playground => throw new NotImplementedException();

        

        public void Method1()
        {
            Facts f = new Facts();
            Fact f1 = f.Dropped();
            bus.Subscribe(f1);

        }
        
    }

    public class Fact1 : Fact
    {
        public override Fact ReactionTo => throw new NotImplementedException();

        public override UniversalTimeStamp When => throw new NotImplementedException();
    }
    public class Fact2 : Fact
    {
        public override Fact ReactionTo => throw new NotImplementedException();

        public override UniversalTimeStamp When => throw new NotImplementedException();
    }


    public class Service
        : IFactProvider<Fact1>
        , IFactProvider<Fact2>
    { }

}