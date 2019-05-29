using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Sharpframework.Roslyn.CSharp;


namespace Test.DependencyInjection.DynamicProxy
{
    public static class ExternalMethods
    {
        public static void StandardProlog ()
        {
            Console.WriteLine ( "Standard Prolog Invoked" );
        }
    }

    public class TestProxy
        : ProxyGenerator
    {
        protected override IEnumerable<StatementSyntax> ImplGetAccessorStatementSet (
            PropertyInfo propInfo )
        {
            return base.ImplGetAccessorStatementSet ( propInfo );
        }

        protected override IEnumerable<StatementSyntax> ImplMethodStatementSet (
            MethodInfo methInfo )
        {
            yield return SyntaxFactory.ExpressionStatement (
                            SyntaxHelper.InvocationExpression (
                                "Test.DependencyInjection.DynamicProxy.ExternalMethods", "StandardProlog" ) );

            foreach ( StatementSyntax stmt in base.ImplMethodStatementSet ( methInfo ) )
                yield return stmt;

            yield break;
        }

        protected override IEnumerable<StatementSyntax> ImplSetAccessorStatementSet (
            PropertyInfo propInfo )
        {
            return base.ImplSetAccessorStatementSet ( propInfo );
        }
    }
}
