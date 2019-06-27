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

            public bool Subscribe ( IFactConsumer factConsumer )
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

            public bool UnSubscribe ( object factConsumer )
            {
                throw new NotImplementedException ();
            }

            public bool UnSubscribe<FactType> ( IFactConsumer<FactType> factConsumer ) where FactType : IFact
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

            Bus                 bus = new Bus ();
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
