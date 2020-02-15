using System;
using System.Reflection;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Sharpframework.Core;
using Sharpframework.Roslyn.CSharp;
using Sharpframework.Roslyn.DynamicProxy;


namespace Test.DependencyInjection.DynamicProxy
{
    [ EpilogInterceptor ( typeof ( PrologEpilog ), nameof (PrologEpilog.Epilog )) ]
    [ PrologInterceptor ( typeof ( PrologEpilog ), nameof (PrologEpilog.Prolog )) ]
    public interface IPippo
    {
        void Paperino ( Int32 num, String str );

        Int32 Pluto ( String str );

        String Description { get; set; }
    }

    public class Pippo : IPippo
    {
        public Pippo () { }
        public Pippo ( IServiceProvider sp ) { }
        public String Description { get; set; }

        public void Paperino ( Int32 num, String str )
        {
            Console.WriteLine ( "Called method Pippo.Paperino (...)" );
        }

        public int Pluto ( string str )
        {
            return str == null ? 0 : str.Length;
        }
    }


    public class Program
    {
        private static void SingleClassProxy ( Type contractType, Type implementationType )
        {
            CompilationUnitSyntax _BuildCompileUnit ( ClassDeclarationSyntax classDeclStx )
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

            IPippo _GetProxy ( Assembly assembly )
                => assembly.CreateInstance ( assembly.ExportedTypes.First ().FullName ) as IPippo;


            ClassDeclarationSyntax      classDeclStx;
            Assembly                    compiledAssembly;
            CompilationUnitSyntax       cuStx;
            IPippo                      proxy;
            DistinctList<Assembly>      referredAssemblies;
            PrologEpilogProxyGenerator  roslynProxyGen;

            referredAssemblies  = new DistinctList<Assembly> ();
            roslynProxyGen      = new PrologEpilogProxyGenerator ();

            classDeclStx = roslynProxyGen.Generate (
                                contractType, implementationType, referredAssemblies );

            cuStx = _BuildCompileUnit ( classDeclStx );

            Console.WriteLine ( cuStx.ToFullString () );

            //compiledAssembly = CompilationHelper.CompileLibrary (
            //                        "InMemoryAssembly", referredAssemblies, cuStx.SyntaxTree );
            CompilationHelper2 ch = new CompilationHelper2 ();

            if ( ch.CompileLibrary ( "InMemoryAssembly", referredAssemblies, cuStx.SyntaxTree ) )
                if ( ch.EmitLibrary () )
                    compiledAssembly = ch.LoadLibrary ();
                else
                    return;
            else
                return;

            /*compiledAssembly = ch.MakeLibrary (
                                    "InMemoryAssembly",
                                    referredAssemblies,
                                    cuStx.SyntaxTree ); *//*,
                                    new CSharpCompilationOptions ( OutputKind.DynamicallyLinkedLibrary )
                                            .WithOptimizationLevel ( OptimizationLevel.Release )
                                            .WithOverflowChecks ( false ) );*/

            proxy = _GetProxy ( compiledAssembly );

            Int32 yy = proxy.Pluto ( "pappa" );
            proxy.Paperino ( 14, "ciccia" );
        }

        static void Main ( string [] args )
        {
            SingleClassProxy ( typeof ( IPippo ), typeof ( Pippo ) );

            //ProxyServiceContainer psc = new ProxyServiceContainer ();

            //psc.AddService<IPippo, Pippo> ();

            //psc.GetService ( typeof ( IPippo ) );

            Console.ReadKey ();

            return;
        }
    }
}
