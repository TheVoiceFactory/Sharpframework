using System;

using System.Collections;
using System.Collections.Generic;


namespace Sharpframework.Core
{
    public interface IKeyedCollection<KeyType, ItemType>
        : IReadOnlyKeyedCollection<KeyType, ItemType>
        , IList<ItemType>
        , IList
    {
        new Int32   Count { get; }

        new ItemType this [ KeyType key ] { get; set; }
    } // End of Interface IKeyedCollection<...>

    public interface IReadOnlyKeyedCollection<KeyType, ItemType>
        : IReadOnlyList<ItemType>
    {
        Boolean ContainsKey ( KeyType key );

        ItemType this [ KeyType key ] { get; }
    } // End of Interface IReadOnlyKeyedCollection<...>
} // End of Namespace Sharpframework.Core
