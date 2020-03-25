using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Sharpframework.Core;
using Sharpframework.Roslyn.CSharp;


namespace Sharpframework.Roslyn.DynamicProxy
{
    public class ProxyGenerator
    {
        protected const String DisposeProxiedObjectFieldName    = "__disposeProxiedObject";
        protected const String ProxiedObjectFieldName           = "__proxiedObject";
        protected const String SpConstructorParameterName       = "sp";

        private DistinctList<MethodInfo>        __contractMethods;
        private DistinctList<PropertyInfo>      __contractProperties;
        private List<MemberDeclarationSyntax>   __membersDecl;
        private DistinctList<Assembly>          __referredAssemblies;


        public ClassDeclarationSyntax Generate<ContractType, ImplementationType> (
                ContractType            contractType,
                ImplementationType      implementationType,
                DistinctList<Assembly>  referredAssemblies )
            where ImplementationType    : Type, ContractType
            where ContractType          : Type
        {
            __referredAssemblies = referredAssemblies;
            ImplClear ();
            return ImplGenerate ( contractType, implementationType, referredAssemblies, null );
        } // End of Generate<...> (...)

        public ClassDeclarationSyntax Generate<ContractType, ImplementationType> (
                ContractType            contractType,
                ImplementationType      implementationType,
                DistinctList<Assembly>  referredAssemblies,
                String                  proxyClassName )
            where ImplementationType    : Type, ContractType
            where ContractType          : Type
        {
            __referredAssemblies = referredAssemblies;
            ImplClear ();
            return ImplGenerate ( contractType,
                        implementationType, referredAssemblies, proxyClassName );
        }



        protected IReadOnlyCollection<MethodInfo> ImplContractMethods
        {
            get
            {
                if ( __contractMethods == null )
                    __contractMethods = new DistinctList<MethodInfo> ();

                if ( __contractMethods.Count < 1 )
                    __contractMethods.AddRange ( ImplObjectContract.GetMethods ()
                                .RemovePropertiesAccessors ( ImplContractProperties ) );

                return __contractMethods;
            }
        }

        protected IReadOnlyCollection<PropertyInfo> ImplContractProperties
        {
            get
            {
                if ( __contractProperties == null )
                    __contractProperties = new DistinctList<PropertyInfo> ();

                if ( __contractProperties.Count < 1 )
                    __contractProperties.AddRange (
                        ImplObjectContract.GetAllInterfacePropertiesInfo () );

                return __contractProperties;
            }
        }

        protected Type ImplObjectType { get; set; }

        protected Type ImplObjectContract { get; set; }

        protected Boolean ImplProxiedObjectIsDisposable { get; private set; }

        protected Boolean ImplProxiedObjectIsReferenceType { get; private set; }

        protected virtual IEnumerable<BaseTypeSyntax> ImplProxyBaseTypes
        {
            get
            {
                yield return SyntaxFactory.SimpleBaseType (
                                SyntaxHelper.Type ( ImplObjectContract ) );

                yield break;
            }
        }

        protected String ImplProxyClassName { get; set; }


