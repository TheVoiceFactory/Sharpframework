using System;

using Sharpframework.Core;


namespace Sharpframework.Serialization.ValueDom
{
    using Sharpframework.Serialization;

    // Alias
    using ObjectSerializationManagerDescr =
                ValueDomObjectSerializationManagerCollection.SerializationManagerDescr;


    public abstract class ValueDomSerializationManager<
                                            InstanceType,
                                            AdapterType,
                                            SerializedType,
                                            SerializationContextType,
                                            MyselfType>
        : SerializationManager< InstanceType,
                                AdapterType, 
                                SerializedType,
                                SerializationContextType,
                                MyselfType>
    where AdapterType               : class
                                    , ISymbolTableAdapter
                                    , new ()
    where SerializedType            : ISymbolTableItem
    where SerializationContextType  : ISerializationContext
    where MyselfType                : ValueDomSerializationManager<
                                                            InstanceType,
                                                            AdapterType,
                                                            SerializedType,
                                                            SerializationContextType,
                                                            MyselfType>
                                    , new ()
    {
        protected virtual Boolean ImplDeserializeItem (
            out Object                                      retVal,
                Object                                      item,
                IHierarchicalMetadataSerializationContext   context )
            => ImplDeserializeItem (    out retVal,
                                            item,
                                            delegate () { return context.Hierarchy.Peek (); },
                                            context );

        protected virtual Boolean ImplDeserializeItem<ContextType> (
            out Object                  retVal,
                Object                  item,
                Func<IValueItemBase>    getCurrentContextDlg,
                ContextType             context )
        where ContextType : ISerializationContext
        {
            retVal = null;

            //Type                                            itemType        = null;
            //NumericStringValue                              numericString   = null;
            IConvertibleString                              cnvStr          = null;
            ISerializable<IValueUnit>                       serMgr          = null;
            ValueDomObjectSerializationManagerCollection    serMgrColl      = null;
            ObjectSerializationManagerDescr                 serMgrDescr     = null;
            IValueUnit                                      subCtxtLvl      = null;
            IValueUnit                                      subUnit         = null;
            String                                          typeName        = null;
            IValueItem                                      valueItem       = null;

            if ( item == null )
                retVal = null;
            else if ( ImplIsBuiltinType ( item.GetType () ) )
                retVal = item;
            //else if ( ImplIsString ( item.GetType () ) )
            //    if ( (itemType = ImplGetItemType ( context )) == null )
            //        retVal = item;
            //    else if ( itemType == typeof ( String ) )
            //        retVal = item;
            //    else
            //        retVal = ImpConvertToType ( item, itemType );
            //else if ( (numericString = item as NumericStringValue) != null )
            //    retVal = ImpConvertToType ( numericString, ImplGetItemType ( context ) );
            else if ( ImplIsString ( item.GetType () ) )
                retVal = ImpConvertToType ( item, ImplGetItemType ( context ) );
            else if ( (cnvStr = item as IConvertibleString) != null )
                retVal = ImpConvertToType ( cnvStr, ImplGetItemType ( context ) );
            else if ( (subUnit = item as IValueUnit) == null )
                return false;
            //propValue = pd.GetValue ( InitDto );

            //if ( _DeserializeEnumerable ( ref propValue, pd, valueItem.Value, context ) )
            //    continue;
            //else
            //    propValue = valueItem.Value;
            else if ( getCurrentContextDlg == null )
                return false;
            else if ( (subCtxtLvl = getCurrentContextDlg () as IValueUnit) == null )
                return false;
            else if ( (valueItem = subCtxtLvl [ "SerializationManager" ] as IValueItem) == null )
                return false;
            else if ( valueItem.Value == null )
                return false;
            else if ( (typeName = valueItem.Value.ToString ()) == null )
                return false;
            else if ( (serMgrColl = ValueDomObjectSerializationManagerCollection.Singleton) == null )
                return false;
            else if ( (serMgrDescr = serMgrColl [ typeName ]) == null )
                return false;
            else if ( (serMgr = serMgrDescr.SerializationManager) == null )
                return false;
            else
                retVal = serMgr.Deserialize ( subUnit, context );

            return true;
        } // End  of ImplDeserializeItem (...)

        protected virtual Type ImplGetItemType ( IHierarchicalMetadataSerializationContext context )
        {
            if ( context == null ) return null;

            IValueItem itemCtxtVi = null;
            IValueUnit itemCtxtVu = context.Hierarchy.Peek () as IValueUnit;

            if ( itemCtxtVu                                 == null ) return null;
            if ( (itemCtxtVi = itemCtxtVu [ "TypeGuid" ])   == null ) return null;

            GuidTypeMapping.Item tmp = GuidTypeMapping.Singleton [
                                            new Guid ( itemCtxtVi.Value as String ) ];

            return tmp == null ? null : tmp.Type;
        } // End of ImplGetItemType (...)

        protected virtual Type ImplGetItemType ( Object context )
            => ImplGetItemType ( context as IHierarchicalMetadataSerializationContext );

        protected override Boolean ImplSerializeObject<RetvalType, ContextType> (
            out RetvalType                  retVal,
                ISerializable<RetvalType>   serMgr,
                Action                      contextPrologDlg,
                Action                      contextEpilogDlg,
                Object                      objValue,
                ContextType                 context )
        {
            if ( serMgr == null )
                objValue.GetSerializationManager ( ref serMgr, context );

            return base.ImplSerializeObject ( out   retVal,
                                                    serMgr,
                                                    contextPrologDlg,
                                                    contextEpilogDlg,
                                                    objValue,
                                                    context );
        } // End of ImplSerializeObject<...> (...)
    } // End of Class ValueDomSerializationManager<...>
} // End of Namespace Sharpframework.Serialization.ValueDom
