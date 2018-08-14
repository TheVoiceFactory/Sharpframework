using System.Collections.Generic;


namespace Sharpframework.Serialization.ValueDom
{
    using Sharpframework.Serialization;


    public abstract class GenericDictionaryRepositorySerializationManager<  SequenceType,
                                                                            SequenceKeyType,
                                                                            SequenceValueType,
                                                                            AdapterType,
                                                                            MyselfType>
        : GenericDictionarySerializationManager<    SequenceType,
                                                    SequenceKeyType,
                                                    SequenceValueType,
                                                    AdapterType,
                                                    IRepositorySerializationContext,
                                                    MyselfType>
    where SequenceType  : Dictionary<SequenceKeyType, SequenceValueType>
                        , new ()
    where AdapterType   : class
                        , ISymbolTableAdapter<IValueUnit>//IValueSequence>
                        , new ()
    where MyselfType    : GenericDictionaryRepositorySerializationManager<  SequenceType,
                                                                            SequenceKeyType,
                                                                            SequenceValueType,
                                                                            AdapterType,
                                                                            MyselfType>
                        , new ()
    {
        protected override SequenceType ImplDeserialize (
            IValueSequence                  serialization,
            IRepositorySerializationContext context )
        => ImplDeserializeEnumerable ( serialization, context );

        protected override void ImplSerialize (
            out IValueSequence                  retVal,
                SequenceType                    instance,
                IRepositorySerializationContext context )
        => ImplSerializeEnumerable ( out retVal, instance, context );
    } // End of Class GenericDictionaryRepositorySerializationManager<...>

    public abstract class GenericDictionaryRepositorySerializationManager<  SequenceValueType,
                                                                            SequenceKeyType,
                                                                            AdapterType,
                                                                            MyselfType>
        : GenericDictionaryRepositorySerializationManager<
                                                Dictionary<SequenceKeyType, SequenceValueType>,
                                                SequenceKeyType,
                                                SequenceValueType,
                                                AdapterType,
                                                MyselfType>
    where AdapterType   : class
                        , ISymbolTableAdapter<IValueUnit>//IValueSequence>
                        , new ()
    where MyselfType    : GenericDictionaryRepositorySerializationManager<  SequenceValueType,
                                                                            SequenceKeyType,
                                                                            AdapterType,
                                                                            MyselfType>
                        , new ()
    {
    } // End of Class GenericDictionaryRepositorySerializationManager<...>
} // End of Namespace Sharpframework.Serialization.ValueDom