        protected virtual void ImplAddConstructors ()
        {
            IEnumerable<Tuple<Type, String>> _CopyConstructorParameterSet (
                Type implType, String origObjPrm )
            {
                yield return Tuple.Create ( implType, origObjPrm );
                yield break;
            }

            IEnumerable<Tuple<Type, String>> _FullConstructorParameterSet ()
            {
                yield return Tuple.Create ( ImplObjectContract, "origObj" );
                yield return Tuple.Create (
                                typeof ( IServiceProvider ), SpConstructorParameterName );
                yield break;
            }

            IEnumerable<Tuple<Type, String>> _SpConstructorParameterSet ()
            {
                yield return Tuple.Create (
                                typeof ( IServiceProvider ), SpConstructorParameterName );
                yield break;
            }


            ImplAddMember ( SyntaxHelper.PublicConstructorDeclaration (
                                ImplProxyClassName,
                                ImplDefaultConstructorStatementSet (),
                                null )
                                    .WithInitializer (
                                        SyntaxFactory.ConstructorInitializer (
                                            SyntaxKind.ThisConstructorInitializer,
                                            SyntaxHelper.ArgumentList (
                                                SyntaxHelper.Argument (),
                                                SyntaxHelper.Argument () ) ) ) );

            ImplAddMember ( SyntaxHelper.PublicConstructorDeclaration (
                                ImplProxyClassName,
                                ImplCopyConstructorStatementSet ( "origObj" ),
                                _CopyConstructorParameterSet ( ImplObjectContract, "origObj" ) )
                                    .WithInitializer  (
                                        SyntaxFactory.ConstructorInitializer (
                                            SyntaxKind.ThisConstructorInitializer,
                                            SyntaxHelper.ArgumentList (
                                                SyntaxHelper.Argument ( "origObj" ),
                                                SyntaxHelper.Argument () ) ) ) );

            ImplAddMember ( SyntaxHelper.PublicConstructorDeclaration (
                                ImplProxyClassName,
                                ImplSpConstructorStatementSet (),
                                _SpConstructorParameterSet () )
                                    .WithInitializer (
                                        SyntaxFactory.ConstructorInitializer (
                                            SyntaxKind.ThisConstructorInitializer,
                                            SyntaxHelper.ArgumentList (
                                                SyntaxHelper.Argument (),
                                                SyntaxHelper.Argument (
                                                    SpConstructorParameterName ) ) ) ) );

            ImplAddMember ( SyntaxHelper.PublicConstructorDeclaration (
                                ImplProxyClassName,
                                ImplFullConstructorStatementSet (),
                                _FullConstructorParameterSet () ) );
        } // End of ImplAddConstructors ()

        protected virtual void ImplAddDestructor ()
            => ImplAddMember ( SyntaxHelper.DestructorDeclaration (
                                            ImplProxyClassName, ImplDestructorStatementSet () ) );

        protected virtual void ImplAddExplicitInterfaceImplementations () { }

        protected virtual void ImplAddFieldsDeclarations ()
        {
            foreach ( FieldDeclarationSyntax fldDeclStx in ImplFieldsDeclarations () )
                ImplAddMember ( fldDeclStx );
        } // End of ImplAddFieldsDeclarations ()

        protected virtual void ImplAddMember ( MemberDeclarationSyntax member )
        {
            __membersDecl.Add ( member );
        } // End of ImplAddMember (...)

        protected virtual void ImplAddNestedTypes () { }

        protected virtual void ImplAddPrivateMethods () { }

        protected virtual void ImplAddProtectedMethods () { }

        protected virtual void ImplAddPublicMethods ()
        {
            //IEnumerable<StatementSyntax> _GetNewInstanceStatementSet ()
            //{
            //    yield return SyntaxFactory.ReturnStatement (
            //                            SyntaxFactory.ObjectCreationExpression (
            //                                SyntaxFactory.Token ( SyntaxKind.NewKeyword ),
            //                                SyntaxFactory.IdentifierName ( ImplObjectType.Name ),
            //                                SyntaxHelper.ArgumentList (),
            //                                null ) );
            //    yield break;
            //}

            //ImplAddMember ( 
            //     SyntaxHelper.MethodDeclaration (
            //                SyntaxHelper.PublicModifier,
            //                ImplObjectContract,
            //                "GetNewInstance",
            //                null as IEnumerable<Tuple<Type, String>>,
            //                _GetNewInstanceStatementSet (),
            //                null as ExplicitInterfaceSpecifierSyntax ) );

            ImplAddMember ( SyntaxHelper.PublicMethodDeclaration (
                                typeof ( IDisposable ).GetMethod (
                                            nameof ( IDisposable.Dispose ) ),
                                ImplDisposeStatementSet () ) );

            foreach ( MethodInfo methInfo in ImplContractMethods )
            {
                ImplAddMember ( SyntaxHelper.PublicMethodDeclaration (
                                    methInfo, ImplMethodStatementSet ( methInfo ) ) );

                foreach ( ParameterInfo pi in methInfo.GetParameters () )
                    ImplAddReferredAssembly ( pi.ParameterType );

                ImplAddReferredAssembly ( methInfo.ReturnType );
            }
        } // End of ImplAddPublicMethods ()

