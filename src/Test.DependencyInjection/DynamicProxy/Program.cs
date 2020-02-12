using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sharpframework.Core;
using Sharpframework.Roslyn.CSharp;
using Sharpframework.Roslyn.DynamicProxy;
using System;
using System.Linq;
using System.Reflection;

namespace Test.DependencyInjection.DynamicProxy
{
    [EpilogInterceptor(typeof(PrologEpilog), nameof(PrologEpilog.Epilog))]
    [PrologInterceptor(typeof(PrologEpilog), nameof(PrologEpilog.Prolog))]
    public interface IPippo
    {
        void Paperino(Int32 num, String str);

        Int32 Pluto(String str);

        String Description { get; set; }
    }

    public class Pippo : IPippo
    {
        public Pippo()
        {
        }

        public Pippo(IServiceProvider sp)
        {
        }

        public String Description { get; set; }

        public void Paperino(Int32 num, String str)
        {
            Console.WriteLine("Called method Pippo.Paperino (...)");
        }

        public int Pluto(string str)
        {
            return str == null ? 0 : str.Length;
        }
    }

    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    public class TestPrim
    {
        private IPippo proxyVirtual;

        [GlobalSetup]
        public void Setup()
        {
            // generator = new ProxyGenerator();

            // ProxyGenerationOptions options = new ProxyGenerationOptions();
            //options.AddMixinInstance(new Incaps2());
            //options.AddMixinInstance(new Incaps1());
            //T1 proxy = (T1)generator.CreateClassProxyWithTarget(typeof(Incaps1), new Incaps1(), new ICapsInterceptor());

            proxyVirtual = (IPippo)(new S1<IPippo>()).SingleClassProxy( typeof(Pippo));
        }

        [Benchmark]
        public void TestVirtual()
        {
            proxyVirtual.Pluto("Coccodrillo");
        }
    }

    public  class S1<T> 
    {
        Assembly compiledAssembly;
        public  T  SingleClassProxy( Type implementationType)
        {
            CompilationUnitSyntax _BuildCompileUnit(ClassDeclarationSyntax classDeclStx)
            {
                NameSyntax nameStx;
                NamespaceDeclarationSyntax nsDeclStx;

                nameStx = SyntaxHelper.IdentifierName(implementationType.Namespace, "Proxies");

                nsDeclStx = SyntaxFactory.NamespaceDeclaration(nameStx)
                            .WithMembers(SyntaxHelper.SyntaxList(
                                                (MemberDeclarationSyntax)classDeclStx));

                nsDeclStx = nsDeclStx.AddUsings(SyntaxFactory.UsingDirective(
                                                        SyntaxFactory.ParseName(
                                                            implementationType.Namespace)));

                return SyntaxFactory.CompilationUnit()
                            .WithMembers(SyntaxHelper.SyntaxList((MemberDeclarationSyntax)nsDeclStx))
                            .NormalizeWhitespace();
            }

            T _GetProxy(Assembly assembly)
                =>(T) assembly.CreateInstance(assembly.ExportedTypes.First().FullName);

            ClassDeclarationSyntax classDeclStx;
          
            CompilationUnitSyntax cuStx;
            T proxy;
            DistinctList<Assembly> referredAssemblies;
            PrologEpilogProxyGenerator roslynProxyGen;

            referredAssemblies = new DistinctList<Assembly>();
            roslynProxyGen = new PrologEpilogProxyGenerator();

            classDeclStx = roslynProxyGen.Generate( typeof ( T ), implementationType, referredAssemblies);

            cuStx = _BuildCompileUnit(classDeclStx);

            Console.WriteLine(cuStx.ToFullString());

            compiledAssembly = CompilationHelper.CompileLibrary(
                                    "InMemoryAssembly", referredAssemblies, cuStx.SyntaxTree);

            proxy = (T)_GetProxy(compiledAssembly);
            return proxy;
            // Int32 yy = proxy.Pluto("pappa");
            //proxy.Paperino(14, "ciccia");
        }

        public T GetProxy()
            => (T)compiledAssembly.CreateInstance(compiledAssembly.ExportedTypes.First().FullName);
    }

    public class Program
    {
        private static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
            //Console.ReadLine();

            (new S1<IPippo>()).SingleClassProxy( typeof(Pippo));
            (new S1<IPippo>()).SingleClassProxy( typeof(Pippo));

            //ProxyServiceContainer psc = new ProxyServiceContainer ();

            //psc.AddService<IPippo, Pippo> ();

            //psc.GetService ( typeof ( IPippo ) );

            Console.ReadKey();

            return;
        }
    }
}