using System;
using System.Collections;


namespace Sharpframework.Serialization
{
    public abstract class SequenceSerializationManager< SequenceType,
                                                        AdapterType,
                                                        SerializedType,
                                                        SerializationContextType,
                                                        MyselfType>
        : SerializationManager< SequenceType,
                                AdapterType,
                                SerializedType,
                                SerializationContextType,
                                MyselfType>
    where SequenceType              : IEnumerable
    where AdapterType               : class
                                    , ISymbolTableAdapter//<ValueDom.IValueUnit>//IValueSequence>
                                    , new ()
    where SerializedType            : ISymbolTableValueSequence
    where SerializationContextType  : ISerializationContext
    where MyselfType                : SequenceSerializationManager< SequenceType,
                                                                    AdapterType,
                                                                    SerializedType,
                                                                    SerializationContextType,
                                                                    MyselfType>
                                    , new ()
    {
    } // End of Class SequenceSerializationManager<...>
} // End of Namespace Sharpframework.Serialization
