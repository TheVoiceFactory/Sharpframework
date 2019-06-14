using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Sharpframework.Core;
using Sharpframework.EntityModel;
using Sharpframework.EntityModel.Implementation;
using Sharpframework.Roslyn.CSharp;
using Sharpframework.Roslyn.DynamicProxy;
using Sharpframework.Propagation.Facts;


namespace Test.DependencyInjection.FactProxy
{
    public class FactProxyGenerator
        : ProxyGenerator
    {
        protected const String FactTargetParameterName  = "target";
        protected const String FactUidFieldName         = "Uid";
        protected const String FactUidParameterName     = "uid";
        protected const String PublisherFieldName       = "__publisher";
        protected const String ReturnValueVariableName  = "retVal";

        private Boolean __begin;


        protected static String ImplGetExecVerbFactName ( String verbName )
            => "ExecVerb" + verbName + "Fact";
        protected override void ImplAddMember ( MemberDeclarationSyntax member )
        {
            if ( __begin )
                foreach ( MemberDeclarationSyntax factDecl in _FactsDeclarations () )
                    base.ImplAddMember ( factDecl );

            base.ImplAddMember ( member );
        }

        protected override IEnumerable<FieldDeclarationSyntax> ImplFieldsDeclarations ()
        {
            foreach ( FieldDeclarationSyntax fldDeclStx in base.ImplFieldsDeclarations () )
                yield return fldDeclStx;

            Type fldType;

            AddReferredAssembly ( fldType = typeof ( IFactPublisher ) );
            yield return SyntaxHelper.PrivateFieldDeclaration ( fldType, PublisherFieldName );

            yield break;
        }

        protected override ClassDeclarationSyntax ImplGenerate<ContractType, ImplementationType> (
            ContractType            contractType,
            ImplementationType      implementationType,
            DistinctList<Assembly>  referredAssemblies,
            String                  proxyClassName )
        {
            __begin = true;

            return base.ImplGenerate ( contractType, implementationType, referredAssemblies, proxyClassName );
        }

        protected override IEnumerable<StatementSyntax> ImplMethodStatementSet ( MethodInfo methInfo )
        {
            IEnumerable<String> _Arguments ()
            {
                foreach ( ParameterInfo pi in methInfo.GetParameters () )
                    yield return pi.Name;


                yield break;
            }

            IEnumerable<ArgumentSyntax> _FactArguments ()
            {
                yield return SyntaxFactory.Argument ( SyntaxFactory.ThisExpression () );

                foreach ( String methArg in _Arguments () )
                    yield return SyntaxFactory.Argument ( SyntaxFactory.IdentifierName ( methArg ) );

                yield break;
            }


            yield return SyntaxFactory.LocalDeclarationStatement (
                            SyntaxHelper.VariableDeclaration (
                                methInfo.ReturnType, ReturnValueVariableName ) );

            yield return SyntaxFactory.ExpressionStatement (
                            SyntaxFactory.AssignmentExpression (
                                SyntaxKind.SimpleAssignmentExpression,
                                SyntaxFactory.IdentifierName ( ReturnValueVariableName ),
                                SyntaxHelper.InvocationExpression (
                                    methInfo.Name,
                                    _Arguments (),
                                    ProxiedObjectFieldName ) ) );


            ObjectCreationExpressionSyntax factCreationStx;

            factCreationStx = SyntaxFactory.ObjectCreationExpression (
                                    SyntaxFactory.Token ( SyntaxKind.NewKeyword ),
                                    SyntaxFactory.IdentifierName (
                                        ImplGetExecVerbFactName ( methInfo.Name ) ),
                                    SyntaxHelper.ArgumentList ( _FactArguments () ),
                                    null );
            ArgumentSyntax argStx = SyntaxFactory.Argument ( factCreationStx );

            yield return SyntaxFactory.IfStatement (
                            SyntaxFactory.BinaryExpression (
                                SyntaxKind.LogicalAndExpression,
                                SyntaxFactory.IdentifierName ( ReturnValueVariableName ),
                                SyntaxFactory.BinaryExpression (
                                    SyntaxKind.NotEqualsExpression,
                                    SyntaxHelper.IdentifierName ( PublisherFieldName ),
                                    SyntaxHelper.NullLiteralExpression ) ),
                            SyntaxFactory.ExpressionStatement (
                                SyntaxFactory.InvocationExpression (
                                    SyntaxHelper.IdentifierName (
                                        PublisherFieldName,
                                        nameof ( IFactPublisher.Publish ) ),
                                    SyntaxHelper.ArgumentList ( argStx) ) )  );

            yield return SyntaxFactory.ReturnStatement (
                SyntaxFactory.IdentifierName ( ReturnValueVariableName ) );

            yield break;
        }

        protected override IEnumerable<StatementSyntax> ImplSpConstructorStatementSet ()
        {
            foreach ( StatementSyntax stmtStx in base.ImplSpConstructorStatementSet () )
                yield return stmtStx;

            yield return SyntaxFactory.IfStatement (
                            SyntaxFactory.BinaryExpression (
                                SyntaxKind.EqualsExpression,
                                SyntaxHelper.IdentifierName ( SpConstructorParameterName ),
                                SyntaxHelper.NullLiteralExpression ),
                            SyntaxFactory.ReturnStatement () );

            yield return SyntaxFactory.ExpressionStatement (
                            SyntaxFactory.AssignmentExpression (
                                SyntaxKind.SimpleAssignmentExpression,
                                SyntaxHelper.IdentifierName ( PublisherFieldName ),
                                SyntaxFactory.BinaryExpression (
                                    SyntaxKind.AsExpression,
                                    SyntaxFactory.InvocationExpression (
                                        SyntaxHelper.IdentifierName (
                                            SpConstructorParameterName,
                                            nameof ( IServiceProvider.GetService ) ),
                                            SyntaxHelper.ArgumentList (
                                                SyntaxFactory.Argument (
                                                    SyntaxFactory.TypeOfExpression (
                                                        SyntaxHelper.Type (
                                                            typeof ( IFactPublisher ) ) ) ) ) ),
                                    SyntaxHelper.Type ( typeof (IFactPublisher) ) ) ) );

            yield break;
        }


        private IEnumerable<MemberDeclarationSyntax> _FactsDeclarations ()
        {
            __begin = false;

            ConstructorDeclarationSyntax    constrDeclStx;
            Boolean                         isUidContract;

            isUidContract = typeof ( IUid ).IsAssignableFrom ( ImplObjectContract );

            foreach ( MethodInfo methInfo in ImplObjectContract.GetMethods ()
                                                .RemovePropertiesAccessors () )
            {
                Type                    execDescrType;
                MethodDeclarationSyntax execMeth;
                MethodInfo              execMethInfo;

                IEnumerable<Tuple<Type, String>> _ConstructorParameterSet ( Boolean uidOnHead )
                {
                    if ( methInfo == null ) yield break;

                    if ( uidOnHead )
                    {
                        AddReferredAssembly ( typeof ( IUid ) );
                        yield return Tuple.Create ( typeof ( IUid ), FactUidParameterName );
                    }

                    foreach ( ParameterInfo paramInfo in methInfo.GetParameters () )
                    {
                        AddReferredAssembly ( paramInfo.ParameterType );
                        yield return Tuple.Create ( paramInfo.ParameterType, paramInfo.Name );
                    }

                    yield break;
                }

                IEnumerable<StatementSyntax> _ConstructorStatementSet ()
                {
                    if ( methInfo == null ) yield break;

                    //if ( typeof ( IUid ).IsAssignableFrom ( ImplObjectContract ) )
                    //{
                    //    AddReferredAssembly ( typeof ( UId ) );
                    //    yield return SyntaxFactory.ExpressionStatement (
                    //                    SyntaxFactory.AssignmentExpression (
                    //                        SyntaxKind.SimpleAssignmentExpression,
                    //                        SyntaxFactory.MemberAccessExpression (
                    //                            SyntaxKind.SimpleMemberAccessExpression,
                    //                            SyntaxFactory.ThisExpression (),
                    //                            SyntaxFactory.IdentifierName (
                    //                                                FactUidFieldName ) ),
                    //                        SyntaxFactory.ObjectCreationExpression (
                    //                            SyntaxFactory.Token ( SyntaxKind.NewKeyword ),
                    //                            SyntaxHelper.Type ( typeof ( UId ) ),
                    //                            SyntaxHelper.ArgumentList ( FactUidParameterName ),
                    //                            null )
                    //                    ) );
                    //}

                    foreach ( ParameterInfo paramInfo in methInfo.GetParameters () )
                        yield return SyntaxFactory.ExpressionStatement (
                                       SyntaxFactory.AssignmentExpression (
                                           SyntaxKind.SimpleAssignmentExpression,
                                            SyntaxFactory.MemberAccessExpression (
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                SyntaxFactory.ThisExpression (),
                                                SyntaxFactory.IdentifierName ( paramInfo.Name ) ),
                                           SyntaxFactory.IdentifierName ( paramInfo.Name )
                                       ) );

                    yield break;
                }

                IEnumerable<BaseTypeSyntax> _BaseTypeSet ( Boolean isUidFact )
                {
                    Type baseFactType;

                    baseFactType = isUidFact
                                ? typeof ( ExecEntityVerbFact<> )
                                : typeof ( ExecVerbFact<> );

                    AddReferredAssembly ( baseFactType );

                    yield return SyntaxFactory.SimpleBaseType (
                                    SyntaxHelper.Type ( baseFactType, ImplObjectContract ) );

                    yield break;
                }

                ConstructorInitializerSyntax _BaseInitSet ( String descrClassName )
                {
                    IEnumerable<ArgumentSyntax> _BaseArguments  ()
                    {
                        IEnumerable<ArgumentSyntax> _FactArguments ()
                        {
                            foreach ( Tuple<Type, String> prm in _ConstructorParameterSet ( false ) )
                                yield return SyntaxFactory.Argument (
                                    SyntaxFactory.IdentifierName ( prm.Item2 ) );

                            yield break;
                        }


                        yield return SyntaxFactory.Argument (
                                        SyntaxFactory.ObjectCreationExpression (
                                            SyntaxFactory.Token ( SyntaxKind.NewKeyword ),
                                            SyntaxFactory.IdentifierName ( descrClassName ),
                                            SyntaxHelper.ArgumentList ( _FactArguments () ),
                                            null ) );

                        if ( isUidContract )
                            yield return SyntaxFactory.Argument (
                                SyntaxFactory.IdentifierName ( "uid" ) );

                        yield break;
                    }

                    return SyntaxFactory.ConstructorInitializer (
                                SyntaxKind.BaseConstructorInitializer,
                                SyntaxHelper.ArgumentList ( _BaseArguments () ) );

                }

                IEnumerable<BaseTypeSyntax> _ParamBaseTypeSet ( String typeName )
                {
                    yield return SyntaxFactory.SimpleBaseType (
                                    SyntaxFactory.IdentifierName ( typeName ) );

                    yield break;
                }

                IEnumerable<FieldDeclarationSyntax> _ParamsDataDecl ()
                {
                    if ( methInfo == null ) yield break;

                    //if ( typeof ( IUid ).IsAssignableFrom ( ImplObjectContract ) )
                    //    yield return SyntaxHelper.PublicFieldDeclaration (
                    //                                typeof ( IUid ), FactUidFieldName );

                    foreach ( ParameterInfo paramInfo in methInfo.GetParameters () )
                        yield return SyntaxHelper.PublicFieldDeclaration (
                                        paramInfo.ParameterType, paramInfo.Name );

                    yield break;
                }

                IEnumerable<StatementSyntax> _ParamsExecMethodStatementSet ()
                {
                    IEnumerable<String> _Arguments ()
                    {
                        foreach ( ParameterInfo pi in methInfo.GetParameters () )
                            yield return pi.Name;

                        yield break;
                    }

                    String                      factTargetPrmName = FactTargetParameterName;
                    InvocationExpressionSyntax  invkExprStx;

                    foreach ( ParameterInfo pi in execMethInfo.GetParameters () )
                        if ( pi.ParameterType == ImplObjectContract ) {
                            factTargetPrmName = pi.Name; break; }

                    //yield return SyntaxFactory.IfStatement (
                    //                SyntaxFactory.BinaryExpression (
                    //                    SyntaxKind.EqualsExpression,
                    //                    SyntaxHelper.IdentifierName ( factTargetPrmName ),
                    //                    SyntaxHelper.NullLiteralExpression ),
                    //                SyntaxHelper.ReturnFalse );

                    //yield return SyntaxFactory.IfStatement (
                    //                SyntaxFactory.BinaryExpression (
                    //                    SyntaxKind.EqualsExpression,
                    //                    SyntaxHelper.IdentifierName ( FactUidFieldName ),
                    //                    SyntaxHelper.NullLiteralExpression ),
                    //                SyntaxHelper.ReturnFalse );

                    //yield return SyntaxFactory.IfStatement (
                    //                SyntaxFactory.BinaryExpression (
                    //                    SyntaxKind.NotEqualsExpression,
                    //                    SyntaxHelper.IdentifierName ( FactUidFieldName ),
                    //                    SyntaxHelper.IdentifierName ( factTargetPrmName ) ),
                    //                SyntaxHelper.ReturnFalse );

                    invkExprStx = SyntaxHelper.InvocationExpression (
                                            methInfo.Name, _Arguments (), factTargetPrmName );

                    if ( methInfo.ReturnType == typeof ( Boolean ) )
                        yield return SyntaxFactory.ReturnStatement ( invkExprStx );
                    else
                    {
                        yield return SyntaxFactory.ExpressionStatement ( invkExprStx );
                        yield return SyntaxHelper.ReturnTrue;
                        //yield return SyntaxFactory.ReturnStatement (
                        //                SyntaxFactory.LiteralExpression (
                        //                    SyntaxKind.TrueLiteralExpression ) );
                    }

                    yield break;
                }


                if ( methInfo.GetCustomAttribute<ProvideExecVerbFactAttribute> () == null )
                    continue;

                execDescrType = typeof ( ExecVerbFact<>.ExecDescr )
                                    .MakeGenericType ( ImplObjectContract );
                execMethInfo = execDescrType.GetMethod (
                                    "ImplExec", BindingFlags.Instance | BindingFlags.NonPublic );

                execMeth = SyntaxHelper.MethodDeclaration (
                                execDescrType.GetMethod ("ImplExec", BindingFlags.Instance | BindingFlags.NonPublic ),
                                _ParamsExecMethodStatementSet () );

                constrDeclStx = SyntaxHelper.PublicConstructorDeclaration (
                                    "Params1",
                                    _ConstructorStatementSet (),
                                    _ConstructorParameterSet ( false ) );

                ClassDeclarationSyntax classDecl = SyntaxFactory.ClassDeclaration ( "Params1" )
                    .WithModifiers ( SyntaxHelper.PublicModifier )
                    .WithBaseList ( SyntaxFactory.BaseList (
                                        _ParamBaseTypeSet (
                                                nameof ( ExecVerbFact<Object>.ExecDescr ) )
                                            .SeparatedList () ))
                    .AddMembers ( _ParamsDataDecl ().ToArray () )
                    .AddMembers ( constrDeclStx, execMeth );

                constrDeclStx = SyntaxHelper.PublicConstructorDeclaration (
                                    ImplGetExecVerbFactName ( methInfo.Name ),
                                    null,
                                    _ConstructorParameterSet ( isUidContract ) )
                                .WithInitializer ( _BaseInitSet ( "Params1" ) );
                                        //SyntaxFactory.ConstructorInitializer (
                                        //    SyntaxKind.BaseConstructorInitializer,
                                        //    SyntaxHelper.ArgumentList ( "uid") ) );

                yield return SyntaxFactory.ClassDeclaration (
                                    ImplGetExecVerbFactName ( methInfo.Name ) )
                    .WithModifiers ( SyntaxHelper.PublicModifier )
                    .WithBaseList ( SyntaxFactory.BaseList (
                                        _BaseTypeSet ( isUidContract ).SeparatedList () ) )
                    .AddMembers ( classDecl, constrDeclStx );

            }

            yield break;
        }
    }
}
