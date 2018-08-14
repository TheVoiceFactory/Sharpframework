using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Sharpframework.Core
{
    public abstract class KeyedCollectionBase<KeyType, ItemType>
        : KeyedCollection<KeyType, ItemType>
        , IKeyedCollection<KeyType, ItemType>
    {
        public KeyedCollectionBase () { }

        protected KeyedCollectionBase ( IEqualityComparer<KeyType> comparer )
            : base ( comparer ) { }

        protected KeyedCollectionBase (
            IEqualityComparer<KeyType> comparer, Int32 dictionaryCreationThreshold )
            : base ( comparer, dictionaryCreationThreshold ) { }


        public new ItemType this [ KeyType key ]
        {
            get
            {
                ItemType value;

                return TryGetValue ( key, out value ) ? value : default ( ItemType );
            }

            set { Remove ( key ); Add ( value ); }
        }


        public Boolean ContainsKey ( KeyType key )
        { return Contains ( key ); }


        Int32 IKeyedCollection<KeyType, ItemType>.Count
        { get { return Count;} }
    } // End of lass KeyedCollectionBase<...>
} // End of Namespace Sharpframework.Core
