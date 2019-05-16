using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;


namespace Test.Attributes
{
    public class MethodsEpilogAttribute : Attribute
    {
        public MethodsEpilogAttribute ( String epilogMethodName ) { }
    }

    public class MethodsPrologAttribute : Attribute
    {
        public MethodsPrologAttribute ( String prologMethodName ) { }
    }
}

namespace Test.DependencyInjection
{
    class Program
    {
        public interface IMyService
        {
            String StringDecorator ( String target );
        }

        public interface IMyHostedService
            : IHostedService
        {
            String StringService ( String data );
        }
        private class MyService : IMyService
        {
            public String StringDecorator ( String target )
            {
                return "$$$" + target + "@@@";
            }
        }

        private class MyHostedService
            : BackgroundService, IMyHostedService
        {
            private IMyService          __mySer;
            private IServiceProvider    __sp;

            public MyHostedService  ( IServiceProvider sp )
            {
                __sp = sp;

                if (sp == null) return;

                __mySer = sp.GetService<IMyService> ();
            }
            public override Task StartAsync ( CancellationToken cancellationToken )
            {
                return base.StartAsync ( cancellationToken );
            }

            public override Task StopAsync ( CancellationToken cancellationToken )
            {
                return base.StopAsync ( cancellationToken );
            }
            protected async override Task ExecuteAsync ( CancellationToken stoppingToken )
            {
                async Task<String> ConsoleReadLine ( CancellationToken stoppingToken )
                    => await Task<String>.Run ( () => Console.ReadLine (), stoppingToken );

                String lineStr = String.Empty;

                while ( (lineStr = await ConsoleReadLine ( stoppingToken )) != null )
                {
                    Console.WriteLine ( "Echo: {0}", lineStr );
                    Console.WriteLine ( "Local Computation Result: {0}", StringService ( lineStr ) );
                    Console.WriteLine ( "StringDecorator Result: {0}", __mySer.StringDecorator ( lineStr ) );
                }
            }

            public String StringService ( String data )
            {
                return "!!!" + data + "%%%";
            }
        }


        //[ MethodsEpilog ( nameof ( Program1._Epilog ) ) ]
        //public class Program1
        //{
        //    static void Main ( string [] args )
        //    {
        //        object s = "abc";
        //    }

        //    public void Pippo ()
        //    {
        //    }

        //    private void _Epilog ()
        //    {
        //    }

        //    private static void _Prolog ()
        //    {
        //    }
        //}
        public static class RsolynTest
        {
            private const string ProgramString = @"
                using System;
                using Test.Attributes;

                namespace MakeConstTest
                {
                    [ MethodsProlog ( ""_Prolog"" ) ]
                    [ MethodsEpilog ( ""_Epilog"" ) ]
                    public class Program
                    {
                        static void Main ( string [] args )
                        {
                            object s = ""abc"";
                            Int32  k = 12;

                            for ( Int32 k = 0 ; k < 10 ; k++ )
                            {
                                s += k.ToString ();
                            }
                        }

                        public void Pippo ()
                        {
                        }

                        private static void _Epilog ()
                        {
                        }

                        private static void _Prolog ()
                        {
                        }
                    }
                }";
         
            private class PrologEpilogSyntaxVisitor
                : CSharpSyntaxVisitor<InvocationExpressionSyntax>
            {
                public override InvocationExpressionSyntax VisitClassDeclaration ( ClassDeclarationSyntax node )
                {
                    return base.VisitClassDeclaration ( node );
                }
                public override InvocationExpressionSyntax VisitMethodDeclaration ( MethodDeclarationSyntax node )
                {
                    return base.VisitMethodDeclaration ( node );
                }
                public override InvocationExpressionSyntax VisitAttribute ( AttributeSyntax node )
                {
                    return base.VisitAttribute ( node );
                }
            }
            private class MethodPrologEpilogRewriter : CSharpSyntaxRewriter
            {
                public override SyntaxNode VisitBlock ( BlockSyntax node )
                {
                    SyntaxNode retVal = base.VisitBlock ( node );

                    InvocationExpressionSyntax epilogInvocation =
                        SyntaxFactory.InvocationExpression (
                            SyntaxFactory.IdentifierName ( "_Epilog" ) );

                    InvocationExpressionSyntax prologInvocation =
                        SyntaxFactory.InvocationExpression (
                            SyntaxFactory.IdentifierName ( "_Prolog" ) );

                    ExpressionStatementSyntax prologInvocationStatement =
                        SyntaxFactory.ExpressionStatement ( prologInvocation );

                    ExpressionStatementSyntax epilogInvocationStatement =
                        SyntaxFactory.ExpressionStatement ( epilogInvocation );

                    BlockSyntax block = node.AddStatements ( epilogInvocationStatement );

                    block = block.InsertNodesBefore (
                                    block.Statements [ 0 ],
                                    new SyntaxNode [] { prologInvocationStatement } );

                    return block;
                }
            }
            private class ClassRewriter : CSharpSyntaxRewriter
            {
                private MethodPrologEpilogRewriter __methodBodyrewriter;

