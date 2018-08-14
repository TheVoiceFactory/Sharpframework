using Sharpframework.Core;


namespace Sharpframework.Serialization.ValueDom
{
    public interface IHierarchicalMetadataSerializationContext
        : IMetadataSerializationContext
        , IHierarchicalMetadataProvider<IValueUnit, IValueItemBase>
    {
    } // End of Interface IHierarchicalMetadataSerializationContext
} // End of Namespace Sharpframework.Serialization.ValueDom