        protected virtual void ImplAddPrivateProperties () { }

        protected virtual void ImplAddProtectedProperties () { }

        protected virtual void ImplAddPublicProperties ()
        {
            foreach ( PropertyInfo propInfo in ImplContractProperties )
            {
                ImplAddMember (
                    SyntaxHelper.PublicPropertyDeclaration (
                        propInfo,
                        propInfo.CanRead ? ImplGetAccessorStatementSet ( propInfo ) : null,
                        propInfo.CanWrite ? ImplSetAccessorStatementSet ( propInfo ) : null ) );

                ImplAddReferredAssembly ( propInfo.PropertyType );
            }
        } // End of ImplAddPublicProperties ()

        protected void ImplAddReferredAssembly ( Type type )
            => ImplAddReferredAssembly ( type == null ? (Assembly) null : type.Assembly );
        
        protected void ImplAddReferredAssembly ( Assembly assembly )
        { if ( assembly != null ) __referredAssemblies.Add ( assembly ); }

        protected virtual void ImplClear ()
        {
            if ( __contractMethods      != null ) __contractMethods.Clear ();
            if ( __contractProperties   != null ) __contractProperties.Clear ();

            if ( __membersDecl == null )
                __membersDecl = new List<MemberDeclarationSyntax> ();
            else
                __membersDecl.Clear ();
        } // End of ImplClear ()

        protected virtual IEnumerable<StatementSyntax> ImplCopyConstructorStatementSet (
            String origObjPrm )
        {
            yield break;
        } // End of ImplCopyConstructorStatementSet (...)

        protected virtual ClassDeclarationSyntax ImplDeclareProxyClass ()
        {
            return SyntaxFactory.ClassDeclaration ( ImplProxyClassName )
                        .WithModifiers ( SyntaxHelper.PublicModifier )
                        .WithBaseList ( SyntaxFactory.BaseList (
                                            ImplProxyBaseTypes.SeparatedList () ) )
                        .WithMembers ( __membersDecl.SyntaxList () );
        } // End of ImplDeclareProxyClass ()

        protected virtual IEnumerable<StatementSyntax> ImplDefaultConstructorStatementSet ()
            => _ConstructorStatementSet ( SyntaxHelper.ArgumentList () );

        protected virtual IEnumerable<StatementSyntax> ImplDestructorStatementSet ()
        {
            //yield return SyntaxFactory.ExpressionStatement (
            //                SyntaxHelper.InvocationExpression ( nameof ( IDisposable.Dispose ) ) );
            yield return SyntaxHelper.MethodInvokation ( nameof ( IDisposable.Dispose ) );

            yield break;
        } // End of ImplDestructorStatementSet ()

