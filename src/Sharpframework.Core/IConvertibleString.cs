using System;


namespace Sharpframework.Core
{
    public interface IConvertibleString
        : IConvertible
    {
        String StringValue { get; set; }
    } // End of Interface IConvertibleString
} // End of Namespace Sharpframework.Core
