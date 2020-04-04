using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Sharpframework.Core;


namespace Sharpframework.Roslyn.CSharp
{
    public class IdentifierAssignmentBuilder
        : AssignmenBuilder<IdentifierAssignmentBuilder>
    {
        private NameBuilder __right;


        protected IdentifierAssignmentBuilder () => ImplReset ();


        public static implicit operator AssignmentExpressionSyntax (
            IdentifierAssignmentBuilder bld )
                => bld == null ? null : bld.ToAssignment ();

        public static implicit operator ExpressionStatementSyntax (
            IdentifierAssignmentBuilder bld )
                => bld == null ? null : bld.ToStatement ();


        public static IdentifierAssignmentBuilder Allocate ( String left, String right )
            => Allocate ( left, right, AssignmentKind.Assign );

        public static IdentifierAssignmentBuilder Allocate (
            IEnumerable<String> left, String right )
                => Allocate ( left, right, AssignmentKind.Assign );

        public static IdentifierAssignmentBuilder Allocate (
            String left, IEnumerable<String> right )
                => Allocate ( left, right, AssignmentKind.Assign );

        public static IdentifierAssignmentBuilder Allocate (
            IEnumerable<String> left, IEnumerable<String> right )
                => Allocate ( left, right, AssignmentKind.Assign );

        public static IdentifierAssignmentBuilder Allocate ( String left, NameBuilder right )
            => Allocate ( left, right, AssignmentKind.Assign );

        public static IdentifierAssignmentBuilder Allocate (
            IEnumerable<String> left, NameBuilder right )
                => Allocate ( left, right, AssignmentKind.Assign );

        public static IdentifierAssignmentBuilder Allocate ( NameBuilder left, String right )
            => Allocate ( left, right, AssignmentKind.Assign );

        public static IdentifierAssignmentBuilder Allocate (
            NameBuilder left, IEnumerable<String> right )
                => Allocate ( left, right, AssignmentKind.Assign );

        public static IdentifierAssignmentBuilder Allocate ( NameBuilder left, NameBuilder right )
            => Allocate ( left, right, AssignmentKind.Assign );

        public static IdentifierAssignmentBuilder Allocate (
            String left, String right, AssignmentKind kind )
                => Allocate ( left.AsEnumerable (), right.AsEnumerable (), kind );

        public static IdentifierAssignmentBuilder Allocate (
            IEnumerable<String> left, IEnumerable<String> right, AssignmentKind kind )
        {
            if ( right == null )
                throw new ArgumentException ( "Invalid right side operand" );

            IdentifierAssignmentBuilder retVal = _GetInstance;

            retVal.ImplInit ( left, kind );

            if ( retVal.__right == null )
                retVal.__right = SyntaxHelper.Name ( right ).DisableAutoRelease ();
            else
                retVal.__right.Assign ( right );

            return retVal;
        } // End of Allocate (...)

        public static IdentifierAssignmentBuilder Allocate (
            String left, NameBuilder right, AssignmentKind kind )
                => Allocate ( left.AsEnumerable (), right, kind );

        public static IdentifierAssignmentBuilder Allocate (
            IEnumerable<String> left, NameBuilder right, AssignmentKind kind )
        {
            if ( right == null )
                throw new ArgumentException ( "Invalid right side operand" );

            IdentifierAssignmentBuilder retVal = _GetInstance;

            retVal.ImplInit ( left, kind );

            if ( retVal.__right != right )
            {
                NameBuilder.ImplRelease ( retVal.__right );
                retVal.__right = right.DisableAutoRelease ();
            }

            return retVal;
        } // End of Allocate (...)

        public static IdentifierAssignmentBuilder Allocate (
            NameBuilder left, String right, AssignmentKind kind )
                => Allocate ( left, right.AsEnumerable (), kind );

        public static IdentifierAssignmentBuilder Allocate (
            NameBuilder left, IEnumerable<String> right, AssignmentKind kind )
        {
            if ( right == null )
                throw new ArgumentException ( "Invalid right side operand" );

            IdentifierAssignmentBuilder retVal = _GetInstance;

            retVal.ImplInit ( left, kind );

            if ( retVal.__right == null )
                retVal.__right = SyntaxHelper.Name ( right ).DisableAutoRelease ();
            else
                retVal.__right.Assign ( right );

            return retVal;
        } // End of Allocate (...)

        public static IdentifierAssignmentBuilder Allocate (
            NameBuilder left, NameBuilder right, AssignmentKind kind )
        {
            if ( right == null )
                throw new ArgumentException ( "Invalid right side operand" );

            IdentifierAssignmentBuilder retVal = _GetInstance;

            retVal.ImplInit ( left, kind );

            if ( retVal.__right != null )
            {
                NameBuilder.ImplRelease ( retVal.__right );
                retVal.__right = right.DisableAutoRelease ();
            }

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

        public IdentifierAssignmentBuilder WithRightPrefix ( String rightPrefix )
        {
            if ( String.IsNullOrWhiteSpace ( rightPrefix )  ) return this;
            if ( __right == null                            ) return this;

            __right.Prepend ( rightPrefix );

            return this;
        } // End of WithRightPrefix (...)

        protected override AssignmenBuilder<IdentifierAssignmentBuilder> ImplReset ()
        {
            if ( __right != null ) __right.Clear ();

            return base.ImplReset ();
        } // End of ImplReset ()


        private static IdentifierAssignmentBuilder _GetInstance
        {
            get
            {
                IdentifierAssignmentBuilder retVal = ImplGetRecycled () as IdentifierAssignmentBuilder;

                return retVal == null ? new IdentifierAssignmentBuilder () : retVal;
            }
        }
    } // End of Class IdentifierAssignmentBuilder
} // End of Namespace Sharpframework.Roslyn.CSharp
