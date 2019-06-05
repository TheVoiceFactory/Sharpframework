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


        public static ArgumentListSyntax ArgumentList ( params String [] parameters )
            => ArgumentList ( parameters as IEnumerable<String> );

        public static ArgumentListSyntax ArgumentList ( IEnumerable<String> parameters )
            => SyntaxFactory.ArgumentList ( ArgumentSet ( parameters ).SeparatedList () );

        public static IEnumerable<ArgumentSyntax> ArgumentSet (
            IEnumerable<String> parameters )
                => ConvertSet ( IdentifierSet ( parameters ), SyntaxFactory.Argument );

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
                => MethodDeclaration ( GetModifier ( modifierKind ), methInfo, statementSet );

        public static MethodDeclarationSyntax MethodDeclaration (
            SyntaxTokenList                 modifiers,
            MethodInfo                      methInfo,
            IEnumerable<StatementSyntax>    statementSet )
        {
            IEnumerable<Tuple<Type, String>> _TranlsatedParameters ()
            {
                foreach ( ParameterInfo pi in methInfo.GetParameters () )
                    yield return Tuple.Create ( pi.ParameterType, pi.Name );

                yield break;
            }

            TypeSyntax returnTypeStx = methInfo.ReturnType == typeof ( void )
                            ? SyntaxFactory.PredefinedType (
                                    SyntaxFactory.Token ( SyntaxKind.VoidKeyword ) )
                            : SyntaxFactory.ParseTypeName ( methInfo.ReturnType.FullName );

            return SyntaxFactory.MethodDeclaration ( returnTypeStx, methInfo.Name )
                        .WithBody ( SyntaxFactory.Block ( statementSet ) )
                        .WithModifiers ( modifiers )
                        .WithParameterList ( ParameterList ( _TranlsatedParameters () ) );
        }

        public static ParameterListSyntax ParameterList (
            IEnumerable<Tuple<Type, String>> parameters )
                => SyntaxFactory.ParameterList ( ParameterSet ( parameters ).SeparatedList () );
        public static IEnumerable<ParameterSyntax> ParameterSet (
            IEnumerable<Tuple<Type, String>> parameters )
        {
            if ( parameters == null ) yield break;

            foreach ( Tuple<Type, String> param in parameters )
                yield return SyntaxFactory.Parameter (
                                    EmptySyntaxList<AttributeListSyntax> (),
                                    EmptyTokenList (),
                                    SyntaxFactory.ParseTypeName ( param.Item1.FullName ),
                                    SyntaxFactory.Identifier ( param.Item2 ),
                                    null );

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
            if ( parameters == null ) yield break;
            if ( StxItemGeneratorDlg == null ) yield break;

            foreach ( InputItemType prm in parameters )
                yield return StxItemGeneratorDlg ( prm );

            yield break;
        }

        public static VariableDeclarationSyntax VariableDeclaration (
            Type variableType, params String [] variablesNames )
                => SyntaxFactory.VariableDeclaration (
                        SyntaxFactory.ParseTypeName ( variableType.FullName ),
                        SyntaxSet ( SyntaxFactory.VariableDeclarator, variablesNames )
                            .SeparatedList () );

        public static SyntaxList<SyntaxItemType> SyntaxList<SyntaxItemType> (
            this IEnumerable<SyntaxItemType> target )
                where SyntaxItemType : SyntaxNode
                    => SyntaxFactory.List<SyntaxItemType> ( target );
    }

}
