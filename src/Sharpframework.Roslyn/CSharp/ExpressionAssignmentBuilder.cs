using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Sharpframework.Core;


namespace Sharpframework.Roslyn.CSharp
{
    public class ExpressionAssignmentBuilder
        : AssignmenBuilder<ExpressionAssignmentBuilder>
    {
        private ExpressionSyntax __right;


        protected ExpressionAssignmentBuilder () => ImplReset ();


        public static implicit operator AssignmentExpressionSyntax (
            ExpressionAssignmentBuilder bld )
                => bld == null ? null : bld.ToAssignment ();

        public static implicit operator ExpressionStatementSyntax (
            ExpressionAssignmentBuilder bld )
                => bld == null ? null : bld.ToStatement ();


        public static ExpressionAssignmentBuilder Allocate ( String left, ExpressionSyntax right )
            => Allocate ( left, right, AssignmentKind.Assign );

        public static ExpressionAssignmentBuilder Allocate (
            IEnumerable<String> left, ExpressionSyntax right )
                => Allocate ( left, right, AssignmentKind.Assign );

        public static ExpressionAssignmentBuilder Allocate (
            NameBuilder left, ExpressionSyntax right )
                => Allocate ( left, right, AssignmentKind.Assign );

        public static ExpressionAssignmentBuilder Allocate (
            String left, ExpressionSyntax right, AssignmentKind kind )
                => Allocate ( left.AsEnumerable (), right, kind );

        public static ExpressionAssignmentBuilder Allocate (
            IEnumerable<String> left, ExpressionSyntax right, AssignmentKind kind )
        {
            if ( right == null )
                throw new ArgumentException ( "Invalid right side operand" );

            ExpressionAssignmentBuilder retVal = _GetInstance;

            retVal.ImplInit ( left, kind );

            retVal.__right = right;

            return retVal;
        } // End of Allocate (...)

        public static ExpressionAssignmentBuilder Allocate (
            NameBuilder left, ExpressionSyntax right, AssignmentKind kind )
        {
            if ( right == null )
                throw new ArgumentException ( "Invalid right side operand" );

            ExpressionAssignmentBuilder retVal = _GetInstance;

            retVal.ImplInit ( left, kind );

            retVal.__right = right;

            return retVal;
        } // End of Allocate (...)


        public AssignmentExpressionSyntax ToAssignment ()
        {
            AssignmentExpressionSyntax retVal = null;

            retVal = SyntaxFactory.AssignmentExpression ( ImplTranslatedKind, ImplLeft, __right );

            if ( ImplAutoRelease ) ImplRelease ( this );

            return retVal;
        } // End of ToAssignment ()

        public ExpressionStatementSyntax ToStatement ()
            => SyntaxFactory.ExpressionStatement ( ToAssignment () );

        protected override AssignmenBuilder<ExpressionAssignmentBuilder> ImplReset ()
        {
            __right = null;

            return base.ImplReset ();
        } // End of ImplReset ()


        private static ExpressionAssignmentBuilder _GetInstance
        {
            get
            {
                ExpressionAssignmentBuilder retVal = ImplGetRecycled () as ExpressionAssignmentBuilder;

                return retVal == null ? new ExpressionAssignmentBuilder () : retVal;
            }
        }
    } // End of Class ExpressionAssignmentBuilder
} // End of Namespace Sharpframework.Roslyn.CSharp
