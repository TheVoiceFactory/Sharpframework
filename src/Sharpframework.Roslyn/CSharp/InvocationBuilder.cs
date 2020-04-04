using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Sharpframework.Roslyn.CSharp
{
    public class InvocationBuilder
        : BuilderBase<InvocationBuilder>
    {
        List<ArgumentSyntax>    __argsStxs;
        NameBuilder             __nameBld;


        private InvocationBuilder () => ImplReset ();


        public static InvocationBuilder Allocate (
            params String [] methodNameStrTokens )
        {
            InvocationBuilder retVal = _GetInstance;

            if ( retVal.__nameBld == null )
                retVal.__nameBld = SyntaxHelper.Name ( methodNameStrTokens )
                                                    .DisableAutoRelease ();
            else
                retVal.__nameBld.Assign ( methodNameStrTokens );

            return retVal;
        } // End of Allocate (...)

        public static InvocationBuilder Allocate ( NameBuilder methodNameBld )
        {
            if ( methodNameBld == null ) throw new ArgumentNullException ();

            return ImplAllocate ( methodNameBld );
        } // End of Allocate (...)

        protected static InvocationBuilder ImplAllocate ( NameBuilder methodNameBld )
        {
            InvocationBuilder retVal = _GetInstance;

            if ( retVal.__nameBld != methodNameBld )
            {
                NameBuilder.ImplRelease ( retVal.__nameBld );
                retVal.__nameBld = methodNameBld.DisableAutoRelease ();
            }

            return retVal;
        } // End of ImplAllocate (...)


        public static implicit operator InvocationExpressionSyntax ( InvocationBuilder invkBld )
            => invkBld == null ? null : invkBld.ToInvocation ();

        public static implicit operator ExpressionStatementSyntax ( InvocationBuilder invkBld )
            => invkBld == null ? null : invkBld.ToStatement ();


        public InvocationBuilder AddArgument ( ArgumentSyntax argStx )
        {
            if ( argStx != null ) _AddArgument ( argStx );

            return this;
        } // End of AddArgument (...)

        public InvocationBuilder AddArgument ( ExpressionSyntax argExprStx )
        {
            if ( argExprStx != null ) _AddArgument ( argExprStx );

            return this;
        } // End of AddArgument (...)

        public InvocationBuilder AddArgument ( params String [] argsStr )
            => _AddArgument ( SyntaxHelper.IdentifierName ( argsStr ) );

        public InvocationBuilder AddArgument ( String argStr )
            => AddArgument ( argStr, false );

        public InvocationBuilder AddArgument ( String argStr, Boolean isIdentifier )
        {
            if ( !String.IsNullOrWhiteSpace ( argStr ) ) _AddArgument ( argStr, isIdentifier );

            return this;
        } // End of AddArgument (...)

        public InvocationBuilder AddArguments ( IEnumerable<ParameterInfo> piSet )
        {
            if ( piSet != null ) _AddArguments ( piSet );

            return this;
        } // End of AddArguments (...)

        public InvocationBuilder AddArguments ( params ParameterInfo [] piSet )
            => AddArguments ( piSet as IEnumerable<ParameterInfo> );

        public InvocationExpressionSyntax ToInvocation ()
        {
            InvocationExpressionSyntax retVal;

            retVal = SyntaxFactory.InvocationExpression (
                                        __nameBld,
                                        SyntaxFactory.ArgumentList ( __argsStxs.SeparatedList () ) );

            if ( ImplAutoRelease ) ImplRelease ( this );

            return retVal;
        } // End of ToInvocation ()

        public ReturnStatementSyntax ToReturnStatement ()
            => SyntaxFactory.ReturnStatement ( ToInvocation () );

        public ExpressionStatementSyntax ToStatement ()
            => SyntaxFactory.ExpressionStatement ( ToInvocation () );


        protected override InvocationBuilder ImplReset ()
        {
            if ( __argsStxs != null ) __argsStxs.Clear ();
            if ( __nameBld  != null ) __nameBld.Clear ();

            return this;
        } // End of ImplReset ()


        private static InvocationBuilder _GetInstance
        {
            get
            {
                InvocationBuilder retVal = ImplGetRecycled ();

                return retVal == null ? new InvocationBuilder () : retVal;
            }
        }

        private List<ArgumentSyntax> _ArgsStxs
        {
            get
            {
                if ( __argsStxs == null ) __argsStxs = new List<ArgumentSyntax> ();

                return __argsStxs;
            }
        }

        private InvocationBuilder _AddArgument ( ArgumentSyntax argStx )
        {
            _ArgsStxs.Add ( argStx );

            return this;
        } // End of _AddArgument (...)

        private InvocationBuilder _AddArguments ( IEnumerable<ParameterInfo> piSet )
        {
            foreach ( ParameterInfo pi in piSet )
                _AddArgument ( SyntaxHelper.IdentifierName ( pi.Name ) );

            return this;
        } // End of _AddArguments (...)

        private InvocationBuilder _AddArgument ( ExpressionSyntax argExprStx )
            => _AddArgument ( SyntaxFactory.Argument ( argExprStx ) );

        private InvocationBuilder _AddArgument ( String argStr, Boolean isIdentifier  )
            => isIdentifier ? _AddArgument ( SyntaxHelper.IdentifierName ( argStr ) )
                            : _AddArgument ( SyntaxHelper.StringLiteralExpression ( argStr ) );
    } // End of Class InvocationBuilder
} // End of Namespace Sharpframework.Roslyn.CSharp
