using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Sharpframework.Roslyn.CSharp
{
    public class NameBuilder
        : BuilderBase<NameBuilder>
    {
        protected enum AttachMode { Tail, Head }


        private List<String> __strTokens;


        protected NameBuilder () => ImplReset ();


        public static NameBuilder Allocate ( params String [] nameStrTokens )
        {
            if ( nameStrTokens == null ) throw new ArgumentNullException ();

            return ImplAllocate ( nameStrTokens );
        } // End of Allocate (...)

        public static NameBuilder Allocate ( IEnumerable<String> nameStrTokens )
        {
            if ( nameStrTokens == null ) throw new ArgumentNullException ();

            return ImplAllocate ( nameStrTokens );
        } // End of Allocate (...)

        protected static NameBuilder ImplAllocate ( IEnumerable<String> nameStrTokens )
        {
            NameBuilder retVal = _GetInstance;

            retVal.ImplAttach ( nameStrTokens );

            if ( retVal.__strTokens.Count < 1 )
                throw new ArgumentException ( "Invalid empty name" );

            return retVal;
        } // End of Allocate (...)


        public static implicit operator NameBuilder ( String nameStrToken )
            => Allocate ( nameStrToken );

        public static implicit operator NameSyntax ( NameBuilder bld )
            => bld == null ? null : bld.ToName ();


        public NameBuilder Append ( params String [] nameStrTokens )
            => Append ( nameStrTokens as IEnumerable<String> );

        public NameBuilder Append ( IEnumerable<String> nameStrTokens )
            => nameStrTokens == null ? this : ImplAttach ( nameStrTokens );

        public NameBuilder Assign ( params String [] nameStrTokens )
            => Assign ( nameStrTokens as IEnumerable<String> );

        public NameBuilder Assign ( IEnumerable<String> nameStrTokens )
        {
            if ( nameStrTokens == null ) return this;

            Clear ();

            return Append ( nameStrTokens );
        } // End of Assign (...)

        public NameBuilder Prepend ( params String [] nameStrTokens )
            => Prepend ( nameStrTokens as IEnumerable<String> );

        public NameBuilder Prepend ( IEnumerable<String> nameStrTokens )
            => nameStrTokens == null ? this : ImplAttach ( nameStrTokens, AttachMode.Head );

        public NameBuilder Clear ()
        {
            __strTokens.Clear ();

            return this;
        } // End of Clear ()

        public NameSyntax ToName ()
        {
            NameSyntax retVal = SyntaxHelper.IdentifierName ( __strTokens );

            if ( ImplAutoRelease ) ImplRelease ( this );

            return retVal;
        } // End of ToName ()

        protected override NameBuilder ImplReset ()
        {
            if ( __strTokens == null )
                __strTokens = new List<String> ();
            else
                __strTokens.Clear ();

            return base.ImplReset ();
        } // End of ImplReset ()

        protected void ImplAttach ( String nameStrToken, AttachMode attachMode )
        {
            if ( nameStrToken == null                                               ) return;
            if ( (nameStrToken = nameStrToken.Trim ( ' ', '.' ) ) == String.Empty   ) return;

            foreach ( String subTk in nameStrToken.Split ( '.' ) )
                if ( String.IsNullOrWhiteSpace ( subTk ) )
                    throw new ArgumentException ( "Invalid empty name token" );
                else if ( attachMode == AttachMode.Tail )
                    __strTokens.Add ( subTk );
                else
                    __strTokens.Insert ( 0, subTk );

            return;
        } // End of ImplAttach (...)

        protected NameBuilder ImplAttach ( IEnumerable<String> nameStrTokens )
            => ImplAttach ( nameStrTokens, AttachMode.Tail );

        protected NameBuilder ImplAttach (
            IEnumerable<String> nameStrTokens,
            AttachMode          attachMode )
        {
            foreach ( String token in nameStrTokens ) ImplAttach ( token, attachMode );

            return this;
        } // End of ImplAttach (...)


        private static NameBuilder _GetInstance
        {
            get
            {
                NameBuilder retVal = ImplGetRecycled ();

                return retVal == null ? new NameBuilder () : retVal;
            }
        }
    } // End of Class NameBuilder
} // End of Namespace Sharpframework.Roslyn.CSharp
