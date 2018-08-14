using System.Collections.Generic;


namespace Sharpframework.Core
{
    public interface IHierarchicalMetadataProvider<MetadataRootType, MetadataItemType>
        : IMetadataProvider<MetadataRootType>
    {
        Stack<MetadataItemType> Hierarchy { get; }
    } // End of Interface IHierarchicalMetadataProvider<...>

    public interface IMetadataProvider<MetadataItemType>
    {
        MetadataItemType Metadata { get; }
    } // End of Interface IMetadataProvider<...>
} // End of Namespace Sharpframework.Core
