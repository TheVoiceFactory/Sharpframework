using System.Collections;
using System.Collections.Generic;


namespace Sharpframework.Serialization.ValueDom
{
    using Sharpframework.Serialization;


    public abstract class GenericListSerializationManager<  SequenceType,
                                                            SequenceItemType,
                                                            AdapterType,
                                                            SerializationContextType,
                                                            MyselfType>
        : ValueSequenceSerializationManager<    SequenceType,
                                                SequenceItemType,
                                                AdapterType,
                                                SerializationContextType,
                                                MyselfType>
    where SequenceType              : List<SequenceItemType>
                                    , new ()
    where AdapterType               : class
                                    , ISymbolTableAdapter<IValueUnit>//IValueSequence>
                                    , new ()
    where SerializationContextType  : ISerializationContext
    where MyselfType                : GenericListSerializationManager<  SequenceType,
                                                                        SequenceItemType,
                                                                        AdapterType,
                                                                        SerializationContextType,
                                                                        MyselfType>
                                    , new ()
    {
        protected override SequenceType ImplPopulateSequence (  SequenceType    sequence,
                                                                IEnumerator     enumerator )
            => base.ImplPopulateSequence (  sequence == null? new SequenceType () : sequence,
                                            enumerator );
    } // End of Class GenericListSerializationManager
} // End of Namespace Sharpframework.Serialization.ValueDom
