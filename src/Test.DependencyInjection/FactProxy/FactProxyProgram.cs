using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Sharpframework.Core;
using Sharpframework.EntityModel;
using Sharpframework.EntityModel.Implementation;
using Sharpframework.Propagation.Facts;
using Sharpframework.Roslyn.CSharp;


namespace Test.DependencyInjection.FactProxy
{
    public class FactProxyProgram
    {
        public class ServiceContainer : IServiceProvider
        {
            private Dictionary<Type, Object> __services;

            public ServiceContainer ()
                =>__services = new Dictionary<Type, Object> ();

            public Object GetService ( Type serviceType )
            {
                Object retVal;

                __services.TryGetValue ( serviceType, out retVal );

                return retVal;
            }

            public void AddService ( Type srvType, Object srvInst )
                => __services.TryAdd ( srvType, srvInst );
        }

        public class BusBase
            : IFactPublisher
            , IFactDispatcher
        {
            private Subsctibers __subscribers;


            protected class Subsctibers
                : KeyedCollectionBase<Type, FactSubscribersBase>
            {
                public void Consume<FactType> ( FactType fact )
                    where FactType : IFact
                {
                    if ( TryGetValue ( fact.GetType (), out FactSubscribersBase consumers ) )
                        consumers.Consume ( fact );
                }

                public Boolean Subscribe<FactType> ( IFactConsumer<FactType> factConsumer )
                    where FactType :  IFact
                {
                    if ( TryGetValue ( typeof ( FactType ), out FactSubscribersBase consumers ) )
                        consumers.Subscribe ( factConsumer );
                    else
                        Add ( new FactSubscribers<FactType> ( factConsumer ) );

                    return true;
                }

                public Boolean UnSubscribe<FactType> ( IFactConsumer<FactType> factConsumer )
                    where FactType : IFact
                {
                    if ( TryGetValue ( typeof ( FactType ), out FactSubscribersBase consumers ) )
                        consumers.UnSubscribe ( factConsumer );

                    return true;
                }


                protected override Type GetKeyForItem ( FactSubscribersBase item )
                    => item.TypeOfFact;

            }
            protected abstract class FactSubscribersBase
            {
                public Type TypeOfFact { get => ImplTypeOfFact; }

                public void Consume<FactType> ( FactType fact )
                    where FactType : IFact
                        => ImplConsume ( fact );

                public Boolean Subscribe<FactType> ( IFactConsumer<FactType> factConsumer )
                    where FactType : IFact
                        => ImplSubscribe ( factConsumer );

                public Boolean UnSubscribe<FactType> ( IFactConsumer<FactType> factConsumer )
                    where FactType : IFact
                        => ImplUnSubscribe ( factConsumer );

                protected abstract Type ImplTypeOfFact { get; }

                protected abstract void ImplConsume<FactType> ( FactType fact )
                    where FactType : IFact;

                protected abstract Boolean ImplSubscribe<FactType> ( IFactConsumer<FactType> factConsumer )
                    where FactType : IFact;

                protected abstract Boolean ImplUnSubscribe<FactType> ( IFactConsumer<FactType> factConsumer )
                    where FactType : IFact;
            }

            protected class FactSubscribers<FactType>
                : FactSubscribersBase
                , IFactConsumer<FactType>
            where FactType :  IFact
            {
                private DistinctList<IFactConsumer<FactType>> __factSubscribers;

                public FactSubscribers ( IFactConsumer<FactType> firstSubscriber )
                    => (__factSubscribers = new DistinctList<IFactConsumer<FactType>> ())
                            .Add ( firstSubscriber );


                public void Consume ( FactType fact )
                {
                    if ( fact != null )
                        foreach ( IFactConsumer<FactType> consumer in __factSubscribers )
                            consumer.Consume ( fact );
                }

                public Boolean Subscribe ( IFactConsumer<FactType> factConsumer )
                {
                    if ( factConsumer == null ) return false;

                    __factSubscribers.Add ( factConsumer );

                    return true;
                }

                protected override Type ImplTypeOfFact => typeof ( FactType );

                public Boolean UnSubscribe ( IFactConsumer<FactType> factConsumer )
                    => factConsumer != null && __factSubscribers.Remove ( factConsumer );

                protected override void ImplConsume<FactType1> ( FactType1 fact )
                {
                    if ( typeof ( FactType1 ).IsAssignableFrom ( typeof ( FactType ) ) )
                        Consume ( (FactType) ((IFact) fact) );
                }

                protected override Boolean ImplSubscribe<FactType1> ( IFactConsumer<FactType1> factConsumer )
                    => Subscribe ( factConsumer as IFactConsumer<FactType> );

                protected override Boolean ImplUnSubscribe<FactType1> ( IFactConsumer<FactType1> factConsumer )
                    => UnSubscribe ( factConsumer as IFactConsumer<FactType> );
            }

            public BusBase () => __subscribers = new Subsctibers ();


            public void Consume<FactType> ( FactType fact )
                where FactType : class, IFact
                    => __subscribers.Consume ( fact );
            
            public bool Subscribe ( IFactConsumer factConsumer )
            {
                throw new NotImplementedException ();
            }

            public Boolean Subscribe<FactType> ( IFactConsumer<FactType> factConsumer )
                where FactType : IFact
                    => __subscribers.Subscribe ( factConsumer );

            public bool UnSubscribe ( object factConsumer )
            {
                throw new NotImplementedException ();
            }

            public Boolean UnSubscribe<FactType> ( IFactConsumer<FactType> factConsumer )
                where FactType : IFact
                    => __subscribers.UnSubscribe ( factConsumer );

            public void Publish<FactType> ( FactType fact )
                where FactType : IFact
            {
                ImplDispatch ( fact );
                ImplPublish ( fact );
            }


            // Dispatch to the local registered Consumers
            protected virtual void ImplDispatch<FactType> ( FactType fact )
                where FactType : IFact
                    => __subscribers.Consume ( fact );

            // Publish (send) to remote Consumers (on the network)
            protected virtual void ImplPublish<FactType> ( FactType fact )
            { }
        }
        public class Bus
            : IFactDispatcher
            , IFactPublisher
        {

            private Dictionary<Type, ArrayList> __subscribers;

            public Bus () => __subscribers = new Dictionary<Type, ArrayList> ();

            public void Publish<FactType> ( FactType fact )
                where FactType : IFact
            {
                ArrayList subscriptions;

                if ( !__subscribers.TryGetValue ( typeof ( FactType ), out subscriptions ) )
                    return;

                foreach ( IFactConsumer<FactType> item in subscriptions )
                    item.Consume ( fact );
            }

            public Boolean Subscribe ( IFactConsumer factConsumer )
            {
                throw new NotImplementedException ();
            }

            public Boolean Subscribe<FactType> ( IFactConsumer<FactType> factConsumer )
                where FactType : IFact
            {
                if ( factConsumer == null ) return false;

                ArrayList subscriptions;

                if ( !__subscribers.TryGetValue ( typeof ( FactType ), out subscriptions ) )
                    __subscribers.Add ( typeof ( FactType ), subscriptions = new ArrayList () );

                if ( subscriptions.Contains ( factConsumer ) )
                    return false;
                else
                    subscriptions.Add ( factConsumer );

                return true;
            }

            public Boolean UnSubscribe ( object factConsumer )
            {
                throw new NotImplementedException ();
            }

            public Boolean UnSubscribe<FactType> ( IFactConsumer<FactType> factConsumer )
                where FactType : IFact
            {
                throw new NotImplementedException ();
            }
        }

