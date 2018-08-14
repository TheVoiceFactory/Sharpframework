using System;

using Sharpframework.Core;


namespace Sharpframework.Serialization.ValueDom
{
    public interface IValueItem<KeyType>
        : IValueItemBase
        , IKeyProvider<KeyType>
    {
        IValueItemObjectValueBinder ValueBinder { get; set; }

        Boolean AlignBoundValueToValue ();
        Boolean AlignValueToBoundValue ();
    } // End of Interface IValueItem<...>


    public interface IValueItem : IValueItem<String> { }
} // End of Namespace Sharpframework.Serialization.ValueDom
