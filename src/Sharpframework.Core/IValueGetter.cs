using System;


namespace Sharpframework.Core
{
    public interface IValueGetter
    {
        Boolean GetValue<ValueType> ( ref ValueType value );
    } // End of Interface IValueGetter
} // End of Namespace Sharpframework.Core
