using System;
using System.Collections.Generic;


namespace Sharpframework.Core
{
    public class DistinctList<ItemType>
        : List<ItemType>
    {
        public DistinctList () : base () { }
        public DistinctList ( Int32 capacity ) : base ( capacity ) { }
        public DistinctList ( IEnumerable<ItemType> collection ) : base ( collection ) { }


        public new void Add ( ItemType item )
        {
            if ( !Contains ( item ) ) base.Add ( item );
        }

        public new void AddRange ( IEnumerable<ItemType> collection )
        {
            if ( collection == null ) return;

            foreach ( ItemType item in collection ) Add ( item );
        }
    }
}
