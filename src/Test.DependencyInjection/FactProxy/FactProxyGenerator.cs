using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Sharpframework.Core;
using Sharpframework.EntityModel;
//using Sharpframework.EntityModel.Implementation;
using Sharpframework.Roslyn.CSharp;
using Sharpframework.Roslyn.DynamicProxy;
using Sharpframework.Propagation.Facts;


namespace Test.DependencyInjection.FactProxy
{
    public class FactProxyGenerator
        : ProxyGenerator
    {
        protected const String DispatcherFieldName      = "__dispatcher";
        protected const String FactTargetParameterName  = "target";
        protected const String FactUidFieldName         = "Uid";
        protected const String FactUidParameterName     = "uid";
        protected const String PublisherFieldName       = "__publisher";
        protected const String ReturnValueVariableName  = "retVal";
        protected const String SenderParameterName      = "sender";

        private DistinctList<String>    __factProviderVerbs;


        protected static String ImplGetExecVerbFactName ( String verbName )
            => "ExecVerb" + verbName + "Fact";


        protected virtual IReadOnlyCollection<String> ImplFactProviderVerbs
        {
            get
            {
                if ( __factProviderVerbs == null )
                    __factProviderVerbs = new DistinctList<String> ();

                if ( __factProviderVerbs.Count < 1 )
                    foreach ( MethodInfo mi in ImplContractMethods )
                        if ( mi.GetCustomAttribute<ProvideExecVerbFactAttribute> ( true ) != null )
                            __factProviderVerbs.Add ( mi.Name );

                return __factProviderVerbs;
            }
        }

        private NameSyntax _FactConsumerInterfaceStx ( String factVerb )
        {
            TypeSyntax  factType;
            SyntaxToken factConsumer;
            NameSyntax  nspcStx;

            nspcStx = SyntaxHelper.IdentifierName (
                                typeof ( IFactConsumer<> ).Namespace );

            factConsumer = SyntaxFactory.Identifier (
                                typeof ( IFactConsumer<> ).Name.Split ( '`' ) [0] );

            factType = SyntaxFactory.QualifiedName (
                SyntaxFactory.IdentifierName ( ImplProxyClassName ),
                SyntaxFactory.IdentifierName (
                    ImplGetExecVerbFactName ( factVerb ) ) );

            return SyntaxFactory.QualifiedName (
                    nspcStx,
                    SyntaxFactory.GenericName (
                        factConsumer,
                        SyntaxFactory.TypeArgumentList ( SyntaxFactory.List<TypeSyntax> ().Add ( factType ).SeparatedList () ) ) );
        }

        protected override IEnumerable<BaseTypeSyntax> ImplProxyBaseTypes
        {
            get
            {
                foreach ( BaseTypeSyntax baseType in base.ImplProxyBaseTypes )
                    yield return baseType;

                ImplAddReferredAssembly ( typeof ( IFactConsumer<> ) );

                foreach ( String factProviderVerb in ImplFactProviderVerbs )
                    yield return SyntaxFactory.SimpleBaseType (
                                        _FactConsumerInterfaceStx ( factProviderVerb ) );

                yield break;
            }
        }


        protected override void ImplAddExplicitInterfaceImplementations ()
        {
            MethodDeclarationSyntax methDeclStx;

            foreach ( String factVerb in ImplFactProviderVerbs )
            {
                StatementSyntax     stmtStx;
                ParameterSyntax     prmStx;

                //stmtStx = SyntaxFactory.ExpressionStatement (
                //                SyntaxFactory.InvocationExpression (
                //                    SyntaxFactory.QualifiedName (
                //                        SyntaxFactory.IdentifierName ( "fact" ),
                //                        SyntaxFactory.IdentifierName ( "Exec" ) ),
                //                    SyntaxFactory.ArgumentList (
                //                        SyntaxFactory.Argument (
                //                            SyntaxFactory.IdentifierName ( ProxiedObjectFieldName )
                //                            ).ToEnumerable ().SeparatedList () ) ) );
                // fact.Exec ( <ProxiedObjectFieldName> );
                stmtStx = SyntaxHelper.MethodInvocation ( "fact.Exec" ) // Or "fact", "Exec"
                                    .AddArgument ( ProxiedObjectFieldName, true );

                prmStx = SyntaxHelper.Parameter (
                            SyntaxHelper.IdentifierName (
                                ImplGetExecVerbFactName ( factVerb ) ), "fact" );


                methDeclStx = SyntaxHelper.MethodDeclaration (
                                    default ( SyntaxTokenList ),
                                    typeof ( void ),
                                    nameof ( IFactConsumer<IFact>.Consume ),
                                    prmStx.ToEnumerable (),
                                    stmtStx.ToEnumerable (),
                                    SyntaxFactory.ExplicitInterfaceSpecifier (
                                        _FactConsumerInterfaceStx ( factVerb ) )
                                    );

                ImplAddMember ( methDeclStx );
            }

            base.ImplAddExplicitInterfaceImplementations ();
        }

        protected override void ImplAddNestedTypes ()
        {
            foreach ( MemberDeclarationSyntax factDecl in _FactsDeclarations () )
                base.ImplAddMember ( factDecl );

            base.ImplAddNestedTypes ();
        }

        protected override void ImplClear ()
        {
            if ( __factProviderVerbs != null ) __factProviderVerbs.Clear ();

            base.ImplClear ();
        }

        protected override IEnumerable<StatementSyntax> ImplDisposeStatementSet ()
        {
            IEnumerable<StatementSyntax> _ClearDispatch ()
            {
                using ( IEnumerator<String> factVerbEnum = ImplFactProviderVerbs.GetEnumerator () )
                {
                    IEnumerable<StatementSyntax> _UnSubscribeStatementSet ()
                    {
                        do
                            yield return SyntaxFactory.ExpressionStatement (
                                                SyntaxFactory.InvocationExpression (
                                                    SyntaxFactory.QualifiedName (
                                                        SyntaxFactory.IdentifierName (
                                                            DispatcherFieldName ),
                                                        SyntaxFactory.IdentifierName (
                                                            nameof ( IFactDispatcher.UnSubscribe )
                                                    ) ),
                                                    SyntaxFactory.ArgumentList (
                                                        SyntaxFactory.Argument (
                                                            SyntaxFactory.BinaryExpression (
                                                                SyntaxKind.AsExpression,
                                                                SyntaxFactory.ThisExpression (),
                                                                SyntaxHelper.Type (
                                                                    typeof ( IFactConsumer<> ),
                                                                    ImplGetExecVerbFactName (
                                                                        factVerbEnum.Current ) )
                                                                ) ).ToEnumerable ()
                                                                .SeparatedList () )
                                                      ) );
                        while ( factVerbEnum.MoveNext () );

                        //yield return SyntaxFactory.ExpressionStatement (
                        //                SyntaxFactory.AssignmentExpression (
                        //                    SyntaxKind.SimpleAssignmentExpression,
                        //                    SyntaxHelper.IdentifierName ( DispatcherFieldName ),
                        //                    SyntaxHelper.LiteralNull ) );
                        // <DispatcherFieldName> = null;
                        yield return SyntaxHelper.NullAssignment ( DispatcherFieldName );

                        yield break;
                    }


                    if ( factVerbEnum.MoveNext () )
                        yield return SyntaxFactory.IfStatement (
                                        SyntaxFactory.BinaryExpression (
                                            SyntaxKind.NotEqualsExpression,
                                            SyntaxFactory.IdentifierName (
                                                            DispatcherFieldName ),
                                            SyntaxHelper.LiteralNull ),
                                        SyntaxFactory.Block ( _UnSubscribeStatementSet () ) );
                    else
                        //yield return SyntaxFactory.ExpressionStatement (
                        //                SyntaxFactory.AssignmentExpression (
                        //                    SyntaxKind.SimpleAssignmentExpression,
                        //                    SyntaxHelper.IdentifierName ( DispatcherFieldName ),
                        //                    SyntaxHelper.LiteralNull ) );
                        // <DispatcherFieldName> = null;
                        yield return SyntaxHelper.NullAssignment ( DispatcherFieldName );
                }

                //yield return SyntaxFactory.ExpressionStatement (
                //                SyntaxFactory.AssignmentExpression (
                //                    SyntaxKind.SimpleAssignmentExpression,
                //                    SyntaxHelper.IdentifierName ( PublisherFieldName ),
                //                    SyntaxHelper.LiteralNull ) );
                // <PublisherFieldName> = null;
                yield return SyntaxHelper.NullAssignment ( PublisherFieldName );

                yield break;
            }


            foreach ( StatementSyntax stmtStx in _ClearDispatch () )
                yield return stmtStx;

            foreach ( StatementSyntax stmtStx in base.ImplDisposeStatementSet () )
                yield return stmtStx;

            yield break;
        }

        protected override IEnumerable<FieldDeclarationSyntax> ImplFieldsDeclarations ()
        {
            foreach ( FieldDeclarationSyntax fldDeclStx in base.ImplFieldsDeclarations () )
                yield return fldDeclStx;

            Type fldType;

            ImplAddReferredAssembly ( fldType = typeof ( IFactPublisher ) );
            yield return SyntaxHelper.PrivateFieldDeclaration ( fldType, PublisherFieldName );

            ImplAddReferredAssembly ( fldType = typeof ( IFactDispatcher ) );
            yield return SyntaxHelper.PrivateFieldDeclaration ( fldType, DispatcherFieldName );

            yield break;
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
                yield return SyntaxFactory.Argument (
                                SyntaxFactory.IdentifierName ( ProxiedObjectFieldName ) );

                yield return SyntaxFactory.Argument (
                                SyntaxFactory.IdentifierName ( ProxiedObjectFieldName ) );

                foreach ( String methArg in _Arguments () )
                    yield return SyntaxFactory.Argument ( SyntaxFactory.IdentifierName ( methArg ) );

                yield break;
            }


            yield return SyntaxFactory.LocalDeclarationStatement (
                            SyntaxHelper.VariableDeclaration (
                                methInfo.ReturnType, ReturnValueVariableName ) );

            //yield return SyntaxFactory.ExpressionStatement (
            //                SyntaxFactory.AssignmentExpression (
            //                    SyntaxKind.SimpleAssignmentExpression,
            //                    SyntaxFactory.IdentifierName ( ReturnValueVariableName ),
            //                    SyntaxHelper.InvocationExpression (
            //                        methInfo.Name,
            //                        _Arguments (),
            //                        ProxiedObjectFieldName ) ) );
            // <ProxiedObjectFieldName>.<methInfo.Name> ( <Method Info Parameters> )
            yield return SyntaxHelper.Assignment (
                                ReturnValueVariableName,
                                SyntaxHelper.MethodInvocation ( ProxiedObjectFieldName,
                                                                methInfo.Name )
                                    .AddArguments ( methInfo.GetParameters () ) );


            ObjectCreationExpressionSyntax factCreationStx;

            factCreationStx = SyntaxFactory.ObjectCreationExpression (
                                    SyntaxFactory.Token ( SyntaxKind.NewKeyword ),
                                    SyntaxFactory.IdentifierName (
                                        ImplGetExecVerbFactName ( methInfo.Name ) ),
                                    SyntaxHelper.ArgumentList ( _FactArguments () ),
                                    null );
            ArgumentSyntax argStx = SyntaxFactory.Argument ( factCreationStx );

            // if ( retVal && __publisher != null ) __publisher.Publish (...)
            yield return SyntaxFactory.IfStatement (
                            SyntaxFactory.BinaryExpression (
                                SyntaxKind.LogicalAndExpression,
                                SyntaxFactory.IdentifierName ( ReturnValueVariableName ),
                                SyntaxFactory.BinaryExpression (
                                    SyntaxKind.NotEqualsExpression,
                                    SyntaxHelper.IdentifierName ( PublisherFieldName ),
                                    SyntaxHelper.LiteralNull ) ),
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

        protected override IEnumerable<StatementSyntax> ImplFullConstructorStatementSet ()
        {
            IEnumerable<StatementSyntax> _ClearFieldsAndReturn ()
            {
                //yield return SyntaxFactory.ExpressionStatement (
                //                SyntaxFactory.AssignmentExpression (
                //                    SyntaxKind.SimpleAssignmentExpression,
                //                    SyntaxFactory.IdentifierName ( DispatcherFieldName ),
                //                    SyntaxHelper.LiteralNull ) );

                //yield return SyntaxFactory.ExpressionStatement (
                //                SyntaxFactory.AssignmentExpression (
                //                    SyntaxKind.SimpleAssignmentExpression,
                //                    SyntaxFactory.IdentifierName ( PublisherFieldName ),
                //                    SyntaxHelper.LiteralNull ) );

                // <DispatcherFieldName> = null;
                yield return SyntaxHelper.NullAssignment ( DispatcherFieldName );

                // <PublisherFieldName> = null;
                yield return SyntaxHelper.NullAssignment ( PublisherFieldName );

                yield return SyntaxFactory.ReturnStatement ();

                yield break;
            }

            foreach ( StatementSyntax stmtStx in base.ImplFullConstructorStatementSet () )
                yield return stmtStx;

            // if ( sp == null ) ...
            yield return SyntaxFactory.IfStatement (
                            SyntaxFactory.BinaryExpression (
                                SyntaxKind.EqualsExpression,
                                SyntaxHelper.IdentifierName ( SpConstructorParameterName ),
                                SyntaxHelper.LiteralNull ),
                            SyntaxFactory.Block ( _ClearFieldsAndReturn () ) );

            // __dispatcher = sp.GetService ( typeof ( IFactDispatcher ) ) as IFactDispatcher
            yield return SyntaxFactory.ExpressionStatement (
                            SyntaxFactory.AssignmentExpression (
                                SyntaxKind.SimpleAssignmentExpression,
                                SyntaxHelper.IdentifierName ( DispatcherFieldName ),
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
                                                            typeof ( IFactDispatcher ) ) ) ) ) ),
                                    SyntaxHelper.Type ( typeof ( IFactDispatcher ) ) ) ) );

            // __publisher = sp.GetService ( typeof ( IFactPublisher ) ) as IFactPublisher
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
                                    SyntaxHelper.Type ( typeof ( IFactPublisher ) ) ) ) );

            yield return SyntaxFactory.IfStatement (
                            SyntaxFactory.BinaryExpression (
                                SyntaxKind.EqualsExpression,
                                SyntaxHelper.IdentifierName ( DispatcherFieldName ),
                                SyntaxHelper.LiteralNull ),
                            SyntaxFactory.ReturnStatement () );

            foreach ( String factVerb in ImplFactProviderVerbs )
                yield return SyntaxFactory.ExpressionStatement (
                                    SyntaxFactory.InvocationExpression (
                                        SyntaxFactory.QualifiedName (
                                            SyntaxFactory.IdentifierName ( DispatcherFieldName ),
                                            SyntaxFactory.IdentifierName (
                                                nameof ( IFactDispatcher.Subscribe ) ) ),
                                        SyntaxFactory.ArgumentList (
                                            SyntaxFactory.Argument (
                                                SyntaxFactory.BinaryExpression (
                                                    SyntaxKind.AsExpression,
                                                    SyntaxFactory.ThisExpression (),
                                                    SyntaxHelper.Type ( typeof ( IFactConsumer<> ),
                                                        ImplGetExecVerbFactName ( factVerb ) )
                                                    ) ) .ToEnumerable ().SeparatedList () ) 
                                          ) );

            yield break;
        }


        private IEnumerable<MemberDeclarationSyntax> _FactsDeclarations ()
        {
            ConstructorDeclarationSyntax    constrDeclStx;
            Boolean                         isUidContract;

            isUidContract = typeof ( IUid ).IsAssignableFrom ( ImplObjectContract );

            foreach ( MethodInfo methInfo in ImplObjectContract.GetMethods ()
                                                .RemovePropertiesAccessors () )
            {
                Type                    execDescrType;
                MethodDeclarationSyntax execMeth;
                MethodInfo              execMethInfo;

                ConstructorInitializerSyntax _BaseInitSet ( String descrClassName )
                {
                    IEnumerable<ArgumentSyntax> _BaseArguments  ()
                    {
                        IEnumerable<ArgumentSyntax> _FactArguments ()
                        {
                            foreach ( Tuple<Type, String> prm in _DescrConstructorParameterSet () )
                                yield return SyntaxFactory.Argument (
                                    SyntaxFactory.IdentifierName ( prm.Item2 ) );

                            yield break;
                        }


                        yield return SyntaxFactory.Argument (
                            SyntaxFactory.IdentifierName ( SenderParameterName ) );

                        if ( isUidContract )
                            yield return SyntaxFactory.Argument (
                                SyntaxFactory.IdentifierName ( "uid" ) );

                        yield return SyntaxFactory.Argument (
                                        SyntaxFactory.ObjectCreationExpression (
                                            SyntaxFactory.Token ( SyntaxKind.NewKeyword ),
                                            SyntaxFactory.IdentifierName ( descrClassName ),
                                            SyntaxHelper.ArgumentList ( _FactArguments () ),
                                            null ) );

                        yield break;
                    }

                    return SyntaxFactory.ConstructorInitializer (
                                SyntaxKind.BaseConstructorInitializer,
                                SyntaxHelper.ArgumentList ( _BaseArguments () ) );

                }

                IEnumerable<BaseTypeSyntax> _BaseTypeSet ( Boolean isUidFact )
                {
                    Type baseFactType;

                    baseFactType = isUidFact
                                ? typeof ( ExecEntityVerbFact<> )
                                : typeof ( ExecVerbFact<> );

                    ImplAddReferredAssembly ( baseFactType );

                    yield return SyntaxFactory.SimpleBaseType (
                                    SyntaxHelper.Type ( baseFactType, ImplObjectContract ) );


                    yield break;
                }

                IEnumerable<Tuple<Type, String>> _ConstructorParameterSet ()
                {
                    if ( methInfo == null ) yield break;

                    yield return Tuple.Create ( typeof ( Object ), SenderParameterName );

                    if ( isUidContract )
                    {
                        ImplAddReferredAssembly ( typeof ( IUid ) );
                        yield return Tuple.Create ( typeof ( IUid ), FactUidParameterName );
                    }

                    foreach ( Tuple<Type, String> item in _DescrConstructorParameterSet () )
                        yield return item;

                    yield break;
                }

                IEnumerable<StatementSyntax> _ConstructorStatementSet ()
                {
                    if ( methInfo == null ) yield break;

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

                IEnumerable<Tuple<Type, String>> _DescrConstructorParameterSet ()
                {
                    if ( methInfo == null ) yield break;

                    foreach ( ParameterInfo paramInfo in methInfo.GetParameters () )
                    {
                        ImplAddReferredAssembly ( paramInfo.ParameterType );
                        yield return Tuple.Create ( paramInfo.ParameterType, paramInfo.Name );
                    }

                    yield break;
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

                    invkExprStx = SyntaxHelper.InvocationExpression (
                                            methInfo.Name, _Arguments (), factTargetPrmName );

                    if ( methInfo.ReturnType == typeof ( Boolean ) )
                        yield return SyntaxFactory.ReturnStatement ( invkExprStx );
                    else
                    {
                        yield return SyntaxFactory.ExpressionStatement ( invkExprStx );
                        yield return SyntaxHelper.ReturnTrue;
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
                                    _DescrConstructorParameterSet () );

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
                                    _ConstructorParameterSet () )
                                .WithInitializer ( _BaseInitSet ( "Params1" ) );

                yield return SyntaxFactory.ClassDeclaration (
                                    ImplGetExecVerbFactName ( methInfo.Name ) )
                    .WithModifiers ( SyntaxHelper.PublicModifier )
                    .WithBaseList ( SyntaxFactory.BaseList (
                                        _BaseTypeSet ( isUidContract ).SeparatedList () ) )
                    .AddMembers ( classDecl, constrDeclStx );

            }

            yield break;
        }

        //protected override ClassDeclarationSyntax ImplDeclareProxyClass ()
        //{
        //    MethodDeclarationSyntax methDeclStx;

        //    foreach ( String factVerb in ImplFactProviderVerbs )
        //    {
        //        StatementSyntax     stmtStx;
        //        ParameterSyntax     prmStx;

        //        stmtStx = SyntaxFactory.ExpressionStatement (
        //                        SyntaxFactory.InvocationExpression (
        //                            SyntaxFactory.QualifiedName (
        //                                SyntaxFactory.IdentifierName ( "fact" ),
        //                                SyntaxFactory.IdentifierName ( "Exec" ) ),
        //                            SyntaxFactory.ArgumentList (
        //                                SyntaxFactory.Argument (
        //                                    SyntaxFactory.IdentifierName ( ProxiedObjectFieldName )
        //                                    ).ToEnumerable ().SeparatedList () ) ) );

        //        prmStx = SyntaxHelper.Parameter (
        //                    SyntaxHelper.IdentifierName (
        //                        ImplGetExecVerbFactName ( factVerb ) ), "fact" );


        //        methDeclStx = SyntaxHelper.MethodDeclaration (
        //                            default ( SyntaxTokenList ),
        //                            typeof ( void ),
        //                            nameof ( IFactConsumer<IFact>.Consume ),
        //                            prmStx.ToEnumerable (),
        //                            stmtStx.ToEnumerable (),
        //                            SyntaxFactory.ExplicitInterfaceSpecifier (
        //                                _FactConsumerInterfaceStx ( factVerb ) )
        //                            );

        //        ImplAddMember ( methDeclStx );
        //    }

        //    return base.ImplDeclareProxyClass ();
        //}
    }
}
