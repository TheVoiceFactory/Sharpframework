using System;
using System.Collections;
using System.Collections.Generic;

using Sharpframework.Core;
using Sharpframework.Core.GuidExtension;


namespace Sharpframework.Serialization.ValueDom
{
    using Sharpframework.Serialization;


    public abstract class ValueSequenceSerializationManager<    SequenceType,
                                                                SequenceItemType,
                                                                AdapterType,
                                                                SerializationContextType,
                                                                MyselfType>
        : ValueDomSerializationManager< SequenceType,
                                        AdapterType,
                                        IValueSequence,
                                        SerializationContextType,
                                        MyselfType>
    where SequenceType              : IEnumerable
                                    //, new ()
    where AdapterType               : class
                                    , ISymbolTableAdapter<IValueUnit>//IValueSequence>
                                    , new ()
    where SerializationContextType  : ISerializationContext
    where MyselfType                : ValueSequenceSerializationManager<
                                                                SequenceType,
                                                                SequenceItemType,
                                                                AdapterType,
                                                                SerializationContextType,
                                                                MyselfType>
                                    , new ()
    {
        //protected virtual Boolean ImplAddSequenceItem ( SequenceType sequence, Object item )
        //{
        //    IList list = sequence as IList;

        //    if ( list == null ) return false;

        //    try { list.Add ( item ); }
        //    catch { return false; }

        //    return true;
        //} // End of ImplAddSequenceItem (...)

        protected virtual SequenceType ImplDeserializeEnumerable (
            IValueSequence                                  serialization,
            IRepositorySerializationContext                 context )
        {
            IEnumerator enumerator  = ImplDeserializeEnumerable (
                                            serialization.Value, context ).GetEnumerator ();
            if ( enumerator         == null ) return default ( SequenceType );
            if ( !enumerator.MoveNext ()    ) return default ( SequenceType );

            return ImplPopulateSequence ( default (SequenceType), enumerator );
        } // End of ImplDeserializeEnumerable (...)

        protected virtual IEnumerable ImplDeserializeEnumerable (
            IEnumerable                                 value,
            IHierarchicalMetadataSerializationContext   context )
        {
            IValueProvider<IEnumerable> valueSequence = context.Hierarchy.Peek ()
                                                                    as IValueSequence;

            if ( valueSequence          == null ) yield break;
            if ( valueSequence.Value    == null ) yield break;

            IEnumerator ctxtEnumerator = valueSequence.Value.GetEnumerator ();

            foreach ( Object item in value )
                if ( !ctxtEnumerator.MoveNext () )
                    yield break;
                else
                {
                    context.Hierarchy.Push ( ctxtEnumerator.Current as IValueItemBase );

                    try
                    {
                        if ( ImplDeserializeItem ( out Object retVal, item, context ) )
                            yield return retVal;
                        else
                            yield break;
                    } finally { context.Hierarchy.Pop (); }
                }
        } // End of ImplDeserializeEnumerable (...)

        //protected virtual Boolean ImplDeserializeItem (
        //    out Object                                      retVal,
        //        Object                                      item,
        //        IHierarchicalMetadataSerializationContext   context )
        //{
        //    retVal = null;

        //    ISerializable<IValueUnit>                       serMgr          = null;
        //    ValueDomObjectSerializationManagerCollection    serMgrColl      = null;
        //    ObjectSerializationManagerDescr                 serMgrDescr     = null;
        //    IValueUnit                                      subCtxtLvl      = null;
        //    IValueUnit                                      subUnit         = null;
        //    String                                          typeName        = null;
        //    IValueItem                                      valueItem       = null;

        //    if ( item == null )
        //        retVal = null;
        //    else if ( ImplIsBuiltinType ( item.GetType () ) )
        //        retVal = item;
        //    else if ( ImplIsString ( item.GetType () ) )
        //        retVal = item;
        //    else if ( (subUnit = item as IValueUnit) == null )
        //        return false;
        //        //propValue = pd.GetValue ( InitDto );

        //        //if ( _DeserializeEnumerable ( ref propValue, pd, valueItem.Value, context ) )
        //        //    continue;
        //        //else
        //        //    propValue = valueItem.Value;
        //    else if ( (subCtxtLvl = context.Hierarchy.Peek () as IValueUnit) == null )
        //        return false;
        //    else if ( (valueItem = subCtxtLvl [ "SerializationManager" ] as IValueItem) == null )
        //        return false;
        //    else if ( valueItem.Value == null )
        //        return false;
        //    else if ( (typeName = valueItem.Value.ToString ()) == null )
        //        return false;
        //    else if ( (serMgrColl = ValueDomObjectSerializationManagerCollection.Singleton) == null )
        //        return false;
        //    else if ( (serMgrDescr = serMgrColl [ typeName ]) == null )
        //        return false;
        //    else if ( (serMgr = serMgrDescr.SerializationManager) == null )
        //        return false;
        //    else
        //        retVal = serMgr.Deserialize ( subUnit, context );

        //    return true;
        //} // End  of ImplDeserializeItem (...)

        protected virtual SequenceType ImplPopulateSequence (   SequenceType    sequence,
                                                                IEnumerator     enumerator )
        {
            if ( enumerator == null ) return sequence;
            if ( sequence   == null ) return sequence;

            ICollection<SequenceItemType> sequenceColl = sequence as ICollection<SequenceItemType>;

            if ( sequenceColl == null ) return sequence;

            do
                try     { sequenceColl.Add ( (SequenceItemType) enumerator.Current ); }
                catch   { return sequence; }
            while ( enumerator.MoveNext () );

            return sequence;
        } // End of ImplPopulateSequence (...)

        protected virtual void ImplSerializeEnumerable (
            out IValueSequence                              retVal,
                SequenceType                                instance,
                IHierarchicalMetadataSerializationContext   context )
        {
            ArrayList sequenceValues;

            if ( ImplSerializeEnumerable ( out sequenceValues, instance, context ) )
                retVal = new ValueSequence ( sequenceValues );
            else
                retVal = null;
        } // End of ImplSerializeEnumerable (...)

        protected virtual Boolean ImplSerializeEnumerable (
            out ArrayList                                   values,
                IEnumerable                                 objValue,
                IHierarchicalMetadataSerializationContext   context )
        {
            values = new ArrayList ();

            if ( objValue   == null ) return false;
            if ( context    == null ) return false;

            IValueSequence valueSeq = context.Hierarchy.Peek () as IValueSequence;

            if ( valueSeq == null ) return false;

            IList ctxts = ((IValueItemBase) valueSeq).Value as IList;

            if ( ctxts == null ) return false;

            IValueUnit  ctxUnit     = null;
            Type        itemType    = null;
            Type        serMgrType  = null;
            IValueUnit  subUnit     = null;

            context.Hierarchy.Pop ();

            try
            {
                if ( (ctxUnit = context.Hierarchy.Peek () as IValueUnit) == null )
                    return false;
                else if ( (serMgrType = GetType ()).IsGenericType )
                {
                    ctxUnit.Add (   "SerializationManager",
                                    serMgrType.GetGenericTypeDefinition ().AssemblyQualifiedName );

                    ArrayList typeParamGuids = new ArrayList ();

                    foreach ( Type par in serMgrType.GenericTypeArguments )
                        typeParamGuids.Add ( par.GetTypeGuid ().ToString () );

                    ctxUnit.Add (   "SerializationManagerInstanceTypeParamGuids",
                                    new ValueSequence ( typeParamGuids ) );
                }
                else
                    ctxUnit.Add ( "SerializationManager", serMgrType.AssemblyQualifiedName );
            }
            finally { context.Hierarchy.Push ( valueSeq ); }

            foreach ( Object item in objValue )
                if ( ImplIsBuiltinType ( itemType = item.GetType () ) )
                {
                    ValueUnit vu = new ValueUnit ();

                    vu.Add ( new ValueItem ( "TypeGuid", itemType.GetTypeGuid ().ToString () ) );
                    ctxts.Add ( vu );
                    values.Add ( item );
                }
                else if ( ImplIsString ( item.GetType () ) ) {
                    ctxts.Add ( null ); values.Add ( item ); }
                //else if ( ImplSerializeEnumerable ( retVal, pd.Name, item as IEnumerable, context ) )
                //    continue;
                else if ( ImplSerializeObject (
                        out subUnit,
                            //delegate ( IValueUnit vu ) { ctxts.Add ( vu ); },
                            delegate () {
                                ValueUnit vu = new ValueUnit ();

                                ctxts.Add ( vu );
                                context.Hierarchy.Push ( vu );
                            },
                            delegate () { context.Hierarchy.Pop (); },
                            item, //as IValueObjectContract,
                            context ) )
                    values.Add ( subUnit );

            return true;
        } // End of ImplSerializeEnumerable (...)
    } // End of Class ValueSequenceSerializationManager<...>
} // End of Namespace Sharpframework.Serialization.ValueDom