        protected virtual IEnumerable<StatementSyntax> ImplDisposeStatementSet ()
        {
            if ( ImplProxiedObjectIsDisposable )
            {
                ExpressionSyntax ifClause;

                ifClause =  SyntaxFactory.BinaryExpression (
                                SyntaxKind.NotEqualsExpression,
                                SyntaxHelper.IdentifierName ( ProxiedObjectFieldName ),
                                SyntaxHelper.NullLiteralExpression );

                if ( ImplProxiedObjectIsReferenceType )
                {
                    // if ( __disposeProxiedObject && __proxiedObject != null ) __proxiedObject.Dispose ();
                    yield return SyntaxFactory.IfStatement (
                                    SyntaxFactory.BinaryExpression (
                                        SyntaxKind.LogicalAndExpression,
                                        SyntaxFactory.IdentifierName ( DisposeProxiedObjectFieldName ),
                                        ifClause ),
                                    //SyntaxFactory.ExpressionStatement (
                                    //    SyntaxFactory.InvocationExpression (
                                    //        SyntaxHelper.IdentifierName (
                                    //            ProxiedObjectFieldName,
                                    //            nameof ( IDisposable.Dispose ) ),
                                    //        SyntaxHelper.ArgumentList () ) ) );
                                    SyntaxHelper.MethodInvokation ( ProxiedObjectFieldName,
                                                            nameof ( IDisposable.Dispose ) ) );

                    // <ProxiedObjectFieldName> = null;
                    yield return SyntaxFactory.ExpressionStatement (
                                    SyntaxFactory.AssignmentExpression (
                                        SyntaxKind.SimpleAssignmentExpression,
                                        SyntaxFactory.IdentifierName ( ProxiedObjectFieldName ),
                                        SyntaxHelper.NullLiteralExpression ) );

                }
                else
                    // if ( __disposeProxiedObject ) __proxiedObject.Dispose ();
                    yield return SyntaxFactory.IfStatement (
                                    SyntaxFactory.IdentifierName ( DisposeProxiedObjectFieldName ),
                                    //SyntaxFactory.ExpressionStatement (
                                    //    SyntaxFactory.InvocationExpression (
                                    //        SyntaxHelper.IdentifierName (
                                    //            ProxiedObjectFieldName,
                                    //            nameof ( IDisposable.Dispose ) ),
                                    //        SyntaxHelper.ArgumentList () ) ) );
                                    SyntaxHelper.MethodInvokation ( ProxiedObjectFieldName,
                                                            nameof ( IDisposable.Dispose ) ) );
            }

            yield break;
        } // End of ImplDisposeStatementSet ()

        protected virtual IEnumerable<FieldDeclarationSyntax> ImplFieldsDeclarations ()
        {
            if ( ImplProxiedObjectIsDisposable )
                yield return SyntaxHelper.PrivateFieldDeclaration (
                                typeof ( Boolean ), DisposeProxiedObjectFieldName );


            yield return SyntaxHelper.PrivateFieldDeclaration (
                            ImplObjectContract, ProxiedObjectFieldName );

            yield break;
        } // End of ImplFieldsDeclarations ()

        protected virtual IEnumerable<StatementSyntax> ImplFullConstructorStatementSet ()
        {
            ExpressionSyntax ifClause = SyntaxFactory.BinaryExpression (
                                            SyntaxKind.EqualsExpression,
                                            SyntaxFactory.IdentifierName ( "origObj" ),
                                            SyntaxHelper.NullLiteralExpression );

            if ( ImplProxiedObjectIsDisposable )
                ifClause = SyntaxFactory.AssignmentExpression (
                                    SyntaxKind.SimpleAssignmentExpression,
                                    SyntaxHelper.IdentifierName ( DisposeProxiedObjectFieldName ),
                                    ifClause );

            yield return SyntaxFactory.ExpressionStatement (
                                SyntaxFactory.AssignmentExpression (
                                    SyntaxKind.SimpleAssignmentExpression,
                                    SyntaxFactory.IdentifierName ( ProxiedObjectFieldName ),
                                    SyntaxFactory.ConditionalExpression (
                                        ifClause,
                                        SyntaxFactory.ObjectCreationExpression (
                                            SyntaxFactory.Token ( SyntaxKind.NewKeyword ),
                                            SyntaxFactory.IdentifierName ( ImplObjectType.Name ),
                                            SyntaxHelper.ArgumentList (
                                                ImplObjectType.GetConstructor (
                                                    typeof ( IServiceProvider ) ) == null
                                                        ? null : SpConstructorParameterName ),
                                            null ),
                                        SyntaxFactory.IdentifierName ( "origObj" )
                                        ) ) );

            //foreach ( StatementSyntax stmtStx in ImplCopyConstructorStatementSet (  ) )
            //    yield return stmtStx;

            yield break;
        } // End of ImplFullConstructorStatementSet ()

