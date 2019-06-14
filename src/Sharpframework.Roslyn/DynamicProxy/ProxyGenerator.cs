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
        protected const String ProxiedObjectFieldName       = "__proxiedObject";
        protected const String SpConstructorParameterName   = "sp";

        private List<MemberDeclarationSyntax>   __membersDecl;
        private DistinctList<Assembly>          __referredAssemblies;


        public ClassDeclarationSyntax Generate<ContractType, ImplementationType> (
                ContractType            contractType,
                ImplementationType      implementationType,
                DistinctList<Assembly>  referredAssemblies )
            where ImplementationType    : Type, ContractType
            where ContractType          : Type
                => ImplGenerate ( contractType, implementationType, __referredAssemblies = referredAssemblies, null );

        public ClassDeclarationSyntax Generate<ContractType, ImplementationType> (
                ContractType            contractType,
                ImplementationType      implementationType,
                DistinctList<Assembly>  referredAssemblies,
                String                  proxyClassName )
            where ImplementationType    : Type, ContractType
            where ContractType          : Type
                => ImplGenerate ( contractType,
                        implementationType, __referredAssemblies = referredAssemblies, proxyClassName );



        protected virtual void ImplAddMember ( MemberDeclarationSyntax member )
        {
            __membersDecl.Add ( member );
        }

        protected void AddReferredAssembly ( Type type )
            => AddReferredAssembly ( type == null ? (Assembly) null : type.Assembly );
        protected void AddReferredAssembly ( Assembly assembly )
        { if ( assembly != null ) __referredAssemblies.Add ( assembly ); }
        protected virtual IEnumerable<StatementSyntax> ImplCopyConstructorStatementSet (
            String origObjPrm )
        {
            yield return SyntaxFactory.ExpressionStatement (
                            SyntaxFactory.AssignmentExpression (
                                SyntaxKind.SimpleAssignmentExpression,
                                SyntaxFactory.IdentifierName ( ProxiedObjectFieldName ),
                                SyntaxFactory.IdentifierName ( origObjPrm ) ) );

            yield break;
        }

        protected virtual IEnumerable<StatementSyntax> ImplDefaultConstructorStatementSet ()
            => _ConstructorStatementSet ( SyntaxHelper.ArgumentList () );

        protected virtual IEnumerable<FieldDeclarationSyntax> ImplFieldsDeclarations ()
        {
            yield return SyntaxHelper.PrivateFieldDeclaration (
                            ImplObjectContract, ProxiedObjectFieldName );

            yield break;
        }
        protected virtual ClassDeclarationSyntax ImplGenerate<ContractType, ImplementationType> (
                ContractType            contractType,
                ImplementationType      implementationType,
                DistinctList<Assembly>  referredAssemblies,
                String                  proxyClassName )
            where ImplementationType    : Type, ContractType
            where ContractType          : Type
        {
            IEnumerable<Tuple<Type, String>> _CopyConstructorParameterSet (
                Type implType, String origObjPrm )
            {
                yield return Tuple.Create ( implType, origObjPrm );
                yield break;
            }

            IEnumerable<Tuple<Type, String>> _SpConstructorParameterSet ()
            {
                yield return Tuple.Create (
                                typeof ( IServiceProvider ), SpConstructorParameterName );
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

            IEnumerable<PropertyInfo> properties;

            referredAssemblies.Add ( contractType.Assembly );
            referredAssemblies.Add ( implementationType.Assembly );
            referredAssemblies.Add ( typeof ( IServiceProvider ).Assembly );

            ImplProxyClassName = ImplInitProxyClassName ( proxyClassName );

            __membersDecl = new List<MemberDeclarationSyntax> ();

            foreach ( FieldDeclarationSyntax fldDeclStx in ImplFieldsDeclarations () )
                ImplAddMember ( fldDeclStx );

            ImplAddMember ( SyntaxHelper.PublicConstructorDeclaration (
                                ImplProxyClassName,
                                ImplDefaultConstructorStatementSet (),
                                null ) );

            ImplAddMember ( SyntaxHelper.PublicConstructorDeclaration (
                                ImplProxyClassName,
                                ImplCopyConstructorStatementSet ( "origObj" ),
                                _CopyConstructorParameterSet ( contractType, "origObj" ) ) );

            ImplAddMember ( SyntaxHelper.PublicConstructorDeclaration (
                                ImplProxyClassName,
                                ImplSpConstructorStatementSet (),
                                _SpConstructorParameterSet () ) );


            properties = contractType.GetAllInterfacePropertiesInfo ();

            foreach ( MethodInfo methInfo in contractType.GetMethods ()
                                                .RemovePropertiesAccessors ( properties ) )
            {
                ImplAddMember ( SyntaxHelper.PublicMethodDeclaration (
                                    methInfo, ImplMethodStatementSet ( methInfo ) ) );

                foreach ( ParameterInfo pi in methInfo.GetParameters () )
                    referredAssemblies.Add ( pi.ParameterType.Assembly );

                referredAssemblies.Add ( methInfo.ReturnType.Assembly );
            }

            foreach ( PropertyInfo propInfo in properties )
            {
                ImplAddMember (
                    SyntaxHelper.PublicPropertyDeclaration (
                        propInfo,
                        propInfo.CanRead ? ImplGetAccessorStatementSet ( propInfo ) : null,
                        propInfo.CanWrite ? ImplSetAccessorStatementSet ( propInfo ) : null ) );

                referredAssemblies.Add ( propInfo.PropertyType.Assembly );
            }

            return SyntaxFactory.ClassDeclaration ( ImplProxyClassName )
                        .WithModifiers ( SyntaxHelper.PublicModifier )
                        .WithBaseList ( SyntaxHelper.BaseList ( true, ImplObjectContract ) )
                        .WithMembers ( __membersDecl.SyntaxList () );
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


        protected virtual IEnumerable<StatementSyntax> ImplSpConstructorStatementSet ()
            => _ConstructorStatementSet ( SyntaxHelper.ArgumentList (
                    ImplObjectType.GetConstructor ( typeof ( IServiceProvider ) ) == null
                        ? null : "sp" ) );

        private IEnumerable<StatementSyntax> _ConstructorStatementSet (
            ArgumentListSyntax arguments )
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
    }
}
