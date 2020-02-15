using Sharpframework.Core;
using Sharpframework.EntityModel;
using System;
using System.Collections.Generic;

namespace Sharpframework.Propagation.Facts
{
    public abstract class UniversalTimeStamp
    {
        public abstract UniversalTimeStamp Generate();
    }

    public interface IFactInstance
        : IFact
        , IUid
    { // -> IFactInstance
        UniversalTimeStamp HappenedAt { get; }
    }

    public interface IFact
    {
        IFact ReactionTo { get; }
    }

    public class Facts { }

    namespace Events
    {
        public static class IFactConsumerExt
        {
            public static void AddBus(this IFactConsumer subs, object bus)
            {
            }
        }
    }

    public interface IFactConsumer { }

    public interface IPlayGround
    {
        IReadOnlyList<ITransport> Transports { get; }
    }

    public interface IStatusID : IPrimitiveTypeValue<string> { }

    public interface IPlayerFiniteStatus
    {
        IStatusID Current { get; }

        IStatusID From { get; }
    }

    public interface IPlayerFiniteStatusMachine
    {
        IFact OnReactionTo { get; }
    }
    /// <summary>
    /// Who can play the reactive game in the World
    /// </summary>
    public interface IPlayer//can be stateless
    {

        IReadOnlyList<IFact> AccomplishedFacts { get; } //can be void

        /// </summary>
        IReadOnlyList<IFactSequence > ReactTo { get; }

        void Notify(IFact fact); //se dianmico occorre un observable per notificare il Playground
        IPlayGround Playground { get; } //use Notify
    }
    public interface IReaction { }
    public interface IFactSequence:IEnumerable<IFact > { }
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
        /// <summary>
        /// It should keep memory of the Facts belonging to the Sequence , once it is complete the Reaction is triggered
        /// </summary>
        IReadOnlyList<IFactSequence> IPlayer.ReactTo => throw new NotImplementedException();

        public void Notify(IFact fact)
        {
            throw new NotImplementedException();
        }
    }

    public interface IEntityFactory { }

    public class EntityFactory : IEntityFactory
    {
        public static IEntity Build()
        { return null; }
    }

    public static class StandardTransportExt
    {
        public static void AddStandardTransport(this IEntityFactory factory)
        { }
    }

    //public class EnttityFactory:IPlyerFactory
    //{
    //}

    //public interface IPlyerFactory
    //{
    //}
    public interface IEntityService : IService
    { IEnumerable<IEntity> Entities { get; } /* Placeholder: potrebbe non essere esposta */ }

    //public interface IService { }

    public interface IService : IPlayer { }

    public interface IFactEntity : IFactConsumer, IPlayer
    { }

    public interface IDocument : IFactEntity { }

    public class Entity : IFactEntity
    {
        public Entity()
        {
        }

        public IReadOnlyList<IFact> AccomplishedFacts => throw new NotImplementedException();

        public IReadOnlyList<IFact> ReactTo => throw new NotImplementedException();

        public IReadOnlyList<IFactPublisher> Transports => throw new NotImplementedException();

        public IPlayGround Playground => throw new NotImplementedException();

        IReadOnlyList<IFactSequence> IPlayer.ReactTo => throw new NotImplementedException();

        public void Method1()
        {
            Facts f = new Facts();
        }

        public void Notify(IFact fact)
        {
            throw new NotImplementedException();
        }
    }

    // di qui codice bidone di esempio

    public interface IPersonaContract
    {
        string Nome { get; }
    }
    public interface IPersona : IPersonaContract
    {
        void ChangeName(string newName);
    }

    public interface IUniverse
    {
         void MakeItKnown(IFact appenaSuccesso);
    }
    public class PersonaFactory
    {
        public PersonaFactory (IUniverse universe) { }
        public IPersona GetNewPersona() { return new Persona(); }
    }
    public class Persona : IPersona, IFactEntity
    {
        public string Nome => throw new NotImplementedException();

        public IReadOnlyList<IFact> AccomplishedFacts => throw new NotImplementedException();

        public IReadOnlyList<IFactSequence> ReactTo => throw new NotImplementedException();

        public IPlayGround Playground => throw new NotImplementedException();

        public void ChangeName(string newName)
        {
            throw new NotImplementedException();
        }

        public void Notify(IFact fact)
        {
            throw new NotImplementedException();
        }
    }

    public class C1
    {
        private string _nome;
        public string Nome { get => _nome; }

        public void Esegui(string param, int num, ICoccodrillo cocco) { _nome = param; }
    }

    public interface ICoccodrillo
    {
        Single LunghezzaCoda { get; }
        Byte NumeroDenti { get; }
        Single Peso { get; }
    }
    public interface Ilogger { void Log(string logm); }
    public class C1Proxy
    {
        private C1 _C1;
        private Ilogger logger;
        public C1Proxy(C1 classe, Ilogger logger ) { _C1 = classe; }
       // public void Esegui(string param, int num, ICoccodrillo cocco) { _C1.Esegui(param, num); logger.Log(param); }
    }

    public class FactoryCore<ObjectType, InterfaceType>
    {
        private ObjectType __coreObject;

    }
}