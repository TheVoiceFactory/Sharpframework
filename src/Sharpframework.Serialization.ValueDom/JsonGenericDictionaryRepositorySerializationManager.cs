
namespace Sharpframework.Serialization.ValueDom
{
    using System.Collections.Generic;


    public abstract class JsonGenericDictionaryRepositorySerializationManager<  SequenceType,
                                                                                SequenceKeyType,
                                                                                SequenceValueType,
                                                                                MyselfType>
        : GenericDictionaryRepositorySerializationManager<  SequenceType,
                                                            SequenceKeyType,
                                                            SequenceValueType,
                                                            JsonValueDomAdapter,
                                                            MyselfType>
    where SequenceType              : Dictionary<SequenceKeyType, SequenceValueType>
                                    , new ()
    where MyselfType                : JsonGenericDictionaryRepositorySerializationManager<    
                                                                                SequenceType,
                                                                                SequenceKeyType,
                                                                                SequenceValueType,
                                                                                MyselfType>
                                    , new ()
    {
    } // End of Class JsonGenericListRepositorySerializationManager<...>

    public abstract class JsonGenericDictionaryRepositorySerializationManager<  SequenceKeyType,
                                                                                SequenceItemType,
                                                                                MyselfType>
        : JsonGenericDictionaryRepositorySerializationManager<
                                                    Dictionary<SequenceKeyType, SequenceItemType>,
                                                    SequenceKeyType,
                                                    SequenceItemType,
                                                    MyselfType>
    where MyselfType                : JsonGenericDictionaryRepositorySerializationManager<    
                                                                                SequenceKeyType,
                                                                                SequenceItemType,
                                                                                MyselfType>
                                    , new ()
    {
    } // End of Class JsonGenericListRepositorySerializationManager<...>

    public sealed class JsonGenericDictionaryRepositorySerializationManager<    SequenceKeyType,
                                                                                SequenceItemType>
        : JsonGenericDictionaryRepositorySerializationManager<
                SequenceKeyType,
                SequenceItemType,
                JsonGenericDictionaryRepositorySerializationManager<SequenceKeyType,
                                                                    SequenceItemType>>
    {
    } // End of Class JsonGenericListRepositorySerializationManager<...>
} // End of Namespace Sharpframework.Serialization.ValueDom
