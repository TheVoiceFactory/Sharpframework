using System;
using System.Collections.Generic;

using Sharpframework.Core;


namespace Sharpframework.Serialization
{
    public abstract class MetadataSerializationContext< MetadataRootType,
                                                        MetadataItemType,
                                                        MyselfType>
        : SerializationContext<MyselfType>
        , IHierarchicalMetadataProvider<MetadataRootType, MetadataItemType>
        , ISerializeMetadata
    where MetadataRootType  : MetadataItemType
    where MyselfType        : MetadataSerializationContext< MetadataRootType,
                                                            MetadataItemType,
                                                            MyselfType>
                            , new ()
    {
        private Stack<MetadataItemType> __hierarchy;


        protected MetadataSerializationContext ( MetadataRootType metadata )
        {
            __hierarchy = new Stack<MetadataItemType> ();

            __hierarchy.Push ( metadata );

            Metadata            = metadata;
            SerializeMetadata   = false;
        }


        public Stack<MetadataItemType> Hierarchy { get => __hierarchy; }

        public MetadataRootType Metadata { get; private set; }
        public Boolean SerializeMetadata { get; set; }
    } // End of Class MetadataSerializationContext<...>
} // End of Namespace Sharpframework.Serialization
