using Sharpframework.Core;


namespace Sharpframework.Serialization.ValueDom
{
    using Sharpframework.Serialization;

    public interface IMetadataSerializationContext
        : ISerializationContext
        , IMetadataProvider<IValueUnit>
        , ISerializeMetadata
    {
    } // End of Interface IMetadataSerializationContext
} // End of Namespace Sharpframework.Serialization.ValueDom
