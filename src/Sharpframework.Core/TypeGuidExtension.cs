using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


namespace Sharpframework.Core.GuidExtension
{
    public static class TypeGuidExtension
    {
        private static MD5                      __md5;
        private static Dictionary<Type, Guid>   __typeGuids; // TODO: Integrare con Sharpframework.Core.GuidTypeMapping

        static TypeGuidExtension () { __md5 = null; __typeGuids = null; }

        public static Guid GetTypeGuid ( this Object target )
            => target == null ? Guid.Empty : target.GetType ().GetTypeGuid ();

        public static Guid GetTypeGuid ( this Type targetType )
        {
            if ( _TypeGuids.TryGetValue ( targetType, out Guid guid ) ) return guid;

            Byte [] inputBytes  = Encoding.ASCII.GetBytes ( targetType.AssemblyQualifiedName );
            Guid    retVal      = new Guid ( _Md5.ComputeHash ( inputBytes ) );

            _TypeGuids.TryAdd ( targetType, retVal );

            return retVal;
        } // End of GetTypeGuid (...)


        private static MD5 _Md5
        { get { if ( __md5 == null ) __md5 = MD5.Create (); return __md5; } }

        private static Dictionary<Type,Guid> _TypeGuids
        {
            get
            {
                if ( __typeGuids == null ) __typeGuids = new Dictionary<Type, Guid> ();

                return __typeGuids;
            }
        }
    } // End of Class TypeGuidExtension
} // End of Namespace Sharpframework.Core.GuidExtension