        protected virtual ClassDeclarationSyntax ImplGenerate<ContractType, ImplementationType> (
                ContractType            contractType,
                ImplementationType      implementationType,
                DistinctList<Assembly>  referredAssemblies,
                String                  proxyClassName )
            where ImplementationType    : Type, ContractType
            where ContractType          : Type
        {
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

            ImplProxiedObjectIsDisposable = typeof ( IDisposable )
                                                .IsAssignableFrom ( ImplObjectType );
            ImplProxiedObjectIsReferenceType = !ImplObjectType.IsValueType;

            ImplAddReferredAssembly ( contractType );
            ImplAddReferredAssembly ( implementationType );
            ImplAddReferredAssembly ( typeof ( IServiceProvider ) );

            ImplProxyClassName = ImplInitProxyClassName ( proxyClassName );

            ImplAddNestedTypes ();
            ImplAddFieldsDeclarations ();
            ImplAddConstructors ();
            ImplAddDestructor ();
            ImplAddPublicProperties ();
            ImplAddPublicMethods ();
            ImplAddProtectedProperties ();
            ImplAddProtectedMethods ();
            ImplAddPrivateProperties ();
            ImplAddPrivateMethods ();
            ImplAddExplicitInterfaceImplementations ();

            return ImplDeclareProxyClass ();
        } // End of ImplGenerate<...> (...)

        protected virtual IEnumerable<StatementSyntax> ImplGetAccessorStatementSet (
            PropertyInfo propInfo )
        {
            yield return SyntaxFactory.ReturnStatement (
                                SyntaxFactory.QualifiedName (
                                    SyntaxFactory.IdentifierName ( ProxiedObjectFieldName ),
                                    SyntaxFactory.IdentifierName ( propInfo.Name ) ) );

            yield break;
        } // End of ImplGetAccessorStatementSet (...)

        protected virtual String ImplInitProxyClassName ( String name )
            => String.IsNullOrWhiteSpace ( name ) ? ImplObjectType.Name + "Proxy" : name;

        protected virtual IEnumerable<StatementSyntax> ImplMethodStatementSet (
            MethodInfo methInfo )
        {
            //IEnumerable<String> _Arguments ()
            //{
            //    foreach ( ParameterInfo pi in methInfo.GetParameters () )
            //        yield return pi.Name;


            //    yield break;
            //}

            //if ( methInfo.ReturnType == typeof ( void ) )
            //    yield return SyntaxFactory.ExpressionStatement (
            //                    SyntaxHelper.InvocationExpression (
            //                        methInfo.Name, _Arguments (), ProxiedObjectFieldName ) );
            //else
            //    yield return SyntaxFactory.ReturnStatement (
            //                    SyntaxHelper.InvocationExpression (
            //                        methInfo.Name, _Arguments (), ProxiedObjectFieldName ) );

            if ( methInfo.ReturnType == typeof ( void ) )
                yield return SyntaxHelper.MethodInvokation (    ProxiedObjectFieldName,
                                                                methInfo.Name )
                                            .AddArguments ( methInfo.GetParameters () );
            else
                yield return SyntaxHelper.MethodInvokation (    ProxiedObjectFieldName,
                                                                methInfo.Name )
                                            .AddArguments ( methInfo.GetParameters () )
                                        .ToReturnStatement ();

            yield break;
        } // End of ImplMethodStatementSet (...)
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
        } // End of ImplSetAccessorStatementSet (...)


        protected virtual IEnumerable<StatementSyntax> ImplSpConstructorStatementSet ()
            => _ConstructorStatementSet ( SyntaxHelper.ArgumentList (
                    ImplObjectType.GetConstructor ( typeof ( IServiceProvider ) ) == null
                        ? null : SpConstructorParameterName ) );

        private IEnumerable<StatementSyntax> _ConstructorStatementSet (
            ArgumentListSyntax arguments )
        {
            yield break;
        }
    } // End of Class ProxyGenerator
} // End of Namespace Sharpframework.Roslyn.DynamicProxy
