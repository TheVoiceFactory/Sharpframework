﻿using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Sharpframework.Core;
using Sharpframework.Roslyn.CSharp;


namespace Sharpframework.Roslyn.DynamicProxy
{
    public class PrologEpilogProxyGenerator
        : ProxyGenerator
    {
        protected const String ReturnValueVariableName = "retVal";


        protected override ClassDeclarationSyntax ImplGenerate<ContractType, ImplementationType> (
            ContractType            contractType,
            ImplementationType      implementationType,
            DistinctList<Assembly>  referredAssemblies,
            String                  proxyClassName )
        {
            referredAssemblies.Add ( typeof ( MemberType ).Assembly );

            return base.ImplGenerate ( contractType,
                            implementationType, referredAssemblies, proxyClassName );
        }

        protected override IEnumerable<StatementSyntax> ImplGetAccessorStatementSet (
            PropertyInfo propInfo )
        {
            return base.ImplGetAccessorStatementSet ( propInfo );
        }

        protected override IEnumerable<StatementSyntax> ImplMethodStatementSet (
            MethodInfo methInfo )
        {
            IEnumerable<String> _Arguments ()
            {
                foreach ( ParameterInfo pi in methInfo.GetParameters () )
                    yield return pi.Name;


                yield break;
            }

            ArgumentListSyntax _EpilogArgumentList ( Boolean voidMethod )
            {
                IEnumerable<ArgumentSyntax> _Arguments ()
                {
                    yield return SyntaxFactory.Argument (
                                    SyntaxHelper.IdentifierName (
                                        typeof ( MemberType ).FullName,
                                        nameof ( MemberType.Method ) ) );

                    yield return SyntaxFactory.Argument (
                                    SyntaxHelper.StringLiteralExpression ( methInfo.Name ) );

                    yield return SyntaxFactory.Argument (
                                    SyntaxHelper.IdentifierName ( ProxiedObjectFieldName ) );

                    if ( voidMethod )
                        yield return SyntaxFactory.Argument (
                            SyntaxFactory.LiteralExpression ( SyntaxKind.NullLiteralExpression ) );
                    else
                        yield return SyntaxFactory.Argument (
                                        SyntaxHelper.IdentifierName ( ReturnValueVariableName ) );

                    foreach ( ParameterInfo pi in methInfo.GetParameters () )
                        yield return SyntaxFactory.Argument (
                                        SyntaxHelper.IdentifierName ( pi.Name ) );

                    yield break;
                }

                return SyntaxFactory.ArgumentList ( _Arguments ().SeparatedList () );
            }

            ArgumentListSyntax _PrologArgumentList ()
            {
                IEnumerable<ArgumentSyntax> _Arguments ()
                {
                    yield return SyntaxFactory.Argument (
                                    SyntaxHelper.IdentifierName (
                                        typeof ( MemberType ).FullName,
                                        nameof ( MemberType.Method ) ) );

                    yield return SyntaxFactory.Argument (
                                    SyntaxHelper.StringLiteralExpression ( methInfo.Name ) );

                    yield return SyntaxFactory.Argument (
                                    SyntaxHelper.IdentifierName ( ProxiedObjectFieldName ) );

                    foreach ( ParameterInfo pi in methInfo.GetParameters () )
                        yield return SyntaxFactory.Argument (
                                        SyntaxHelper.IdentifierName ( pi.Name ) );

                    yield break;
                }

                return SyntaxFactory.ArgumentList ( _Arguments ().SeparatedList () );
            }


            IEnumerable<PrologInterceptorAttribute> prologs;

            prologs = methInfo.DeclaringType.GetCustomAttributes<PrologInterceptorAttribute> ( true );

            foreach ( PrologInterceptorAttribute prolog in prologs )
                yield return SyntaxFactory.ExpressionStatement (
                                SyntaxFactory.InvocationExpression (
                                    SyntaxHelper.IdentifierName (
                                        prolog.InterceptorClass.FullName,
                                        prolog.InterceptorMethodName ),
                                    _PrologArgumentList () ) );

            IEnumerable<EpilogInterceptorAttribute> epilogs;
            Boolean                                 go;
            Boolean                                 voidMethod;

            voidMethod = methInfo.ReturnType == typeof ( void );

            epilogs = methInfo.DeclaringType.GetCustomAttributes<EpilogInterceptorAttribute> ( true );

            using ( IEnumerator<EpilogInterceptorAttribute> epilogEnum = epilogs.GetEnumerator () )
            {
                if ( !(go = epilogEnum.MoveNext ()) || voidMethod )
                {
                    foreach ( StatementSyntax stmt in base.ImplMethodStatementSet ( methInfo ) )
                        yield return stmt;

                    for ( ; go ; go = epilogEnum.MoveNext () )
                        yield return SyntaxFactory.ExpressionStatement (
                                        SyntaxFactory.InvocationExpression (
                                            SyntaxHelper.IdentifierName (
                                                epilogEnum.Current.InterceptorClass.FullName,
                                                epilogEnum.Current.InterceptorMethodName ),
                                            _EpilogArgumentList ( voidMethod ) ) );
                }
                else
                {
                    yield return SyntaxFactory.LocalDeclarationStatement (
                                    SyntaxHelper.VariableDeclaration (
                                        methInfo.ReturnType, ReturnValueVariableName ) );

                    yield return SyntaxFactory.ExpressionStatement (
                                    SyntaxFactory.AssignmentExpression (
                                        SyntaxKind.SimpleAssignmentExpression,
                                        SyntaxFactory.IdentifierName ( ReturnValueVariableName ),
                                        SyntaxHelper.InvocationExpression (
                                            methInfo.Name, _Arguments (),
                                            ProxiedObjectFieldName ) ) );

                    do
                        yield return SyntaxFactory.ExpressionStatement (
                                        SyntaxFactory.InvocationExpression (
                                            SyntaxHelper.IdentifierName (
                                                epilogEnum.Current.InterceptorClass.FullName,
                                                epilogEnum.Current.InterceptorMethodName ),
                                            _EpilogArgumentList ( voidMethod ) ) );
                    while ( epilogEnum.MoveNext () );

                    yield return SyntaxFactory.ReturnStatement (
                                    SyntaxFactory.IdentifierName ( ReturnValueVariableName ) );
                }
            }

            yield break;
        }

        protected override IEnumerable<StatementSyntax> ImplSetAccessorStatementSet (
            PropertyInfo propInfo )
        {
            return base.ImplSetAccessorStatementSet ( propInfo );
        }
    }
}