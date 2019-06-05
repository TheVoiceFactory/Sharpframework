using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Sharpframework.Core;
using Sharpframework.Roslyn.CSharp;


namespace Sharpframework.Roslyn.DynamicProxy
{
    public class ProxyGenerator
    {
        protected const String ProxiedObjectFieldName = "__proxiedObject";


        public ClassDeclarationSyntax Generate<ContractType, ImplementationType> (
                ContractType            contractType,
                ImplementationType      implementationType,
                DistinctList<Assembly>  referredAssemblies )
            where ImplementationType    : Type, ContractType
            where ContractType          : Type
                => ImplGenerate ( contractType, implementationType, referredAssemblies, null );

        public ClassDeclarationSyntax Generate<ContractType, ImplementationType> (
                ContractType            contractType,
                ImplementationType      implementationType,
                DistinctList<Assembly>  referredAssemblies,
                String                  proxyClassName )
            where ImplementationType    : Type, ContractType
            where ContractType          : Type
                => ImplGenerate ( contractType,
                        implementationType, referredAssemblies, proxyClassName );



        protected virtual ClassDeclarationSyntax ImplGenerate<ContractType, ImplementationType> (
                ContractType            contractType,
                ImplementationType      implementationType,
                DistinctList<Assembly>  referredAssemblies,
                String                  proxyClassName )
            where ImplementationType    : Type, ContractType
            where ContractType          : Type
        {
            IEnumerable<StatementSyntax> _ConstructorStatementSet ( ArgumentListSyntax arguments )
            {
                yield return SyntaxFactory.ExpressionStatement (
                                SyntaxFactory.AssignmentExpression (
                                    SyntaxKind.SimpleAssignmentExpression,
                                    SyntaxFactory.IdentifierName ( ProxiedObjectFieldName ),
                                    SyntaxFactory.ObjectCreationExpression (
                                        SyntaxFactory.Token ( SyntaxKind.NewKeyword ),
                                        SyntaxFactory.ParseTypeName ( ImplObjectType.Name ),
                                        arguments,
                                        null ) ) );

                yield break;
            }

            IEnumerable<Tuple<Type, String>> _CopyConstructorParameterSet ( Type implType, String origObjPrm )
            {
                yield return Tuple.Create ( implType, origObjPrm );
                yield break;
            }

            IEnumerable<StatementSyntax> _CopyConstructorStatementSet ( String origObjPrm )
            {
                yield return SyntaxFactory.ExpressionStatement (
                                SyntaxFactory.AssignmentExpression (
                                    SyntaxKind.SimpleAssignmentExpression,
                                    SyntaxFactory.IdentifierName ( ProxiedObjectFieldName ),
                                    SyntaxFactory.IdentifierName ( origObjPrm ) ) );

                yield break;
            }

            IEnumerable<Tuple<Type, String>> _SpConstructorParameterSet ()
            {
                yield return Tuple.Create ( typeof ( IServiceProvider ), "sp" );
                yield break;
            }


            ImplObjectContract  = contractType;
            ImplObjectType      = implementationType;

            if ( !ImplObjectContract.IsInterface )
                throw new ArgumentException (
                            String.Format ( "The type specified in ContractType ({0}), must be an Interface.",
                                            ImplObjectContract.Name ) );

            if ( !ImplObjectType.IsClass )
                throw new ArgumentException (
                            String.Format ( "The type specified in ImplementationType ({0}), must be a Class.",
                                            ImplObjectType.Name ) );

            ArgumentListSyntax              argListStx = SyntaxHelper.ArgumentList ();
            List<MemberDeclarationSyntax>   membersDeclStx;
            PropertyInfo []                 properties;

            referredAssemblies.Add ( contractType.Assembly );
            referredAssemblies.Add ( implementationType.Assembly );
            referredAssemblies.Add ( typeof ( IServiceProvider ).Assembly );

            ImplProxyClassName = ImplInitProxyClassName ( proxyClassName );

            membersDeclStx = new List<MemberDeclarationSyntax> ();

            membersDeclStx.Add ( SyntaxHelper.PrivateFieldDeclaration (
                                    ImplObjectContract, ProxiedObjectFieldName ) );

            membersDeclStx.Add ( SyntaxHelper.PublicConstructorDeclaration (
                                        ImplProxyClassName,
                                        _ConstructorStatementSet ( argListStx ),
                                        null ) );

            membersDeclStx.Add ( SyntaxHelper.PublicConstructorDeclaration (
                                        ImplProxyClassName,
                                        _CopyConstructorStatementSet ( "origObj" ),
                                        _CopyConstructorParameterSet ( contractType, "origObj" ) ) );

            if ( ImplObjectType.GetConstructor ( typeof ( IServiceProvider ) ) == null )
                argListStx = SyntaxHelper.ArgumentList ();
            else
                argListStx = SyntaxHelper.ArgumentList ( "sp" );

            membersDeclStx.Add ( SyntaxHelper.PublicConstructorDeclaration (
                                        ImplProxyClassName,
                                        _ConstructorStatementSet ( argListStx ),
                                        _SpConstructorParameterSet () ) );


            properties = contractType.GetProperties ();

            foreach ( MethodInfo methInfo in contractType.GetMethods ()
                                                .RemovePropertiesAccessors ( properties ) )
            {
                membersDeclStx.Add ( SyntaxHelper.PublicMethodDeclaration (
                                            methInfo, ImplMethodStatementSet ( methInfo ) ) );

                foreach ( ParameterInfo pi in methInfo.GetParameters () )
                    referredAssemblies.Add ( pi.ParameterType.Assembly );

                referredAssemblies.Add ( methInfo.ReturnType.Assembly );
            }

            foreach ( PropertyInfo propInfo in properties )
            {
                membersDeclStx.Add (
                    SyntaxHelper.PublicPropertyDeclaration (
                        propInfo,
                        propInfo.CanRead ? ImplGetAccessorStatementSet ( propInfo ) : null,
                        propInfo.CanWrite ? ImplSetAccessorStatementSet ( propInfo ) : null ) );

                referredAssemblies.Add ( propInfo.PropertyType.Assembly );
            }

            return SyntaxFactory.ClassDeclaration ( ImplProxyClassName )
                        .WithModifiers ( SyntaxHelper.PublicModifier )
                        .WithBaseList ( SyntaxHelper.BaseList ( true, ImplObjectContract ) )
                        .WithMembers ( membersDeclStx.SyntaxList () );
        }

