using System;
using System.Collections.Generic;

using Sharpframework.Core.GuidExtension;

namespace Sharpframework.Serialization.ValueDom
{
    using System.Collections;
    using Sharpframework.Serialization;


    public abstract class GenericDictionarySerializationManager<    SequenceType,
                                                                    SequenceKeyType,
                                                                    SequenceValueType,
                                                                    AdapterType,
                                                                    SerializationContextType,
                                                                    MyselfType>
        : ValueSequenceSerializationManager<    SequenceType,
                                                KeyValuePair<SequenceKeyType, SequenceValueType>,
                                                AdapterType,
                                                SerializationContextType,
                                                MyselfType>
    where SequenceType              : Dictionary<SequenceKeyType, SequenceValueType>
                                    , new ()
    where AdapterType               : class
                                    , ISymbolTableAdapter<IValueUnit>//IValueSequence>
                                    , new ()
    where SerializationContextType  : ISerializationContext
    where MyselfType                : GenericDictionarySerializationManager<
                                                                    SequenceType,
                                                                    SequenceKeyType,
                                                                    SequenceValueType,
                                                                    AdapterType,
                                                                    SerializationContextType,
                                                                    MyselfType>
                                    , new ()
    {
        //protected override Boolean ImplAddSequenceItem ( SequenceType sequence, Object item )
        //{
        //    if ( !(item is SequenceItemType) ) return false;

        //    SequenceItemType kvp = (SequenceItemType) item;

        //    try { sequence.Add ( kvp.Key, kvp.Value ); }
        //    catch { return false; }

        //    return true;
        //} // End of ImplAddSequenceItem (...)

        protected override Boolean ImplDeserializeItem (
            out Object                                      retVal,
                Object                                      item,
                IHierarchicalMetadataSerializationContext   context )
        {
            retVal = null;

            IValueItem                          curCtxtLvlVi    = null;
            KeyValuePair<   SequenceKeyType,
                            SequenceValueType>  kvp;
            IValueUnit                          kvpVu;
            IValueUnit                          kvpCtxtLvl      = null;
            SequenceKeyType                     kvpKey          = default ( SequenceKeyType );
            SequenceValueType                   kvpValue        = default ( SequenceValueType );
            IValueItem                          valueItem       = null;

            if ( context                                                == null ) return false;
            if ( (kvpVu = item as IValueUnit)                           == null ) return false;
            if ( kvpVu.Count                                            != 2    ) return false;
            if ( (kvpCtxtLvl = context.Hierarchy.Peek () as IValueUnit) == null ) return false;

            // Deserialize the Key
            if ( (valueItem = kvpVu [ nameof ( kvp.Key ) ]) == null ) return false;

            if ( (curCtxtLvlVi = kvpCtxtLvl [ nameof ( kvp.Key ) ]) == null )
                context.Hierarchy.Push ( null );
            else
                context.Hierarchy.Push ( curCtxtLvlVi.Value as IValueUnit );

            try
            {
                if ( !base.ImplDeserializeItem ( out Object key, valueItem.Value, context ) )
                    return false;

                kvpKey = (SequenceKeyType) key;
            } finally { context.Hierarchy.Pop (); }


            // Deserialize the Value
            if ( (valueItem = kvpVu [ nameof ( kvp.Value ) ]) == null ) return false;

            if ( (curCtxtLvlVi = kvpCtxtLvl [ nameof ( kvp.Value ) ]) == null )
                context.Hierarchy.Push ( null );
            else
                context.Hierarchy.Push ( curCtxtLvlVi.Value as IValueUnit );

            try
            {
                if ( !base.ImplDeserializeItem ( out Object value, valueItem.Value, context ) )
                    return false;

                kvpValue = (SequenceValueType) value;
            } finally { context.Hierarchy.Pop (); }

            kvp     = new KeyValuePair<SequenceKeyType, SequenceValueType> ( kvpKey, kvpValue );
            retVal  = kvp;

            return true;
        } // End of ImplDeserializeItem (...)

        protected override SequenceType ImplPopulateSequence (  SequenceType    sequence,
                                                                IEnumerator     enumerator )
            => base.ImplPopulateSequence (  sequence == null? new SequenceType () : sequence,
                                            enumerator );

        protected override Boolean ImplSerializeEnumerable (
            out ArrayList                                   values,
                IEnumerable                                 objValue, 
                IHierarchicalMetadataSerializationContext   context )
        {
            values = new ArrayList ();

            if ( objValue   == null ) return false;
            if ( context    == null ) return false;

            IValueSequence valueSeq = context.Hierarchy.Peek () as IValueSequence;

            if ( valueSeq == null ) return false;

            IList ctxts = valueSeq.Value as IList;

            if ( ctxts == null ) return false;

            IValueUnit  ctxUnit     = null;
            Type        instType    = null;

            context.Hierarchy.Pop ();

            if ( (ctxUnit = context.Hierarchy.Peek () as IValueUnit) == null ) return false;

            // Add SerMgr key
            if ( (instType = typeof ( SequenceType )).IsGenericType )
            {
                ctxUnit.Add ( "SerializationManager",
                                GetType ().GetGenericTypeDefinition ().AssemblyQualifiedName );

                ArrayList typeParamGuids = new ArrayList ();

                foreach ( Type par in instType.GenericTypeArguments )
                    typeParamGuids.Add ( par.GetTypeGuid ().ToString () );

                ctxUnit.Add ( "SerializationManagerInstanceTypeParamGuids",
                                new ValueSequence ( typeParamGuids ) );
            }
            else
                return false;// ctxUnit.Add ( "SerializationManager", instType.AssemblyQualifiedName );

            context.Hierarchy.Push ( valueSeq );

            foreach ( KeyValuePair<SequenceKeyType, SequenceValueType> kvp in objValue )
            {
                ValueUnit   kvpValueUnit    = new ValueUnit ();
                Object      tmpValue        = null;

                ctxts.Add ( ctxUnit = new ValueUnit () );

                context.Hierarchy.Push ( ctxUnit );

                ImplSerializeItem (
                    out tmpValue,
                        //delegate ( IValueUnit vu ) { ctxUnit.Add ( nameof ( kvp.Key ), vu ); },
                        delegate () {
                            ValueUnit vu = new ValueUnit ();

                            ctxUnit.Add ( nameof ( kvp.Key ), vu );
                            context.Hierarchy.Push ( vu );
                        },
                        delegate () { context.Hierarchy.Pop (); },
                        kvp.Key,
                        context );

                kvpValueUnit.Add ( new ValueItem ( nameof ( kvp.Key ), tmpValue ) );

                ImplSerializeItem (
                    out tmpValue,
                        //delegate ( IValueUnit vu ) { ctxUnit.Add ( nameof ( kvp.Value ), vu ); },
                        delegate () {
                            ValueUnit vu = new ValueUnit ();

                            ctxUnit.Add ( nameof ( kvp.Value ), vu );
                            context.Hierarchy.Push ( vu );
                        },
                        delegate () { context.Hierarchy.Pop (); },
                        kvp.Value,
                        context );

                kvpValueUnit.Add ( new ValueItem ( nameof ( kvp.Value ), tmpValue ) );

                values.Add ( kvpValueUnit );

                context.Hierarchy.Pop ();
            }

            return true;
        } // End of ImplSerializeEnumerable (...)

        protected virtual Boolean ImplSerializeItem (
            out Object                                      serializedItem,
                Action                                      contextPrologDlg,
                Action                                      contextEpilogDlg,
                Object                                      itemValue,
                IHierarchicalMetadataSerializationContext   context )
        {
            IValueUnit ctxUnit = null;
            IValueUnit subUnit = null;

            if ( (ctxUnit = context.Hierarchy.Peek () as IValueUnit) == null)
            {
                serializedItem = null;

                return false;
            }

            if ( ImplIsBuiltinType ( itemValue.GetType () ) ) {
                /* ctxts.Add ( null ); */ serializedItem = itemValue; }
            else if ( ImplIsString ( itemValue.GetType () ) ) {
                /* ctxts.Add ( null ); */ serializedItem = itemValue; }
            //else if ( ImplSerializeEnumerable ( retVal, pd.Name, item as IEnumerable, context ) )
            //    continue;
            else if ( ImplSerializeObject (
                    out subUnit,
                        contextPrologDlg,
                        contextEpilogDlg,
                        itemValue, //as IValueObjectContract,
                        context ) )
                serializedItem = subUnit;
            else
                serializedItem = null;

            return true;
        } // End of ImplSerializeItem (...)
    } // End of Class GenericDictionarySerializationManager<...>
} // End of Namespace Sharpframework.Serialization.ValueDom
