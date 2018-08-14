using System.Collections.Generic;


namespace Sharpframework.Serialization.ValueDom
{
    using Sharpframework.Serialization;


    public abstract class GenericListRepositorySerializationManager<    SequenceType,
                                                                        SequenceItemType,
                                                                        AdapterType,
                                                                        MyselfType>
        : GenericListSerializationManager<  SequenceType,
                                            SequenceItemType,
                                            AdapterType,
                                            IRepositorySerializationContext,
                                            MyselfType>
    where SequenceType              : List<SequenceItemType>
                                    , new ()
    where AdapterType               : class
                                    , ISymbolTableAdapter<IValueUnit>//IValueSequence>
                                    , new ()
    where MyselfType                : GenericListRepositorySerializationManager<    SequenceType,
                                                                                    SequenceItemType,
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
    } // End of Class GenericListRepositorySerializationManager<...>

    public abstract class GenericListRepositorySerializationManager<    SequenceItemType,
                                                                        AdapterType,
                                                                        MyselfType>
        : GenericListRepositorySerializationManager<    List<SequenceItemType>,
                                                        SequenceItemType,
                                                        AdapterType,
                                                        MyselfType>
    where AdapterType               : class
                                    , ISymbolTableAdapter<IValueUnit>//IValueSequence>
                                    , new ()
    where MyselfType                : GenericListRepositorySerializationManager<    SequenceItemType,
                                                                                    AdapterType,
                                                                                    MyselfType>
                                    , new ()
    {
    } // End of Class GenericListRepositorySerializationManager<...>
} // End of Namespace Sharpframework.Serialization.ValueDom
