using System;
using System.Collections.Generic;


namespace Sharpframework.Serialization.ValueDom
{
    public interface IValueUnit
        : ISymbolTableRoot
        , IValueItemBase
        , IEnumerable<IValueItem>
    {
        IValueItem this [ String key ] { get; }
        Int32 Count { get; }

        void Add ( String key, Object Value );
    } // End of Interface IValueUnit
} // End of Namespace Sharpframework.Serialization.ValueDom