                private MethodPrologEpilogRewriter _MethodBodyRewriter
                {
                    get
                    {
                        if ( __methodBodyrewriter == null )
                            __methodBodyrewriter = new MethodPrologEpilogRewriter ();

                        return __methodBodyrewriter;
                    }
                }

                public override SyntaxNode VisitAttribute ( AttributeSyntax node )
                {
                    String _GetAttributeName ()
                    {
                        IdentifierNameSyntax idName;

                        if ( (idName = node.Name as IdentifierNameSyntax) != null )
                            return idName.Identifier.ValueText;

                        return String.Empty;
                    }

                    String _MethodPrologName ( ExpressionSyntax expr )
                    {
                        LiteralExpressionSyntax litExpr;

                        if ( (litExpr = expr as LiteralExpressionSyntax) != null )
                            return litExpr.Token.ValueText;

                        return String.Empty;
                    }

                    if ( node.ArgumentList == null )
                        return base.VisitAttribute ( node );
                    
                    if ( node.ArgumentList.Arguments== null )
                        return base.VisitAttribute ( node );

                    if ( node.ArgumentList.Arguments.Count != 1)
                        return base.VisitAttribute ( node );

                    String methodPrologName = String.Empty;

                    if ( _GetAttributeName () == "MethodsProlog" )
                        methodPrologName = _MethodPrologName ( node.ArgumentList.Arguments [0].Expression );
                        //IdentifierNameSyntax idName = node.Name as IdentifierNameSyntax;

                        //if ( idName != null && idName.Identifier.ValueText == "MethodsProlog" )
                        //    ;

                        //if ( node.Name == "Pippo" ) return node;

                        return base.VisitAttribute ( node );
                }
                public override SyntaxNode VisitBlock ( BlockSyntax node )
                {
                    if ( node.Parent is MethodDeclarationSyntax )
                        if ( node.Statements.Count > 0 )
                            return base.VisitBlock ( node );
                        else
                            return node.AddStatements ( _PrologInvocationStatement,
                                                        _EpilogInvocationStatement );
                    else
                        return node;
                }
                public override SyntaxList<TNode> VisitList<TNode> ( SyntaxList<TNode> list )
                {
                    if ( list.Count < 1 )
                        return base.VisitList ( list );

                    if ( !(list [0].Parent is BlockSyntax) )
                        return base.VisitList ( list );

                    if ( !(list [0].Parent.Parent is MethodDeclarationSyntax) )
                        return base.VisitList ( list );

                    return list
                            .Insert ( 0, _PrologInvocationStatement as TNode )
                            .Add ( _EpilogInvocationStatement as TNode );
                    //return base.VisitList ( list );
                }
                public override SyntaxNode VisitMethodDeclaration ( MethodDeclarationSyntax node )
                {
                    if ( node.Identifier.ValueText == "Main" )
                        return base.VisitMethodDeclaration ( node );

                    if ( node.Identifier.ValueText == "Pippo" )
                        return base.VisitMethodDeclaration ( node );

                    return node;
                    //SyntaxNode retVal = base.VisitMethodDeclaration ( node );

                    //if ( node.Identifier.ValueText == "Main" || node.Identifier.ValueText == "Pippo" )
                    //{
                    //    SyntaxNode rewrittenMethod = _MethodBodyRewriter.Visit ( node );

                    //    return rewrittenMethod == null ? node : rewrittenMethod;
                    //}

                    //return retVal;
                }


                private InvocationExpressionSyntax _EpilogInvocationSyntax
                {
                    get => SyntaxFactory.InvocationExpression (
                            SyntaxFactory.IdentifierName ( "_Epilog" ) );
                }

                private ExpressionStatementSyntax _EpilogInvocationStatement
                { get => SyntaxFactory.ExpressionStatement ( _EpilogInvocationSyntax ); }

                private InvocationExpressionSyntax _ProogInvocationSyntax
                {
                    get => SyntaxFactory.InvocationExpression (
                            SyntaxFactory.IdentifierName ( "_Prolog" ) );
                }

                private ExpressionStatementSyntax _PrologInvocationStatement
                { get => SyntaxFactory.ExpressionStatement ( _ProogInvocationSyntax ); }

            }
            public static void Pippo ()
            {
                MSBuildWorkspace workspace = MSBuildWorkspace.Create ();
                ClassRewriter   classRwrt;
                SyntaxTree      synTree;
                SyntaxNode      rewritten;

                classRwrt   = new ClassRewriter ();
                synTree     = CSharpSyntaxTree.ParseText ( ProgramString );
                rewritten   = classRwrt.Visit ( synTree.GetRoot () );

                Console.WriteLine ( rewritten.NormalizeWhitespace ().ToFullString () );
                //CSharpCompilation csc = new CSharpCompilation (
            }
        }
        static async Task Main ( string [] args )
        {
            RsolynTest.Pippo ();

             IHostBuilder hb = new HostBuilder ()
                .ConfigureServices((hostContext, services) =>
                {
                    //services.AddHostedService<MyHostedService> ();
                    services.AddSingleton<IHostedService, MyHostedService> ();
                    services.AddTransient<IMyService, MyService>();
                });

            Console.WriteLine("Hello World!");

            //Task tsk = hb.RunConsoleAsync ();
            await hb.RunConsoleAsync ();
        }
    }
}
