using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Test.DependencyInjection.DynamicProxy
{
    public class ProxyServiceContainer : IServiceProvider
    {
        protected class RegisteredServices : KeyedCollection<Type, RegisteredServices.ServiceDescriptor>
        {
            public class ServiceDescriptor
            {
                public ServiceDescriptor ( Type interfaceType, Type implementationType )
                    : this ( interfaceType, implementationType, null ) { }
                public ServiceDescriptor (
                    Type interfaceType, Type implementationType, IServiceProvider proxyAssemblySp )
                {
                    InterfaceType = interfaceType;
                    ImplementationType = implementationType;
                    ProxyAssemblySp = proxyAssemblySp;
                }
                public Type InterfaceType { get; }
                public Type ImplementationType { get; }
                public IServiceProvider ProxyAssemblySp { get; set; }
            }


            public Boolean TryAdd ( Type interfaceType, Type implementationType )
            {
                if ( interfaceType == null ) return false;
                if ( implementationType == null ) return false;
                if ( !interfaceType.IsInterface ) return false;
                if ( Contains ( interfaceType ) ) return false;

                Int32 cnt = Count + 1;

                Add ( new ServiceDescriptor ( interfaceType, implementationType ) );

                return cnt == Count;
            }


            protected override Type GetKeyForItem ( ServiceDescriptor item )
                => item == null ? null : item.InterfaceType;
        }

        private RegisteredServices __services;

        public Boolean AddService<InterfaceType, ImplementationType> ()
            where ImplementationType : InterfaceType
        {
            return ImplServices.TryAdd ( typeof ( InterfaceType ), typeof ( ImplementationType ) );
        }

        public Object GetService ( Type interfaceType )
        {
            if ( interfaceType == null ) return null;

            RegisteredServices.ServiceDescriptor serviceDescr;

            Type implType;

            if ( !ImplServices.TryGetValue ( interfaceType, out serviceDescr ) ) return null;

            if ( serviceDescr.ProxyAssemblySp != null )
                return serviceDescr.ProxyAssemblySp.GetService ( interfaceType );

            implType = serviceDescr.ImplementationType;

            Dictionary<String, List<ClassDeclarationSyntax>> nsClasses  = new Dictionary<String, List<ClassDeclarationSyntax>> ();
            List<ClassDeclarationSyntax> classes;
            ClassDeclarationSyntax      classDeclStx = null;

            //var k  = ImplServices.Where ( ( sd ) => sd.ImplementationType.Assembly == serviceDescr.ImplementationType.Assembly && sd.ProxyAssemblySp == null );
            foreach ( RegisteredServices.ServiceDescriptor sd
                        in ImplServices.Where ( ( sd ) => sd.ProxyAssemblySp == null ) )
            {
                if ( !nsClasses.TryGetValue ( sd.ImplementationType.Namespace, out classes ) )
                    nsClasses.Add ( sd.ImplementationType.Namespace,
                                    classes = new List<ClassDeclarationSyntax> () );

                classDeclStx = _BuildProxyClass ( sd.ImplementationType, sd.InterfaceType );

                if ( classDeclStx == null )
                {
                    sd.ProxyAssemblySp = this;
                    continue;
                }

                classes.Add ( classDeclStx );
            }

            //foreach ( Type implType1 in serviceDescr.ImplementationType.Assembly.ExportedTypes )
            //    if ( ImplServices.FirstOrDefault ( ( sd ) => sd.ImplementationType == implType1 ) != null )
            //        implType = implType1;

            // Ottenere la lista dei metodi dalla reflection
            MethodInfo [] methInfos = interfaceType.GetMethods ();

            CompilationUnitSyntax       cuStx;
            NameSyntax                  nameStx;
            NamespaceDeclarationSyntax  nsDeclStx;

            nameStx = SyntaxFactory.IdentifierName ( implType.Namespace + ".Proxies" ); //"Sharpframework.DynamicProxy" );

            nsDeclStx = SyntaxFactory.NamespaceDeclaration ( nameStx )
                        .WithMembers ( new SyntaxList<MemberDeclarationSyntax> ( classDeclStx ) );

            nsDeclStx = nsDeclStx.AddUsings ( SyntaxFactory.UsingDirective ( SyntaxFactory.ParseName ( implType.Namespace ) ) );

            cuStx = SyntaxFactory.CompilationUnit ()
                        .WithMembers ( new SyntaxList<MemberDeclarationSyntax> ( nsDeclStx ) );

            Console.WriteLine ( cuStx.NormalizeWhitespace ().ToFullString () );

            return implType;
        }


        protected RegisteredServices ImplServices
        {
            get
            {
                if ( __services == null ) __services = new RegisteredServices ();

                return __services;
            }
        }


        private FieldDeclarationSyntax _BuildFieldDeclaration (
            SyntaxKind modifierKind, Type fieldType, params String [] fieldsNames )
        {
            SyntaxTokenList             modifiers;
            VariableDeclarationSyntax   varDecl;

            modifiers = SyntaxTokenList.Create ( SyntaxFactory.Token ( modifierKind ) );
            varDecl = _BuildVariableDeclaration ( fieldType, fieldsNames );

            return SyntaxFactory.FieldDeclaration (
                        new SyntaxList<AttributeListSyntax> (), modifiers, varDecl );
        }

        private FieldDeclarationSyntax _BuildPrivateFieldDeclaration (
            Type fieldType, params String [] fieldsNames )
                => _BuildFieldDeclaration ( SyntaxKind.PrivateKeyword, fieldType, fieldsNames );

        private VariableDeclarationSyntax _BuildVariableDeclaration (
            Type variableType, params String [] variablesNames )
        {
            IEnumerable<VariableDeclaratorSyntax> _VariableDeclarators ()
            {
                foreach ( String variableName in variablesNames )
                    yield return SyntaxFactory.VariableDeclarator ( variableName );

                yield break;
            }

            SeparatedSyntaxList<VariableDeclaratorSyntax> _VariablesDeclarations ()
                => SyntaxFactory.SeparatedList ( _VariableDeclarators () );
            //=> SyntaxFactory.SeparatedList<VariableDeclaratorSyntax> (
            //                                            _VariableDeclarators () );

            return SyntaxFactory.VariableDeclaration (
                                        SyntaxFactory.ParseTypeName ( variableType.Name ),
                                        _VariablesDeclarations () );
        }

        private MethodDeclarationSyntax _BuildMethodDeclaration ( SyntaxKind modifierKind, MethodInfo mi )
        {
            SyntaxTokenList             modifiers;
            TypeSyntax returnTypeStx = SyntaxFactory.ParseTypeName ( mi.ReturnType.Name );

            modifiers = SyntaxTokenList.Create ( SyntaxFactory.Token ( modifierKind ) );

            QualifiedNameSyntax qualNameStx = SyntaxFactory.QualifiedName ( SyntaxFactory.IdentifierName ( "__proxiedObject" ), SyntaxFactory.IdentifierName ( mi.Name ) );

            InvocationExpressionSyntax invokeExprStx = SyntaxFactory.InvocationExpression ( qualNameStx );

            return SyntaxFactory.MethodDeclaration ( returnTypeStx, mi.Name )
                        .WithBody ( SyntaxFactory.Block ().AddStatements ( SyntaxFactory.ExpressionStatement ( invokeExprStx ) ) )
                        .WithModifiers ( modifiers );
        }
        private ConstructorDeclarationSyntax _BuildConstructorDeclaration ( SyntaxKind modifierKind, String className, BlockSyntax bodyStx, params Tuple<Type, String> [] parameters )
            => _BuildConstructorDeclaration ( modifierKind, className, bodyStx, parameters as IEnumerable<Tuple<Type, String>> );
        private ConstructorDeclarationSyntax _BuildConstructorDeclaration ( SyntaxKind modifierKind, String className, BlockSyntax bodyStx, IEnumerable<Tuple<Type, String>> parameters )
        {
            IEnumerable<ParameterSyntax> ParametersList ()
            {
                if ( parameters == null ) yield break;

                foreach ( Tuple<Type, String> param in parameters )
                    yield return SyntaxFactory.Parameter (
                                        new SyntaxList<AttributeListSyntax> (),
                                        new SyntaxTokenList (),
                                        SyntaxFactory.ParseTypeName ( param.Item1.FullName ),
                                        SyntaxFactory.Identifier ( param.Item2 ),
                                        null );

                yield break;
            }

            SyntaxTokenList                         modifiers;
            ParameterListSyntax                     paramListStx;
            SeparatedSyntaxList<ParameterSyntax>    ssl;

            ssl = SyntaxFactory.SeparatedList<ParameterSyntax> ( ParametersList () );
            paramListStx = SyntaxFactory.ParameterList ( ssl );
            modifiers = SyntaxTokenList.Create ( SyntaxFactory.Token ( modifierKind ) );

            if ( bodyStx == null ) bodyStx = SyntaxFactory.Block ();

            return SyntaxFactory.ConstructorDeclaration ( className )
                        .WithBody ( bodyStx )
                        .WithModifiers ( modifiers )
                        .WithParameterList ( paramListStx );
        }
        private ConstructorDeclarationSyntax _BuildPublicConstructorDeclaration ( String className, params Tuple<Type, String> [] parameters )
            => _BuildPublicConstructorDeclaration ( className, null, parameters );
        private ConstructorDeclarationSyntax _BuildPublicConstructorDeclaration ( String className, BlockSyntax bodyStx, params Tuple<Type, String> [] parameters )
            => _BuildConstructorDeclaration ( SyntaxKind.PublicKeyword, className, bodyStx, parameters );
        private MethodDeclarationSyntax _BuildPublicMethodDeclaration ( MethodInfo mi )
            => _BuildMethodDeclaration ( SyntaxKind.PublicKeyword, mi );

        private BaseListSyntax _BuildBaseListSyntax ( params Type [] baseTypes )
        {
            IEnumerable<BaseTypeSyntax> BaseTypesStx ()
            {
                if ( baseTypes == null ) yield break;

                foreach ( Type baseTy in baseTypes )
                    yield return SyntaxFactory.SimpleBaseType (
                                    SyntaxFactory.ParseTypeName ( baseTy.Name ) );

                yield break;
            }

            return SyntaxFactory.BaseList (
                        SyntaxFactory.SeparatedList<BaseTypeSyntax> ( BaseTypesStx () ) );

        }

        public PropertyDeclarationSyntax _BuildPublicPropertyDeclaration ( PropertyInfo propInfo )
            => _BuildPropertyDeclaration ( SyntaxKind.PublicKeyword, propInfo );
        public PropertyDeclarationSyntax _BuildPropertyDeclaration ( SyntaxKind modifierKind, PropertyInfo propInfo )
        {
            IEnumerable<AccessorDeclarationSyntax> AccessorDeclStx ()
            {
                if ( propInfo.CanRead )
                    yield return SyntaxFactory.AccessorDeclaration (
                        SyntaxKind.GetAccessorDeclaration, SyntaxFactory.Block (
                            SyntaxFactory.ReturnStatement ( SyntaxFactory.QualifiedName ( SyntaxFactory.IdentifierName ( "__proxiedObject" ), SyntaxFactory.IdentifierName ( propInfo.Name ) ) ) ) );

                if ( propInfo.CanWrite )
                    yield return SyntaxFactory.AccessorDeclaration (
                            SyntaxKind.SetAccessorDeclaration,
                            SyntaxFactory.Block (
                                SyntaxFactory.ExpressionStatement (
                                SyntaxFactory.AssignmentExpression ( SyntaxKind.SimpleAssignmentExpression,
                                SyntaxFactory.QualifiedName ( SyntaxFactory.IdentifierName ( "__proxiedObject" ), SyntaxFactory.IdentifierName ( propInfo.Name ) ),
                                SyntaxFactory.IdentifierName ( "value" ) ) )
                            )
                        );

                yield break;
            }

            SyntaxList<AccessorDeclarationSyntax> AccessorDeclStxLst ()
                => new SyntaxList<AccessorDeclarationSyntax> ( AccessorDeclStx () );

            if ( propInfo == null ) return null;

            return SyntaxFactory.PropertyDeclaration (
                        new SyntaxList<AttributeListSyntax> (),
                        SyntaxTokenList.Create ( SyntaxFactory.Token ( modifierKind ) ),
                        SyntaxFactory.ParseTypeName ( propInfo.PropertyType.Name ),
                        null,
                        SyntaxFactory.Identifier ( propInfo.Name ),
                        SyntaxFactory.AccessorList ( AccessorDeclStxLst () ) );
        }
        private IEnumerable<SyntaxType> PPP<SyntaxType, ValueInputType, ParamType> (
            Func<ValueInputType, SyntaxType> StxGeneratorDlg,
            Func<ParamType, ValueInputType> ParamConverterDlg,
            IEnumerable<ParamType> parameters )
        {
            if ( ParamConverterDlg  == null ) yield break;
            if ( parameters         == null ) yield break;
            if ( StxGeneratorDlg    == null ) yield break;

            foreach ( ParamType prm in parameters )
                yield return StxGeneratorDlg ( ParamConverterDlg ( prm ) );

            yield break;
        }

        private SeparatedSyntaxList<ArgumentSyntax> ArgumentSeparatedSyntaxList ( params String [] parameters )
            => SeparatedSyntaxList ( ArgumentSetSyntax, parameters );

        private ArgumentListSyntax ArgumentListSyntax ( params String [] parameters )
            => SyntaxFactory.ArgumentList ( ArgumentSeparatedSyntaxList ( parameters ) );
        private SeparatedSyntaxList<SyntaxType> SeparatedSyntaxList<SyntaxType, ParamType> (
            Func<IEnumerable<ParamType>, IEnumerable<SyntaxType>> ConvertDlg,
            params ParamType [] parameters )
                where SyntaxType : Microsoft.CodeAnalysis.SyntaxNode
                    => SyntaxFactory.SeparatedList<SyntaxType> ( ConvertDlg ( parameters ) );
        private IEnumerable<ArgumentSyntax> ArgumentSetSyntax (
            params String [] parameters )
                => ArgumentSetSyntax ( SyntaxFactory.IdentifierName, parameters );
        private IEnumerable<ArgumentSyntax> ArgumentSetSyntax (
            IEnumerable<String> parameters )
                => ArgumentSetSyntax ( SyntaxFactory.IdentifierName, parameters );
        private IEnumerable<ArgumentSyntax> ArgumentSetSyntax<ParamType> (
            Func<ParamType, ExpressionSyntax> ParamConverterDlg,
            params ParamType [] parameters )
                => PPP ( SyntaxFactory.Argument, ParamConverterDlg, parameters );
        private IEnumerable<ArgumentSyntax> ArgumentSetSyntax<ParamType> (
            Func<ParamType, ExpressionSyntax> ParamConverterDlg,
            IEnumerable<ParamType> parameters )
                => PPP ( SyntaxFactory.Argument, ParamConverterDlg, parameters );
        private ClassDeclarationSyntax _BuildProxyClass ( Type implType, Type contractType )
        {
            if ( !implType.IsClass          ) return null;
            if ( !contractType.IsInterface  ) return null;

            ArgumentListSyntax                  argListStx;
            ClassDeclarationSyntax              classDeclStx;
            String                              className;
            ConstructorDeclarationSyntax        constrDeclStx;
            FieldDeclarationSyntax              fldDeclStx;
            MethodDeclarationSyntax             methDeclStx;
            List<MemberDeclarationSyntax>       membersDeclStx;
            SyntaxToken                         privateModifierStx;
            PropertyInfo []                     properties;

            className = "_" + implType.Name + "Proxy";
            membersDeclStx = new List<MemberDeclarationSyntax> ();
            privateModifierStx = SyntaxFactory.Token ( SyntaxKind.PrivateKeyword );

            fldDeclStx = _BuildPrivateFieldDeclaration ( contractType, "__proxiedObject" );

            membersDeclStx.Add ( fldDeclStx );

            if ( implType.GetConstructor ( new Type [] { typeof ( IServiceProvider ) } ) == null )
                argListStx = ArgumentListSyntax ();
            else
                argListStx = ArgumentListSyntax ( "sp" );

            BlockSyntax bodyStx = SyntaxFactory.Block (
                SyntaxFactory.ExpressionStatement (
                SyntaxFactory.AssignmentExpression ( SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName ( "__proxiedObject" ),
                SyntaxFactory.ObjectCreationExpression ( SyntaxFactory.Token( SyntaxKind.NewKeyword ), SyntaxFactory.ParseTypeName ( implType.Name ), argListStx, null ) ) ) );

            constrDeclStx = _BuildPublicConstructorDeclaration ( className, bodyStx, Tuple.Create ( typeof ( IServiceProvider ), "sp" ) );

            membersDeclStx.Add ( constrDeclStx );

            constrDeclStx = _BuildPublicConstructorDeclaration ( className );

            membersDeclStx.Add ( constrDeclStx );

            properties = contractType.GetProperties ();

            foreach ( MethodInfo methInfo in contractType.GetMethods ()
                                                .RemovePropertiesAccessors ( properties ) )
                membersDeclStx.Add ( methDeclStx = _BuildPublicMethodDeclaration ( methInfo ) );

            foreach ( PropertyInfo propInfo in properties )
                membersDeclStx.Add ( _BuildPublicPropertyDeclaration ( propInfo ) );

            classDeclStx = SyntaxFactory.ClassDeclaration ( className )
                                .WithModifiers ( SyntaxTokenList.Create ( privateModifierStx ) )
                                .WithBaseList ( _BuildBaseListSyntax ( contractType ) )
                                .WithMembers ( new SyntaxList<MemberDeclarationSyntax> ( membersDeclStx ) );

            return classDeclStx;
        }
    }

    public static class MethodInfoExtensions
    {
        private static MethodInfo [] __emptyMethodInfo;

        static MethodInfoExtensions ()
        {
            __emptyMethodInfo = null;
        }

        public static MethodInfo [] Empty ( this MethodInfo [] target )
        {
            if ( __emptyMethodInfo == null ) __emptyMethodInfo = new MethodInfo [0];

            return __emptyMethodInfo;
        }
        public static MethodInfo [] RemovePropertiesAccessors ( this MethodInfo [] target )
            => RemovePropertiesAccessors ( target, null );
        public static MethodInfo [] RemovePropertiesAccessors ( this MethodInfo [] target
            , IEnumerable<PropertyInfo> properties )
        {
            void RemovePropertiesAccessors ( IList<MethodInfo> methods, MethodInfo accessor )
            {
                if ( methods.Contains ( accessor ) ) methods.Remove ( accessor);
            }

            List<MethodInfo> methods        = new List<MethodInfo> ();
            List<MethodInfo> specialNames   = null;

            if ( target == null ) return target.Empty ();

            foreach ( MethodInfo mi in target )
                if ( mi.MemberType != MemberTypes.Method )
                    continue;
                else if ( mi.IsSpecialName )
                    if ( specialNames == null )
                        (specialNames = new List<MethodInfo> ()).Add ( mi );
                    else
                        specialNames.Add ( mi );
                else
                    methods.Add ( mi );

            if ( specialNames == null ) return target;

            if ( properties == null ) properties = target [0].DeclaringType.GetProperties ();

            foreach ( PropertyInfo pi in properties )
            {
                RemovePropertiesAccessors ( specialNames, pi.GetMethod );
                RemovePropertiesAccessors ( specialNames, pi.SetMethod );
            }

            if ( specialNames.Count > 0 ) methods.AddRange ( specialNames );

            return methods.ToArray ();
        }
    }
}
