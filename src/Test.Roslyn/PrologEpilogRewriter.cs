using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Test.Roslyn
{
    public class PrologEpilogRewriter : CSharpSyntaxRewriter
    {
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
            }
            public override SyntaxNode VisitMethodDeclaration ( MethodDeclarationSyntax node )
            {
                if ( node.Identifier.ValueText == "Main" )
                    return base.VisitMethodDeclaration ( node );

                if ( node.Identifier.ValueText == "Pippo" )
                    return base.VisitMethodDeclaration ( node );

                if ( node.Identifier.ValueText == "Pluto" )
                    return base.VisitMethodDeclaration ( node );

                if ( node.Identifier.ValueText == "Paperino" )
                    return base.VisitMethodDeclaration ( node );

                return node;
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
}
