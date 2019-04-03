
using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sharpframework.Propagation.Facts
{
    using Factes.Customers;
    using Factes.Invoices;
    public interface IFact { }
    public class Fact : IFact
    {
    }

    public class Dropped:Fact
        {}
    public  class Facts { }

    namespace Factes.Customers
    {
        public static class FactsExtA
        {
            public class DroppedAFact : Fact { }
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
            public class DroppedBFact : Fact { }
            public static DroppedBFact DroppedB(this Facts f)
            {
                return new DroppedBFact();
            }
        }
    }
    
    public interface IFactProvider<FactType> where FactType : Fact { }
    public interface IFactSubscriber { void Subscribe(Fact fact); }

    public interface IService { }

    namespace Events
    {
        public static class IFactConsumerExt
        {
            public static void AddBus(this IFactConsumer subs,      object bus) { }

        }

    }
    public class FactBus : IFactSubscriber, IService
    {
        private object bus;
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

    public interface IPlayer
    {
        IReadOnlyList<IFact> EmittedFacts { get; }
        IReadOnlyList<IFact> ReactTo{ get; }
    }
    public class Entita:IFactConsumer, IPlayer 
    {
        private FactBus bus;
    
        public Entita(FactBus bus)
        {
            this.bus = bus;
        }

        public IReadOnlyList<IFact> EmittedFacts => throw new NotImplementedException();

        public IReadOnlyList<IFact> ReactTo => throw new NotImplementedException();

        public void Method1()
        {
            Facts f = new Facts();
            Fact f1 = f.Dropped();
            bus.Subscribe(f1);

        }
        
    }

    public class Fact1 : Fact { }
    public class Fact2 : Fact { }


    public class Service
        : IFactProvider<Fact1>
        , IFactProvider<Fact2>
    { }

}