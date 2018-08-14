using System;
using System.Collections.Generic;


namespace Sharpframework.Core
{
    public interface IFieldsValues
        //: IDisposable
    {
        IEnumerable<String> Names { get; }
        IEnumerable<Object> Values { get; }

        Object this [ String name ] { get; }

        Boolean Equals ( IFieldsValues other );
    } // End of Interface IFieldsValues
} // End of Namespace Sharpframework.Core
