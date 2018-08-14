
namespace Sharpframework.Serialization.ValueDom
{
    using Sharpframework.Serialization;

    public abstract class ValueDomMetadataSerializationContext< MetadataRootType,
                                                                MetadataItemType,
                                                                MyselfType>
        : MetadataSerializationContext<MetadataRootType, MetadataItemType, MyselfType>
    where MetadataRootType  : MetadataItemType
                            , IValueUnit
    where MetadataItemType  : IValueItemBase
    where MyselfType        : ValueDomMetadataSerializationContext< MetadataRootType,
                                                                    MetadataItemType,
                                                                    MyselfType>
                            , new ()
    {
        protected ValueDomMetadataSerializationContext ( MetadataRootType metadata )
            : base ( metadata ) { }
    } // End of Class ValueDomMetadataSerializationContext<...>


    public abstract class ValueDomMetadataSerializationContext<MyselfType>
        : ValueDomMetadataSerializationContext<IValueUnit, IValueItemBase, MyselfType>
    where MyselfType    : ValueDomMetadataSerializationContext<MyselfType>
                        , new ()
    {
        protected ValueDomMetadataSerializationContext ( IValueUnit metadata )
            : base ( metadata ) { }
    } // End of Class ValueDomMetadataSerializationContext<...>
} // End of Namespace Sharpframework.Serialization.ValueDom
