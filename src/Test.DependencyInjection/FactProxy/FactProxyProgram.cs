using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Sharpframework.Core;
using Sharpframework.Roslyn.CSharp;


namespace Test.DependencyInjection.FactProxy
{
    public class FactProxyProgram
    {
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

            compiledAssembly = CompilationHelper.CompileLibrary (
                                    "InMemoryAssembly", referredAssemblies, cuStx.SyntaxTree );

            //SpAllocators.GetSpAllocator<TestFactClass> () ( null );

            return;
        }
    }
}
