using System;


namespace Sharpframework.Core
{
    public interface IValueProvider
    {
        Object Value { get; }
    } // End of Interface IValueProvider

    public interface IValueProvider<ValueType>
    {
        ValueType Value { get; }
    } // End of Interface IValueProvider<...>
} // End of Namespace Sharpframework.Core
