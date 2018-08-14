using System;


namespace Sharpframework.Core
{
    [ AttributeUsage ( AttributeTargets.Property, AllowMultiple = true, Inherited = false ) ]
    public class BelongingSetAttribute : Attribute
    {
        private static Object __mainSet;


        static BelongingSetAttribute () { __mainSet = null; }

        public BelongingSetAttribute () : this ( MainSet ) { }

        public BelongingSetAttribute ( Object targetSet ) : this ( targetSet, null ) { }

        public BelongingSetAttribute ( Object targetSet, String name )
        {
            BelongingSet    = targetSet == null ? MainSet : targetSet;
            Name            = name;
        }


        public static Object MainSet
        {
            get
            {
                if ( __mainSet == null ) __mainSet = new Object ();

                return __mainSet;
            }
        }


        public readonly Object BelongingSet;
        public readonly String Name;
    } // End of Class BelongingSetAttribute
} // End of Namespace Sharpframework.Core
