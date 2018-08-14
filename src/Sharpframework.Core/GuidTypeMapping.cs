using System;
using System.Collections.Generic;

using Sharpframework.Core.GuidExtension;


namespace Sharpframework.Core
{
    public sealed class GuidTypeMapping
        : KeyedCollectionBase<Guid, GuidTypeMapping.Item>
    {
        public class Item
        {
            public Item ( Guid guid )
                : this ( guid, null ) { }

            public Item ( Guid guid, Type type )
            { Guid = guid; Type = type; }

            public readonly Guid Guid;
            public Type Type { get; set; }
        } // End of Class GuidTypeMapping


        private static GuidTypeMapping __singleton;


        static GuidTypeMapping () { __singleton = null; }


        public static GuidTypeMapping Singleton
        {
            get
            {
                if ( __singleton == null )
                {
                    __singleton = new GuidTypeMapping ();

                    __singleton.Add ( typeof ( Boolean  ) );
                    __singleton.Add ( typeof ( Byte     ) );
                    __singleton.Add ( typeof ( Char     ) );
                    __singleton.Add ( typeof ( DateTime ) );
                    __singleton.Add ( typeof ( Decimal  ) );
                    __singleton.Add ( typeof ( Double   ) );
                    __singleton.Add ( typeof ( Int16    ) );
                    __singleton.Add ( typeof ( Int32    ) );
                    __singleton.Add ( typeof ( Int64    ) );
                    __singleton.Add ( typeof ( Object   ) );
                    __singleton.Add ( typeof ( SByte    ) );
                    __singleton.Add ( typeof ( Single   ) );
                    __singleton.Add ( typeof ( String   ) );
                    __singleton.Add ( typeof ( UInt16   ) );
                    __singleton.Add ( typeof ( UInt32   ) );
                    __singleton.Add ( typeof ( UInt64   ) );
                }

                return __singleton;
            }
        }

        public void Add ( Type type ) => Add ( type.GetTypeGuid (), type );

        public void Add ( Guid guid, Type type )
        { Add ( new Item ( guid, type ) ); }


        public IEnumerable<Guid> Guids
        {
            get
            {
                foreach ( Item item in this )
                    yield return item.Guid;

                yield break;
            }
        }

        public IEnumerable<Type> Types
        {
            get
            {
                foreach ( Item item in this )
                    yield return item.Type;

                yield break;
            }
        }


        public Boolean TryGetValue ( Guid key, out Type type )
        {
            Boolean retVal;
            Item    item;

            type = (retVal = TryGetValue ( key, out item )) ? item.Type : null;

            return retVal;
        } // End of TryGetValue (...)


        protected override Guid GetKeyForItem ( Item item )
            => item == null ? Guid.Empty : item.Guid;
    } // End of Class GuidTypeMapping
} // End of Namespace Sharpframework.Core
