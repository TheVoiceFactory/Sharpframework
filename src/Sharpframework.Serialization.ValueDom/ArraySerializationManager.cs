
namespace Sharpframework.Serialization.ValueDom
{
    using Sharpframework.Serialization;


    public abstract class ArraySerializationManager<    ArrayItemType,
                                                        AdapterType,
                                                        SerializationContextType,
                                                        MyselfType>
        : ArrayBaseSerializationManager<    ArrayItemType [],
                                            ArrayItemType,
                                            AdapterType,
                                            SerializationContextType,
                                            MyselfType>
    where AdapterType               : class
                                    , ISymbolTableAdapter<IValueUnit>//IValueSequence>
                                    , new ()
    where SerializationContextType  : ISerializationContext
    where MyselfType                : ArraySerializationManager<    ArrayItemType,
                                                                    AdapterType,
                                                                    SerializationContextType,
                                                                    MyselfType>
                                    , new ()
    {
    } // End of Class ArraySerializationManager<...>
} // End of Namespace Sharpframework.Serialization.ValueDom
