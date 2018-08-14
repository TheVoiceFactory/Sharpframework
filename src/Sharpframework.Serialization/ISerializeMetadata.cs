using System;


namespace Sharpframework.Serialization
{
    public interface ISerializeMetadata
        : ISerializeData
    {
        Boolean SerializeMetadata { get; set; }
    } // End of Interface ISerializeMetadata
} // End of Namespace Sharpframework.Serialization
