using System;


namespace Sharpframework.Core
{
    public interface IValueSetter
    {
        Boolean SetValue<ValueType> ( ValueType value );
    } // End of Interface IValueSetter
} // End of Namespace Sharpframework.Core
