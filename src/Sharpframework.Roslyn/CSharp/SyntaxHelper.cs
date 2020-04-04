using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Sharpframework.Roslyn.CSharp
{
    public static class SyntaxHelper
    {
        private static SyntaxTokenList  __internalModifier;
        private static SyntaxToken      __internalToken;
        private static SyntaxTokenList  __privateModifier;
        private static SyntaxToken      __privateToken;
        private static SyntaxTokenList  __protectedModifier;
        private static SyntaxToken      __protectedToken;
        private static SyntaxTokenList  __publicModifier;
        private static SyntaxToken      __publicToken;


        static SyntaxHelper ()
        {
            __internalModifier  = default ( SyntaxTokenList );
            __internalToken     = default ( SyntaxToken );
            __privateModifier   = default ( SyntaxTokenList );
            __privateToken      = default ( SyntaxToken );
            __protectedModifier = default ( SyntaxTokenList );
            __protectedToken    = default ( SyntaxToken );
            __publicModifier    = default ( SyntaxTokenList );
            __publicToken       = default ( SyntaxToken );
        }

        public static SyntaxToken InternalToken
        {
            get
            {
                if ( __internalToken == default ( SyntaxToken ) )
                    __internalToken = SyntaxFactory.Token ( SyntaxKind.InternalKeyword );

                return __internalToken;
            }
        }


        public static SyntaxTokenList InternalModifier
        {
            get
            {
                if ( __internalModifier == default ( SyntaxTokenList ) )
                    __internalModifier = SyntaxTokenList.Create ( InternalToken );

                return __internalModifier;
            }
        }

        public static LiteralExpressionSyntax LiteralFalse
        { get => SyntaxFactory.LiteralExpression ( SyntaxKind.FalseLiteralExpression ); }

        public static LiteralExpressionSyntax LiteralTrue
        { get => SyntaxFactory.LiteralExpression ( SyntaxKind.TrueLiteralExpression ); }

        public static LiteralExpressionSyntax LiteralNull
        { get => SyntaxFactory.LiteralExpression ( SyntaxKind.NullLiteralExpression ); }

        public static SyntaxToken PrivateToken
        {
            get
            {
                if ( __privateToken == default ( SyntaxToken ) )
                    __privateToken = SyntaxFactory.Token ( SyntaxKind.PrivateKeyword );

                return __privateToken;
            }
        }


        public static SyntaxTokenList PrivateModifier
        {
            get
            {
                if ( __privateModifier == default ( SyntaxTokenList ) )
                    __privateModifier = SyntaxTokenList.Create ( PrivateToken );

                return __privateModifier;
            }
        }

        public static SyntaxToken ProtectedToken
        {
            get
            {
                if ( __protectedToken == default ( SyntaxToken ) )
                    __protectedToken = SyntaxFactory.Token ( SyntaxKind.ProtectedKeyword );

                return __protectedToken;
            }
        }


        public static SyntaxTokenList ProtectedModifier
        {
            get
            {
                if ( __protectedModifier == default ( SyntaxTokenList ) )
                    __protectedModifier = SyntaxTokenList.Create ( ProtectedToken );

                return __protectedModifier;
            }
        }

        public static SyntaxToken PublicToken
        {
            get
            {
                if ( __publicToken == default ( SyntaxToken ) )
                    __publicToken = SyntaxFactory.Token ( SyntaxKind.PublicKeyword );

                return __publicToken;
            }
        }

        public static SyntaxTokenList PublicModifier
        {
            get
            {
                if ( __publicModifier == default ( SyntaxTokenList ) )
                    __publicModifier = SyntaxTokenList.Create ( PublicToken );

                return __publicModifier;
            }
        }

        public static ReturnStatementSyntax ReturnFalse
        { get => SyntaxFactory.ReturnStatement ( LiteralFalse ); }

        public static ReturnStatementSyntax ReturnTrue
        { get => SyntaxFactory.ReturnStatement ( LiteralTrue ); }



        public static ArgumentSyntax Argument ( String argument )
            => SyntaxFactory.Argument ( SyntaxFactory.IdentifierName ( argument ) );

        public static ArgumentSyntax Argument ()
            => SyntaxFactory.Argument ( LiteralNull );

        public static ArgumentListSyntax ArgumentList ()
            => SyntaxFactory.ArgumentList ();
        public static ArgumentListSyntax ArgumentList ( params ArgumentSyntax [] arguments )
            => SyntaxFactory.ArgumentList ( arguments.SeparatedList () );

        public static ArgumentListSyntax ArgumentList ( IEnumerable<ArgumentSyntax> arguments )
            => SyntaxFactory.ArgumentList ( arguments.SeparatedList () );

        public static ArgumentListSyntax ArgumentList ( params String [] parameters )
            => ArgumentList ( parameters as IEnumerable<String> );

        public static ArgumentListSyntax ArgumentList ( IEnumerable<String> parameters )
            => SyntaxFactory.ArgumentList ( ArgumentSet ( parameters ).SeparatedList () );

        public static IEnumerable<ArgumentSyntax> ArgumentSet (
                IEnumerable<String> parameters )
            => ConvertSet ( IdentifierSet ( parameters ), SyntaxFactory.Argument );

        public static IdentifierAssignmentBuilder Assignment ( String left, String right )
            => IdentifierAssignmentBuilder.Allocate ( left, right );

        public static IdentifierAssignmentBuilder Assignment (
            IEnumerable<String> left, String right )
                => IdentifierAssignmentBuilder.Allocate ( left, right );

        public static IdentifierAssignmentBuilder Assignment (
            String left, IEnumerable<String> right )
                => IdentifierAssignmentBuilder.Allocate ( left, right );

        public static IdentifierAssignmentBuilder Assignment (
            IEnumerable<String> left, IEnumerable<String> right )
                => IdentifierAssignmentBuilder.Allocate ( left, right );

        public static IdentifierAssignmentBuilder Assignment ( String left, NameBuilder right )
            => IdentifierAssignmentBuilder.Allocate ( left, right );

        public static IdentifierAssignmentBuilder Assignment (
            IEnumerable<String> left, NameBuilder right )
                => IdentifierAssignmentBuilder.Allocate ( left, right );

        public static ExpressionAssignmentBuilder Assignment (
                String left, ExpressionSyntax right )
            => ExpressionAssignmentBuilder.Allocate ( left, right );

        public static ExpressionAssignmentBuilder Assignment (
                IEnumerable<String> left, ExpressionSyntax right )
            => ExpressionAssignmentBuilder.Allocate ( left, right );

        public static IdentifierAssignmentBuilder Assignment ( NameBuilder left, String right )
            => IdentifierAssignmentBuilder.Allocate ( left, right );

        public static IdentifierAssignmentBuilder Assignment (
            NameBuilder left, IEnumerable<String> right )
                => IdentifierAssignmentBuilder.Allocate ( left, right );

        public static IdentifierAssignmentBuilder Assignment (
                NameBuilder left, NameBuilder right )
            => IdentifierAssignmentBuilder.Allocate ( left, right );

        public static ExpressionAssignmentBuilder Assignment (
                NameBuilder left, ExpressionSyntax right )
            => ExpressionAssignmentBuilder.Allocate ( left, right );

        public static IdentifierAssignmentBuilder Assignment (
                String left, String right, AssignmentKind kind )
            => IdentifierAssignmentBuilder.Allocate ( left, right, kind );

        public static IdentifierAssignmentBuilder Assignment (
                IEnumerable<String> left, String right, AssignmentKind kind )
            => IdentifierAssignmentBuilder.Allocate ( left, right, kind );

        public static IdentifierAssignmentBuilder Assignment (
                String left, IEnumerable<String> right, AssignmentKind kind )
            => IdentifierAssignmentBuilder.Allocate ( left, right, kind );

        public static IdentifierAssignmentBuilder Assignment (
                IEnumerable<String> left, IEnumerable<String> right, AssignmentKind kind )
            => IdentifierAssignmentBuilder.Allocate ( left, right, kind );

        public static IdentifierAssignmentBuilder Assignment (
                String left, NameBuilder right, AssignmentKind kind )
            => IdentifierAssignmentBuilder.Allocate ( left, right, kind );

        public static IdentifierAssignmentBuilder Assignment (
                IEnumerable<String> left, NameBuilder right, AssignmentKind kind )
            => IdentifierAssignmentBuilder.Allocate ( left, right, kind );

        public static ExpressionAssignmentBuilder Assignment (
                String left, ExpressionSyntax right, AssignmentKind kind )
            => ExpressionAssignmentBuilder.Allocate ( left, right, kind );

        public static ExpressionAssignmentBuilder Assignment (
                IEnumerable<String> left, ExpressionSyntax right, AssignmentKind kind )
            => ExpressionAssignmentBuilder.Allocate ( left, right, kind );

        public static IdentifierAssignmentBuilder Assignment (
                NameBuilder left, String right, AssignmentKind kind )
            => IdentifierAssignmentBuilder.Allocate ( left, right, kind );

        public static IdentifierAssignmentBuilder Assignment (
                NameBuilder left, IEnumerable<String> right, AssignmentKind kind )
            => IdentifierAssignmentBuilder.Allocate ( left, right, kind );

        public static IdentifierAssignmentBuilder Assignment (
                NameBuilder left, NameBuilder right, AssignmentKind kind )
            => IdentifierAssignmentBuilder.Allocate ( left, right, kind );

        public static ExpressionAssignmentBuilder Assignment (
                NameBuilder left, ExpressionSyntax right, AssignmentKind kind )
            => ExpressionAssignmentBuilder.Allocate ( left, right, kind );


        public static BaseListSyntax BaseList ( Boolean fullName, params Type [] baseTypes )
            => SyntaxFactory.BaseList ( BaseTypesSet ( fullName, baseTypes ).SeparatedList () );



        public static IEnumerable<BaseTypeSyntax> BaseTypesSet (
            Boolean fullName, params Type [] baseTypes )
        {
            if ( baseTypes == null ) yield break;

            if ( fullName )
                foreach ( Type baseTy in baseTypes )
                    yield return SyntaxFactory.SimpleBaseType (
                                    SyntaxFactory.ParseTypeName ( baseTy.FullName ) );
            else
                foreach ( Type baseTy in baseTypes )
                    yield return SyntaxFactory.SimpleBaseType (
                                    SyntaxFactory.ParseTypeName ( baseTy.Name ) );

            yield break;
        }

        public static ConstructorDeclarationSyntax ConstructorDeclaration (
            SyntaxKind                      modifierKind,
            String                          className,
            IEnumerable<StatementSyntax>    bodyStatementSet,
            IEnumerable<Tuple<Type, String>> parameters )
                => ConstructorDeclaration (
                        GetModifier ( modifierKind ), className, bodyStatementSet, parameters );

        public static ConstructorDeclarationSyntax ConstructorDeclaration (
            SyntaxTokenList                     modifiers,
            String                              className,
            IEnumerable<StatementSyntax>        bodyStatementSet,
            IEnumerable<Tuple<Type, String>>    parameters )
        {
            BlockSyntax         bodyStx;
            ParameterListSyntax paramListStx = ParameterList ( parameters );

            bodyStx = bodyStatementSet == null
                            ? SyntaxFactory.Block ()
                            : SyntaxFactory.Block ( bodyStatementSet );

            return SyntaxFactory.ConstructorDeclaration ( className )
                        .WithBody ( bodyStx )
                        .WithModifiers ( modifiers )
                        .WithParameterList ( paramListStx );
        }


        public static DestructorDeclarationSyntax DestructorDeclaration (
            String              className,
            StatementSyntax []  bodyStatementSet )
                => DestructorDeclaration (
                        className, bodyStatementSet as IEnumerable<StatementSyntax> );

        public static DestructorDeclarationSyntax DestructorDeclaration (
            String                          className,
            IEnumerable<StatementSyntax>    bodyStatementSet )
                => SyntaxFactory.DestructorDeclaration (
                                        default ( SyntaxList<AttributeListSyntax> ),
                                        default ( SyntaxTokenList ),
                                        SyntaxFactory.Identifier ( className ),
                                        ParameterList (),
                                        SyntaxFactory.Block ( bodyStatementSet ) );
        
        public static IEnumerable<OutputItemType> ConvertSet<OutputItemType, InputItemType> (
            IEnumerable<InputItemType>          inputSet,
            Func<InputItemType, OutputItemType> ItemConverterDlg )
        {
            if ( inputSet           == null ) yield break;
            if ( ItemConverterDlg   == null ) yield break;

            foreach ( InputItemType inputItem in inputSet )
                yield return ItemConverterDlg ( inputItem );

            yield break;
        }

        public static SyntaxTokenList EmptyTokenList ()
            => default ( SyntaxTokenList );

        public static SyntaxList<SyntaxItemType> EmptySyntaxList<SyntaxItemType> ()
            where SyntaxItemType : SyntaxNode
                => default ( SyntaxList<SyntaxItemType> );

        public static FieldDeclarationSyntax FieldDeclaration (
            SyntaxKind modifierKind, Type fieldType, params String [] fieldsNames )
        {
            return SyntaxFactory.FieldDeclaration (
                        EmptySyntaxList<AttributeListSyntax> (),
                        GetModifier ( modifierKind ),
                        VariableDeclaration ( fieldType, fieldsNames ) );
        }

        public static SyntaxTokenList GetModifier ( SyntaxKind kindStx )
        {
            switch ( kindStx )
            {
                case SyntaxKind.PrivateKeyword  : return PrivateModifier;
                case SyntaxKind.PublicKeyword   : return PublicModifier;
            }

            return default ( SyntaxTokenList );
        }

        public static NameSyntax IdentifierName ( String source )
            => IdentifierName ( source, "." );
        public static NameSyntax IdentifierName ( params String [] sources )
            => IdentifierName ( sources as IEnumerable<String> );
        public static NameSyntax IdentifierName ( IEnumerable<String> sources )
        {
            IEnumerable<String> _Tokens ()
            {
                foreach ( String source in sources )
                    foreach ( String token in source.Split (
                                                '.', StringSplitOptions.RemoveEmptyEntries ) )
                        yield return token;

                yield break;
            }


            NameSyntax  retVal = null;

            using ( IEnumerator<String> tokenEnum = _Tokens ().GetEnumerator () )
            {
                if ( !tokenEnum.MoveNext () ) return retVal;

                retVal = SyntaxFactory.IdentifierName ( tokenEnum.Current );

                while ( tokenEnum.MoveNext () )
                    retVal = SyntaxFactory.QualifiedName (
                                        retVal,
                                        SyntaxFactory.IdentifierName ( tokenEnum.Current ) );
            }

            return retVal;
        }
        public static IEnumerable<NameSyntax> IdentifierSet (
            IEnumerable<String> parameters )
                => SyntaxSet ( IdentifierName, parameters );

        public static InvocationExpressionSyntax InvocationExpression (
            String right, params String [] left )
                => InvocationExpression ( right, null, left );

        public static InvocationExpressionSyntax InvocationExpression (
            String right, IEnumerable<String> arguments, params String [] left )
                => InvocationExpression ( right, left, arguments );

        public static InvocationExpressionSyntax InvocationExpression (
            String right, IEnumerable<String> left, IEnumerable<String> arguments )
        {
            if ( String.IsNullOrWhiteSpace ( right ) ) return null;

            IdentifierNameSyntax rightStx = SyntaxFactory.IdentifierName ( right );

            if ( left == null ) return SyntaxFactory.InvocationExpression ( rightStx );

            NameSyntax qualNameStx;

            using ( IEnumerator<String> paramEnum = left.GetEnumerator () )
            {
                if ( !paramEnum.MoveNext () )
                    return SyntaxFactory.InvocationExpression ( rightStx );

                qualNameStx = IdentifierName ( paramEnum.Current );

                while ( paramEnum.MoveNext () )
                    qualNameStx = SyntaxFactory.QualifiedName (
                                        qualNameStx,
                                        SyntaxFactory.IdentifierName ( paramEnum.Current ) );

                qualNameStx = SyntaxFactory.QualifiedName ( qualNameStx, rightStx );
            }

            return SyntaxFactory.InvocationExpression ( qualNameStx, ArgumentList ( arguments ) );
        }

        public static MethodDeclarationSyntax MethodDeclaration (
            SyntaxKind                      modifierKind,
            MethodInfo                      methInfo,
            IEnumerable<StatementSyntax>    statementSet )
                => MethodDeclaration (
                        GetModifier ( modifierKind ), methInfo, statementSet, null );

        public static MethodDeclarationSyntax MethodDeclaration (
            SyntaxKind                          modifierKind,
            MethodInfo                          methInfo,
            IEnumerable<StatementSyntax>        statementSet,
            ExplicitInterfaceSpecifierSyntax    explicitInterfaceSpec )
                => MethodDeclaration ( GetModifier ( modifierKind ),
                        methInfo, statementSet, explicitInterfaceSpec );

        public static MethodDeclarationSyntax MethodDeclaration (
            MethodInfo                      methInfo,
            IEnumerable<StatementSyntax>    statementSet )
                => MethodDeclaration ( methInfo.GetModifiers (), methInfo, statementSet, null );

        public static MethodDeclarationSyntax MethodDeclaration (
            MethodInfo                          methInfo,
            IEnumerable<StatementSyntax>        statementSet,
            ExplicitInterfaceSpecifierSyntax    explicitInterfaceSpec )
                => MethodDeclaration (
                        methInfo.GetModifiers (), methInfo, statementSet, explicitInterfaceSpec );
        //{
        //    IEnumerable<Tuple<Type, String>> _TranlsatedParameters ()
        //    {
        //        foreach ( ParameterInfo pi in methInfo.GetParameters () )
        //            yield return Tuple.Create ( pi.ParameterType, pi.Name );

        //        yield break;
        //    }

        //    TypeSyntax returnTypeStx = methInfo.ReturnType == typeof ( void )
        //                    ? SyntaxFactory.PredefinedType (
        //                            SyntaxFactory.Token ( SyntaxKind.VoidKeyword ) )
        //                    : SyntaxFactory.ParseTypeName ( methInfo.ReturnType.FullName );

        //    return SyntaxFactory.MethodDeclaration ( returnTypeStx, methInfo.Name )
        //                .WithBody ( SyntaxFactory.Block ( statementSet ) )
        //                .WithModifiers ( methInfo.GetModifiers () )
        //                .WithParameterList ( ParameterList ( _TranlsatedParameters () ) );
        //}

        public static MethodDeclarationSyntax MethodDeclaration (
            SyntaxTokenList modifiers,
            MethodInfo methInfo,
            IEnumerable<StatementSyntax> statementSet )
                => MethodDeclaration ( modifiers, methInfo, statementSet, null );
        public static MethodDeclarationSyntax MethodDeclaration (
            SyntaxTokenList                     modifiers,
            MethodInfo                          methInfo,
            IEnumerable<StatementSyntax>        statementSet,
            ExplicitInterfaceSpecifierSyntax    explicitInterfaceSpec )
        {
            IEnumerable<Tuple<Type, String>> _TranlsatedParameters ()
            {
                foreach ( ParameterInfo pi in methInfo.GetParameters () )
                    yield return Tuple.Create ( pi.ParameterType, pi.Name );

                yield break;
            }

            // Controllare che methInfo non sia null
            return MethodDeclaration (
                        modifiers,
                        methInfo.ReturnType,
                        methInfo.Name,
                        _TranlsatedParameters (),
                        statementSet,
                        explicitInterfaceSpec );
        }

        public static MethodDeclarationSyntax MethodDeclaration (
            SyntaxTokenList modifiers,
            Type returnType,
            String name,
            IEnumerable<Tuple<Type, String>> parameters,
            IEnumerable<StatementSyntax> statementSet,
            ExplicitInterfaceSpecifierSyntax explicitInterfaceSpec )
                => MethodDeclaration (
                        modifiers,
                        returnType,
                        name,
                        ParameterSet ( parameters ),
                        statementSet,
                        explicitInterfaceSpec );

        public static MethodDeclarationSyntax MethodDeclaration (
            SyntaxTokenList                     modifiers,
            Type                                returnType,
            String                              name,
            IEnumerable<ParameterSyntax>        parameters,
            IEnumerable<StatementSyntax>        statementSet,
            ExplicitInterfaceSpecifierSyntax    explicitInterfaceSpec )
        {
            ParameterListSyntax paramListStx;
            TypeSyntax          returnTypeStx;

            paramListStx = parameters == null
                            ? default ( ParameterListSyntax )
                            : SyntaxFactory.ParameterList ( parameters.SeparatedList () );

            if ( returnType == typeof ( void ) )
                returnTypeStx = SyntaxFactory.PredefinedType (
                                    SyntaxFactory.Token ( SyntaxKind.VoidKeyword ) );
            else
                returnTypeStx = SyntaxHelper.IdentifierName ( returnType.FullName );

            return SyntaxFactory.MethodDeclaration (
                default ( SyntaxList<AttributeListSyntax> ),
                modifiers,
                returnTypeStx,
                explicitInterfaceSpec,
                SyntaxFactory.Identifier ( name ),
                default ( TypeParameterListSyntax ),
                paramListStx,
                default ( SyntaxList<TypeParameterConstraintClauseSyntax> ),
                SyntaxFactory.Block ( statementSet ),
                default ( SyntaxToken )
                );
        }

        public static InvocationBuilder MethodInvocation (
            params String [] methodNameStrTokens )
                => InvocationBuilder.Allocate ( methodNameStrTokens );

        public static NameBuilder Name ( params String [] nameStrTokens )
            => NameBuilder.Allocate ( nameStrTokens );

        public static NameBuilder Name ( IEnumerable<String> nameStrTokens )
            => NameBuilder.Allocate ( nameStrTokens );

        public static ExpressionAssignmentBuilder NullAssignment ( params String [] left )
            => Assignment ( left, LiteralNull );

        public static ExpressionAssignmentBuilder NullAssignment ( NameBuilder left )
            => Assignment ( left, LiteralNull );

        public static ParameterSyntax Parameter ( Type parameterType, String parameterName )
            => Parameter ( SyntaxHelper.IdentifierName ( parameterType.FullName ), parameterName );

        public static ParameterSyntax Parameter ( TypeSyntax parameterType, String parameterName )
            => SyntaxFactory.Parameter (
                    default ( SyntaxList<AttributeListSyntax> ),
                    default ( SyntaxTokenList ),
                    parameterType,
                    SyntaxFactory.Identifier ( parameterName ),
                    default ( EqualsValueClauseSyntax ) );

        public static ParameterListSyntax ParameterList ()
                => SyntaxFactory.ParameterList ( ParameterSet ( null ).SeparatedList () );
        public static ParameterListSyntax ParameterList (
            IEnumerable<Tuple<Type, String>> parameters )
                => SyntaxFactory.ParameterList ( ParameterSet ( parameters ).SeparatedList () );
        public static IEnumerable<ParameterSyntax> ParameterSet (
            IEnumerable<Tuple<Type, String>> parameters )
        {
            if ( parameters == null ) yield break;

            foreach ( Tuple<Type, String> param in parameters )
                yield return Parameter ( param.Item1, param.Item2 );

            yield break;
        }

        public static PropertyDeclarationSyntax PropertyDeclaration (
            SyntaxKind                      modifierKind,
            PropertyInfo                    propInfo,
            IEnumerable<StatementSyntax>    getStatementSet,
            IEnumerable<StatementSyntax>    setStatementSet )
                => PropertyDeclaration (
                    GetModifier ( modifierKind ), propInfo, getStatementSet, setStatementSet );

        public static PropertyDeclarationSyntax PropertyDeclaration (
            SyntaxTokenList                 modifiers,
            PropertyInfo                    propInfo,
            IEnumerable<StatementSyntax>    getStatementSet,
            IEnumerable<StatementSyntax>    setStatementSet )
        {
            IEnumerable<AccessorDeclarationSyntax> AccessorDeclStx ()
            {
                if ( propInfo.CanRead && getStatementSet != null )
                    yield return SyntaxFactory.AccessorDeclaration (
                        SyntaxKind.GetAccessorDeclaration, SyntaxFactory.Block (
                            getStatementSet ) );

                if ( propInfo.CanWrite && setStatementSet != null)
                    yield return SyntaxFactory.AccessorDeclaration (
                            SyntaxKind.SetAccessorDeclaration,
                            SyntaxFactory.Block ( setStatementSet ) );

                yield break;
            }


            if ( propInfo == null ) return null;

            return SyntaxFactory.PropertyDeclaration (
                        new SyntaxList<AttributeListSyntax> (),
                        modifiers,
                        SyntaxFactory.ParseTypeName ( propInfo.PropertyType.FullName ),
                        null,
                        SyntaxFactory.Identifier ( propInfo.Name ),
                        SyntaxFactory.AccessorList ( AccessorDeclStx ().SyntaxList () ) );
        }
        public static FieldDeclarationSyntax PrivateFieldDeclaration (
            Type fieldType, params String [] fieldsNames )
                => SyntaxFactory.FieldDeclaration (
                        EmptySyntaxList<AttributeListSyntax> (),
                        PrivateModifier,
                        VariableDeclaration ( fieldType, fieldsNames ) );

        public static FieldDeclarationSyntax ProtectedFieldDeclaration (
            Type fieldType, params String [] fieldsNames )
                => SyntaxFactory.FieldDeclaration (
                        EmptySyntaxList<AttributeListSyntax> (),
                        ProtectedModifier,
                        VariableDeclaration ( fieldType, fieldsNames ) );

        public static ConstructorDeclarationSyntax PublicConstructorDeclaration (
            String                          className,
            IEnumerable<StatementSyntax>    bodyStatementSet,
            IEnumerable<Tuple<Type, String>> parameters )
                => ConstructorDeclaration (
                        PublicModifier, className, bodyStatementSet, parameters );

        public static FieldDeclarationSyntax PublicFieldDeclaration (
            Type fieldType, params String [] fieldsNames )
                => SyntaxFactory.FieldDeclaration (
                        EmptySyntaxList<AttributeListSyntax> (),
                        PublicModifier,
                        VariableDeclaration ( fieldType, fieldsNames ) );

        public static MethodDeclarationSyntax PublicMethodDeclaration (
            MethodInfo                  methInfo,
            params StatementSyntax []   statementSet )
                => MethodDeclaration ( PublicModifier, methInfo, statementSet );

        public static MethodDeclarationSyntax PublicMethodDeclaration (
            MethodInfo                      methInfo,
            IEnumerable<StatementSyntax>    statementSet )
                => MethodDeclaration ( PublicModifier, methInfo, statementSet );
        public static PropertyDeclarationSyntax PublicPropertyDeclaration (
            PropertyInfo                    propInfo,
            IEnumerable<StatementSyntax>    getStatementSet,
            IEnumerable<StatementSyntax>    setStatementSet )
            => PropertyDeclaration ( PublicModifier, propInfo, getStatementSet, setStatementSet );

        public static SeparatedSyntaxList<SyntaxType> SeparatedSyntaxList<SyntaxType, ParamType> (
                    Func<IEnumerable<ParamType>, IEnumerable<SyntaxType>> InputConvertDlg,
            params ParamType [] parameters )
                where SyntaxType : Microsoft.CodeAnalysis.SyntaxNode
                    => SyntaxFactory.SeparatedList<SyntaxType> ( InputConvertDlg ( parameters ) );

        public static LiteralExpressionSyntax StringLiteralExpression ( String srcStr )
            => SyntaxFactory.LiteralExpression (    SyntaxKind.StringLiteralExpression,
                                                    SyntaxFactory.Literal ( srcStr ) );

        public static IEnumerable<SyntaxItemType> SyntaxSet<SyntaxItemType, InputItemType> (
            Func<InputItemType, SyntaxItemType> StxItemGeneratorDlg,
            IEnumerable<InputItemType>          parameters )
        {
            if ( parameters             == null ) yield break;
            if ( StxItemGeneratorDlg    == null ) yield break;

            foreach ( InputItemType prm in parameters )
                yield return StxItemGeneratorDlg ( prm );

            yield break;
        }

        public static TypeSyntax Type ( Type type )
            => _Type ( out Int32 k, type, null as IEnumerable<String> );

        public static TypeSyntax Type ( Type type, params Type [] typeParams )
            => _Type ( out Int32 k, type, typeParams );

        public static TypeSyntax Type ( Type type, params String [] typeParamsStr )
                => _Type ( out Int32 k, type, typeParamsStr );

        private static TypeSyntax _Type (
            out     Int32   outerTypeParamsNum,
                    Type    type,
            params  Type [] typeParams )
        {
            //NameSyntax  retVal;
            //Int32       typeParamsCount;

            //IEnumerable<Type> _PeekTypeParams ( Int32 alreadyUsed )
            //{
            //    for ( Int32 k = alreadyUsed ; k < type.GetGenericArguments ().Length ; k++ )
            //        yield return typeParams [ k ];

            //    yield break;
            //}

            //outerTypeParamsNum  = 0;
            //typeParamsCount     = type.GetGenericArguments ().Length;

            //retVal = type.IsNested
            //            ? _Type ( out outerTypeParamsNum, type.DeclaringType, typeParams )
            //                as NameSyntax
            //            : IdentifierName ( type.Namespace );

            //if ( type.IsGenericType && outerTypeParamsNum < typeParamsCount )
            //    retVal = SyntaxFactory.QualifiedName (
            //                retVal,
            //                SyntaxFactory.GenericName (
            //                    SyntaxFactory.Identifier ( type.Name.Split ( '`' ) [0] ),
            //                    TypeArgunentList ( _PeekTypeParams ( outerTypeParamsNum ) ) ) );

            //else
            //    retVal = SyntaxFactory.QualifiedName (
            //                retVal, SyntaxFactory.IdentifierName ( type.Name ) );

            //outerTypeParamsNum = typeParamsCount;

            //return retVal;
            IEnumerable<String> _TypeParamsStr ()
            {
                if ( typeParams != null )
                    for ( Int32 k = 0 ; k < typeParams.Length ; k++ )
                        yield return typeParams [k].FullName;

                yield break;
            }

            return _Type ( out outerTypeParamsNum, type, _TypeParamsStr () );
        }

        private static TypeSyntax _Type (
            out     Int32               outerTypeParamsNum,
                    Type                type,
                    IEnumerable<String> typeParamsStr )
        {
            NameSyntax  retVal;
            Int32       typeParamsCount;

            IEnumerable<String> _PeekTypeParams ( Int32 alreadyUsed )
            {
                if ( typeParamsStr == null )
                    yield break;

                using ( IEnumerator<String> paramEnum = typeParamsStr.GetEnumerator () )
                {
                    Int32 k = 0;

                    while ( k < alreadyUsed )
                        if ( paramEnum.MoveNext () )
                            k++;
                        else
                            yield break;

                    while ( paramEnum.MoveNext () && k++ < type.GetGenericArguments ().Length )
                        yield return paramEnum.Current;
                }

                yield break;
            }

            outerTypeParamsNum  = 0;
            typeParamsCount     = type.GetGenericArguments ().Length;

            retVal = type.IsNested
                        ? _Type ( out outerTypeParamsNum, type.DeclaringType, typeParamsStr )
                            as NameSyntax
                        : IdentifierName ( type.Namespace );

            if ( type.IsGenericType && outerTypeParamsNum < typeParamsCount )
                retVal = SyntaxFactory.QualifiedName (
                            retVal,
                            SyntaxFactory.GenericName (
                                SyntaxFactory.Identifier ( type.Name.Split ( '`' ) [0] ),
                                TypeArgunentList ( _PeekTypeParams ( outerTypeParamsNum ) ) ) );

            else
                retVal = SyntaxFactory.QualifiedName (
                            retVal, SyntaxFactory.IdentifierName ( type.Name ) );

            outerTypeParamsNum = typeParamsCount;

            return retVal;
        }

        public static TypeArgumentListSyntax TypeArgunentList ( params Type [] argsTypes )
            => SyntaxFactory.TypeArgumentList ( TypeSet ( argsTypes ).SeparatedList () );

        public static TypeArgumentListSyntax TypeArgunentList ( IEnumerable<Type> argsTypes )
            => SyntaxFactory.TypeArgumentList ( TypeSet ( argsTypes ).SeparatedList () );

        public static TypeArgumentListSyntax TypeArgunentList ( params String [] argsTypesStr )
            => SyntaxFactory.TypeArgumentList ( TypeSet ( argsTypesStr ).SeparatedList () );

        public static TypeArgumentListSyntax TypeArgunentList ( IEnumerable<String> argsTypesStr )
            => SyntaxFactory.TypeArgumentList ( TypeSet ( argsTypesStr ).SeparatedList () );

        public static TypeParameterSyntax TypeParameter ( Type paramType )
            => SyntaxFactory.TypeParameter ( paramType.FullName );

        public static TypeParameterSyntax TypeParameter ( String paramTypeStr )
            => SyntaxFactory.TypeParameter ( paramTypeStr );

        public static TypeParameterListSyntax TypeParameterList ( params Type [] paramsTypes )
            => SyntaxFactory.TypeParameterList (
                                TypeParameterSet ( paramsTypes ).SeparatedList () );
        public static TypeParameterListSyntax TypeParameterList ( params String [] paramsTypesStr )
            => SyntaxFactory.TypeParameterList (
                                TypeParameterSet ( paramsTypesStr ).SeparatedList () );

        public static IEnumerable<TypeParameterSyntax> TypeParameterSet (
            params Type [] paramsTypes )
        {
            if ( paramsTypes == null ) yield break;

            foreach ( Type paramType in paramsTypes )
                yield return TypeParameter ( paramType );

            yield break;
        }

        public static IEnumerable<TypeParameterSyntax> TypeParameterSet (
            params String [] paramsTypes )
        {
            if ( paramsTypes == null ) yield break;

            foreach ( String paramType in paramsTypes )
                yield return TypeParameter ( paramType );

            yield break;
        }


        public static IEnumerable<TypeSyntax> TypeSet ( params Type [] types )
            => _TypeSet ( types );

        public static IEnumerable<TypeSyntax> TypeSet ( IEnumerable<Type> types )
            => _TypeSet ( types );

        public static IEnumerable<TypeSyntax> TypeSet ( params String [] typesStr )
            => _TypeSet ( typesStr );

        public static IEnumerable<TypeSyntax> TypeSet ( IEnumerable<String> typesStr )
            => _TypeSet ( typesStr );

        private static IEnumerable<TypeSyntax> _TypeSet ( IEnumerable<Type> types )
        {
            if ( types == null ) yield break;

            foreach ( Type type in types )
                yield return Type ( type );

            yield break;
        }

        private static IEnumerable<TypeSyntax> _TypeSet ( IEnumerable<String> typesStr )
        {
            if ( typesStr == null ) yield break;

            foreach ( String typeStr in typesStr )
                yield return IdentifierName ( typeStr );

            yield break;
        }

        public static VariableDeclarationSyntax VariableDeclaration (
            Type variableType, params String [] variablesNames )
                => SyntaxFactory.VariableDeclaration (
                        SyntaxFactory.ParseTypeName ( variableType.FullName ),
                        SyntaxSet ( SyntaxFactory.VariableDeclarator, variablesNames )
                            .SeparatedList () );

        public static SyntaxTokenList GetModifiers ( this MethodInfo methInfo )
        {
            IEnumerable<SyntaxToken> _ModifierSet ()
            {
                if ( methInfo == null ) yield break;

                if      ( methInfo.IsPrivate )  yield return PrivateToken;
                else if ( methInfo.IsPublic )   yield return PublicToken;
                else                            yield return ProtectedToken;

                if ( methInfo.IsStatic ) yield return SyntaxFactory.Token ( SyntaxKind.StaticKeyword );
                else if ( methInfo.IsAbstract) yield return SyntaxFactory.Token ( SyntaxKind.OverrideKeyword );
                else if ( methInfo.IsVirtual ) yield return SyntaxFactory.Token ( SyntaxKind.OverrideKeyword );

                yield break;
            }


            return SyntaxFactory.TokenList ( _ModifierSet () );
        }

        public static SyntaxList<SyntaxItemType> SyntaxList<SyntaxItemType> (
            params SyntaxItemType [] targets )
                where SyntaxItemType : SyntaxNode
                    => SyntaxFactory.List ( targets );

        public static SyntaxList<SyntaxItemType> SyntaxList<SyntaxItemType> (
            this IEnumerable<SyntaxItemType> target )
                where SyntaxItemType : SyntaxNode
                    => SyntaxFactory.List<SyntaxItemType> ( target );

        public static IEnumerable<ItemType> ToEnumerable<ItemType> ( this ItemType item )
        {
            yield return item;
            yield break;
        }
    }

}
