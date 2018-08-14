
namespace Sharpframework.Serialization.ValueDom
{
    using System.Collections.Generic;


    public abstract class JsonGenericListRepositorySerializationManager<    SequenceType,
                                                                            SequenceItemType,
                                                                            MyselfType>
        : GenericListRepositorySerializationManager<    SequenceType,
                                                        SequenceItemType,
                                                        JsonValueDomAdapter,
                                                        MyselfType>
    where SequenceType              : List<SequenceItemType>
                                    , new ()
    where MyselfType                : JsonGenericListRepositorySerializationManager<    
                                                                            SequenceType,
                                                                            SequenceItemType,
                                                                            MyselfType>
                                    , new ()
    {
    } // End of Class JsonGenericListRepositorySerializationManager<...>

    public abstract class JsonGenericListRepositorySerializationManager<    SequenceItemType,
                                                                            MyselfType>
        : JsonGenericListRepositorySerializationManager<    List<SequenceItemType>,
                                                            SequenceItemType,
                                                            MyselfType>
    where MyselfType                : JsonGenericListRepositorySerializationManager<    
                                                                            SequenceItemType,
                                                                            MyselfType>
                                    , new ()
    {
    } // End of Class JsonGenericListRepositorySerializationManager<...>

    public sealed class JsonGenericListRepositorySerializationManager<SequenceItemType>
        : JsonGenericListRepositorySerializationManager<
                SequenceItemType,
                JsonGenericListRepositorySerializationManager<SequenceItemType>>
    {
    } // End of Class JsonGenericListRepositorySerializationManager<...>
} // End of Namespace Sharpframework.Serialization.ValueDom