        protected virtual IEnumerable<StatementSyntax> ImplGetAccessorStatementSet (
            PropertyInfo propInfo )
        {
            yield return SyntaxFactory.ReturnStatement (
                                SyntaxFactory.QualifiedName (
                                    SyntaxFactory.IdentifierName ( ProxiedObjectFieldName ),
                                    SyntaxFactory.IdentifierName ( propInfo.Name ) ) );

            yield break;
        }

        protected virtual String ImplInitProxyClassName ( String name )
            => String.IsNullOrWhiteSpace ( name ) ? ImplObjectType.Name + "Proxy" : name;

        protected virtual IEnumerable<StatementSyntax> ImplMethodStatementSet (
            MethodInfo methInfo )
        {
            IEnumerable<String> _Arguments ()
            {
                foreach ( ParameterInfo pi in methInfo.GetParameters () )
                    yield return pi.Name;


                yield break;
            }

            if ( methInfo.ReturnType == typeof ( void ) )
                yield return SyntaxFactory.ExpressionStatement (
                                SyntaxHelper.InvocationExpression (
                                    methInfo.Name, _Arguments (), ProxiedObjectFieldName ) );
            else
                yield return SyntaxFactory.ReturnStatement (
                                SyntaxHelper.InvocationExpression (
                                    methInfo.Name, _Arguments (), ProxiedObjectFieldName ) );

            yield break;
        }
        protected Type ImplObjectType { get; set; }

        protected Type ImplObjectContract { get; set; }

        protected String ImplProxyClassName { get; set; }

        protected virtual IEnumerable<StatementSyntax> ImplSetAccessorStatementSet (
            PropertyInfo propInfo )
        {
            yield return  SyntaxFactory.ExpressionStatement (
                                SyntaxFactory.AssignmentExpression (
                                    SyntaxKind.SimpleAssignmentExpression,
                                    SyntaxFactory.QualifiedName (
                                        SyntaxFactory.IdentifierName ( ProxiedObjectFieldName ),
                                        SyntaxFactory.IdentifierName ( propInfo.Name ) ),
                                    SyntaxFactory.IdentifierName ( "value" ) ) );

            yield break;
        }
    }
}
