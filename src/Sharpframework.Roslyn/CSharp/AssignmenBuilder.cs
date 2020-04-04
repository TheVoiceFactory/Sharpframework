using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;

using Sharpframework.Core;


namespace Sharpframework.Roslyn.CSharp
{
    public enum AssignmentKind
    {
        Assign,
        Increment,
        Decrement,
        Multiply,
        Divide,
        And,
        Or
    } // End of Enum AssignmentKind


    public class AssignmenBuilder<BuilderType>
            : BuilderBase<AssignmenBuilder<BuilderType>>
        where BuilderType : AssignmenBuilder<BuilderType>
    {
        private AssignmentKind  __kind;
        private NameBuilder     __left;

        protected AssignmenBuilder () => ImplReset ();


        protected AssignmentKind ImplKind { get => __kind; }

        protected NameBuilder ImplLeft { get => __left; }

        protected SyntaxKind ImplTranslatedKind
        {
            get
            {
                switch ( __kind )
                {
                    case AssignmentKind.Assign: return SyntaxKind.SimpleAssignmentExpression;
                }

                return SyntaxKind.SimpleAssignmentExpression;
            }
        }


        protected void ImplInit ( String left, AssignmentKind kind )
            => ImplInit ( left.AsEnumerable (), kind );

        protected void ImplInit ( IEnumerable<String> left, AssignmentKind kind )
        {
            if ( left == null )
                throw new ArgumentException ( "Invalid left side operand" );

            __kind = kind;

            if ( __left == null )
                __left = SyntaxHelper.Name ( left ).DisableAutoRelease ();
            else
                __left.Assign ( left );
        } // End of ImplInit (...)

        protected void ImplInit ( NameBuilder left, AssignmentKind kind )
        {
            if ( left == null )
                throw new ArgumentException ( "Invalid left side operand" );

            __kind = kind;

            if ( __left == left ) return;

            NameBuilder.ImplRelease ( __left );
            __left = left.DisableAutoRelease ();
        } // End of ImplInit (...)


        public BuilderType WithLeftPrefix ( String leftPrefix )
        {
            if ( !String.IsNullOrWhiteSpace ( leftPrefix ) ) __left.Prepend ( leftPrefix );

            return this as BuilderType;
        } // End of WithLeftPrefix (...)

        protected override AssignmenBuilder<BuilderType> ImplReset ()
        {
            __kind = AssignmentKind.Assign;

            if ( __left != null ) __left.Clear ();

            return this;
        } // End of ImplReset ()
    } // End of Class AssignmenBuilder <...>
} // End of Namespace Sharpframework.Roslyn.CSharp
