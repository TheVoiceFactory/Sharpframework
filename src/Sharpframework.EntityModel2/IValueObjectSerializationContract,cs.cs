using System;
using System.Collections.Generic;
using System.Text;

namespace Sharpframework.EntityModel
{
    using Sharpframework.Serialization;

    public interface IValueObjectSerializationContract
        : IContractBase
    {
        Type GetSerializationContract<SerializationContextType> (
            SerializationContextType context )
        where SerializationContextType : ISerializationContext;
    } // End of Interface IValueObjectSerializationContract
} // End of Namespace Sharpframework.EntityModel
