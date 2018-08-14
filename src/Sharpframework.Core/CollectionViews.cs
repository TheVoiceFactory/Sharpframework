using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Sharpframework.Core
{
    public class CollectionViews<ItemType>
        : ICollection<ItemType>
        , IReadOnlyCollection<ItemType>
        , IReadOnlyList<ItemType>
    {
        private ICollection<ItemType> __collection;


        public CollectionViews ()
        { __collection = new List<ItemType> (); }

        public CollectionViews ( ICollection<ItemType> collection )
        { __collection = collection.IsReadOnly ? _CloneCollection ( collection ) : collection; }

        public CollectionViews ( IEnumerable<ItemType> set )
        {
            ICollection<ItemType> tmp = set as ICollection<ItemType>;

            __collection = tmp == null || tmp.IsReadOnly ? _CloneCollection ( set ) : tmp;
        } // End of Custom Constructor

        public CollectionViews ( IReadOnlyCollection<ItemType> collection )
        {
            ICollection<ItemType> tmp = collection as ICollection<ItemType>;

            __collection = tmp == null || tmp.IsReadOnly ? _CloneCollection ( collection ) : tmp;
        } // End of Custom Constructor


        public ItemType this [ Int32 index ]
        { get { return this.ElementAtOrDefault ( index ); } }

        public Int32 Count { get { return __collection.Count; } }

        public Boolean IsReadOnly { get { return false; } }

        public void Add ( ItemType item )
        { __collection.Add ( item ); }

        public void Clear () { __collection.Clear (); }

        public Boolean Contains ( ItemType item ) { return __collection.Contains ( item ); }

        public void CopyTo ( ItemType [] array, Int32 arrayIndex )
        { __collection.CopyTo ( array, arrayIndex ); }

        public IEnumerator<ItemType> GetEnumerator () { return __collection.GetEnumerator (); }

        public Boolean Remove ( ItemType item ) { return __collection.Remove ( item ); }


        private ICollection<ItemType> _CloneCollection ( IEnumerable<ItemType> collection )
        { return new List<ItemType> ( collection ); }


        IEnumerator IEnumerable.GetEnumerator () { return __collection.GetEnumerator (); }
    } // End of Class CollectionViews<...>
} // End of Namespace Sharpframework.Core
