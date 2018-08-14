
namespace Sharpframework.Serialization.ValueDom
{
    using Sharpframework.Serialization;


    public abstract class ArrayRepositorySerializationManager<  ArrayItemType,
                                                                AdapterType,
                                                                MyselfType>
        : ArraySerializationManager<    ArrayItemType,
                                        AdapterType,
                                        IRepositorySerializationContext,
                                        MyselfType>
    where AdapterType   : class
                        , ISymbolTableAdapter<IValueUnit>//IValueSequence>
                        , new ()
    where MyselfType    : ArrayRepositorySerializationManager<  ArrayItemType,
                                                                AdapterType,
                                                                MyselfType>
                        , new ()
    {
        protected override ArrayItemType [] ImplDeserialize (
            IValueSequence                  serialization,
            IRepositorySerializationContext context )
        => ImplDeserializeEnumerable ( serialization, context );

        protected override void ImplSerialize (
            out IValueSequence                  retVal,
                ArrayItemType []                instance,
                IRepositorySerializationContext context )
        => ImplSerializeEnumerable ( out retVal, instance, context );
    } // End of Class ArrayRepositorySerializationManager<...>
} // End of Namespace Sharpframework.Serialization.ValueDom