        static void Main ( string [] args )
        {
            CompilationUnitSyntax _BuildCompileUnit (
                ClassDeclarationSyntax  classDeclStx,
                Type                    implementationType )
            {
                NameSyntax                  nameStx;
                NamespaceDeclarationSyntax  nsDeclStx;

                nameStx = SyntaxHelper.IdentifierName ( implementationType.Namespace, "Proxies" );

                nsDeclStx = SyntaxFactory.NamespaceDeclaration ( nameStx )
                            .WithMembers ( SyntaxHelper.SyntaxList (
                                                (MemberDeclarationSyntax) classDeclStx ) );

                nsDeclStx = nsDeclStx.AddUsings ( SyntaxFactory.UsingDirective (
                                                        SyntaxFactory.ParseName (
                                                            implementationType.Namespace ) ) );

                return SyntaxFactory.CompilationUnit ()
                            .WithMembers ( SyntaxHelper.SyntaxList ( (MemberDeclarationSyntax) nsDeclStx ) )
                            .NormalizeWhitespace ();
            }

            ClassDeclarationSyntax      classDeclStx;
            Assembly                    compiledAssembly;
            CompilationUnitSyntax       cuStx;
            DistinctList<Assembly>      referredAssemblies;
            FactProxyGenerator          roslynProxyGen;

            referredAssemblies  = new DistinctList<Assembly> ();
            roslynProxyGen      = new FactProxyGenerator ();
            classDeclStx        = roslynProxyGen.Generate (
                                        typeof ( ITestFact ),
                                        typeof ( TestFactClass ),
                                        referredAssemblies );
            cuStx               = _BuildCompileUnit ( classDeclStx, typeof ( TestFactClass ) );

            Console.WriteLine ( cuStx.ToFullString () );

            referredAssemblies.Add ( typeof ( Entity ).Assembly );

            compiledAssembly = CompilationHelper.CompileLibrary (
                                    "InMemoryAssembly", referredAssemblies, cuStx.SyntaxTree );

            //Bus                 bus = new Bus ();
            BusBase             bus = new BusBase ();
            ServiceContainer    sc  = new ServiceContainer ();
            ITestFact           testFactObj1, testFactObj2;
            ITestFact           testFactProxy1, testFactProxy2;
            IUid                uid = new UId ();
            

            sc.AddService ( typeof ( IFactDispatcher ), bus );
            sc.AddService ( typeof ( IFactPublisher ), bus );

            testFactObj1 = new TestFactClass ( sc, uid );
            testFactObj2 = new TestFactClass ( sc, uid );

            testFactProxy1 = compiledAssembly.CreateInstance (
                                "Test.DependencyInjection.FactProxy.Proxies.TestFactClassProxy",
                                false,
                                BindingFlags.Public | BindingFlags.Instance,
                                null,
                                new object [] { testFactObj1, sc }, //args
                                null,
                                null ) as ITestFact;

            testFactProxy2 = compiledAssembly.CreateInstance (
                                "Test.DependencyInjection.FactProxy.Proxies.TestFactClassProxy",
                                false,
                                BindingFlags.Public | BindingFlags.Instance,
                                null,
                                new object [] { testFactObj2, sc }, //args
                                null,
                                null ) as ITestFact;

            //SpAllocators.GetSpAllocator<TestFactClass> () ( null );

            testFactProxy1.ChangeName ( "Coccodrillo" );

            return;
        }
    }
}
