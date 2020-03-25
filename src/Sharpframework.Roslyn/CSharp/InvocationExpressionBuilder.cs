using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Sharpframework.Roslyn.CSharp
{
    public class InvocationExpressionBuilder
        : BuilderBase<InvocationExpressionBuilder>
    {
        List<ArgumentSyntax>    __argsStxs;
        NameSyntax              __memberNameStx;


        private InvocationExpressionBuilder () => ImplReset ();


        public static InvocationExpressionBuilder Allocate ( String methodName )
        {
            if ( String.IsNullOrWhiteSpace ( methodName ) )
                throw new ArgumentException ( "Invalid method name" );

            return Allocate ( SyntaxHelper.IdentifierName ( methodName ) );
        } // End of Allocate (...)

        public static InvocationExpressionBuilder Allocate (
            String leftMethodName,
            String rightMetodName )
        {
            if ( String.IsNullOrWhiteSpace ( leftMethodName ) )
                return Allocate ( rightMetodName );

            if ( String.IsNullOrWhiteSpace ( rightMetodName ) )
                throw new ArgumentException ( "Invalid method name" );

            return Allocate ( SyntaxHelper.IdentifierName ( leftMethodName, rightMetodName ) );
        } // End of Allocate (...)

        public static InvocationExpressionBuilder Allocate ( NameSyntax methodName )
        {
            InvocationExpressionBuilder retVal = ImplGetRecycled ();

            if ( retVal == null )
                retVal = new InvocationExpressionBuilder ();

            retVal.__memberNameStx =  methodName;

            return retVal;
        } // End of Allocate (...)


        public static implicit operator InvocationExpressionSyntax (
            InvocationExpressionBuilder invkBld )
                => invkBld == null ? null : invkBld.ToInvocation ();

        public static implicit operator ExpressionStatementSyntax (
            InvocationExpressionBuilder invkBld )
                => invkBld == null ? null : invkBld.ToStatement ();


        public InvocationExpressionBuilder AddArgument ( ArgumentSyntax argStx )
        {
            if ( argStx != null ) _AddArgument ( argStx );

            return this;
        } // End of AddArgument (...)

        public InvocationExpressionBuilder AddArgument ( ExpressionSyntax argExprStx )
        {
            if ( argExprStx != null ) _AddArgument ( argExprStx );

            return this;
        } // End of AddArgument (...)

        public InvocationExpressionBuilder AddArgument ( params String [] argsStr )
            => _AddArgument ( SyntaxHelper.IdentifierName ( argsStr ) );

        public InvocationExpressionBuilder AddArgument ( String argStr )
            => AddArgument ( argStr, false );

        public InvocationExpressionBuilder AddArgument ( String argStr, Boolean isIdentifier )
        {
            if ( !String.IsNullOrWhiteSpace ( argStr ) ) _AddArgument ( argStr, isIdentifier );

            return this;
        } // End of AddArgument (...)

        public InvocationExpressionBuilder AddArguments ( IEnumerable<ParameterInfo> piSet )
        {
            if ( piSet != null ) _AddArguments ( piSet );

            return this;
        } // End of AddArguments (...)

        public InvocationExpressionBuilder AddArguments ( params ParameterInfo [] piSet )
            => AddArguments ( piSet as IEnumerable<ParameterInfo> );

        public InvocationExpressionSyntax ToInvocation ()
        {
            InvocationExpressionSyntax retVal;

            retVal = SyntaxFactory.InvocationExpression (
                                        __memberNameStx,
                                        SyntaxFactory.ArgumentList ( __argsStxs.SeparatedList () ) );
            ImplRelease ( this );

            return retVal;
        } // End of ToInvocation ()

        public ReturnStatementSyntax ToReturnStatement ()
            => SyntaxFactory.ReturnStatement ( ToInvocation () );

        public ExpressionStatementSyntax ToStatement ()
            => SyntaxFactory.ExpressionStatement ( ToInvocation () );


        protected override InvocationExpressionBuilder ImplReset ()
        {
            if ( __argsStxs != null ) __argsStxs.Clear ();

            __memberNameStx = null;

            return this;
        } // End of ImplReset ()


        private List<ArgumentSyntax> _ArgsStxs
        {
            get
            {
                if ( __argsStxs == null ) __argsStxs = new List<ArgumentSyntax> ();

                return __argsStxs;
            }
        }

        private InvocationExpressionBuilder _AddArgument ( ArgumentSyntax argStx )
        {
            _ArgsStxs.Add ( argStx );

            return this;
        } // End of _AddArgument (...)

        private InvocationExpressionBuilder _AddArguments ( IEnumerable<ParameterInfo> piSet )
        {
            foreach ( ParameterInfo pi in piSet )
                _AddArgument ( SyntaxHelper.IdentifierName ( pi.Name ) );

            return this;
        } // End of _AddArguments (...)

        private InvocationExpressionBuilder _AddArgument ( ExpressionSyntax argExprStx )
            => _AddArgument ( SyntaxFactory.Argument ( argExprStx ) );

        private InvocationExpressionBuilder _AddArgument ( String argStr, Boolean isIdentifier  )
            => isIdentifier ? _AddArgument ( SyntaxHelper.IdentifierName ( argStr ) )
                            : _AddArgument ( SyntaxHelper.StringLiteralExpression ( argStr ) );
    } // End of Class InvocationExpressionBuilder
} // End of Namespace Sharpframework.Roslyn.CSharp
