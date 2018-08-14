using System;

using Sharpframework.Core;


namespace Sharpframework.EntityModel.Implementation
{
    using System.Collections;
    using System.ComponentModel;

    using Sharpframework.Serialization;
    using Sharpframework.Serialization.ValueDom;
    //using Sharpframework.EntityModel.Implementation;
    

    // Alias
    using ObjectSerializationManagerDescr =
                    Sharpframework.Serialization.ValueDom.ValueDomObjectSerializationManagerCollection.SerializationManagerDescr;


    public abstract class ValueObject
        : InfrastructureCoreObject
        , IValueObject
        , IValueObjectSerializationContract
        , IValueDomAlignable<IValueUnit, IValueUnit>
    {
        private IHierarchicalMetadataSerializationContext   __context;
        private IValueUnit                                  __valueUnit;


        protected ValueObject () { _Reset (); }

        protected ValueObject ( IValueUnit                                  valueUnit,
                                IHierarchicalMetadataSerializationContext   context )
            : this ()
        {
            __context   = context;
            __valueUnit = valueUnit;

            ImplFillValues ( valueUnit, context );
        } // End of Custom (Deserialization) Constructor


        public IValueUnit Data      => __valueUnit;
        public IValueUnit Metadata  => __context == null ? null : __context.Metadata;

        public Boolean Align ()
        {
            if ( __valueUnit == null ) return false;

            IValueItem valueItem = null;

            foreach ( IValueItemBase item in __valueUnit )
                if ( (valueItem = item as IValueItem) != null )
                    valueItem.AlignValueToBoundValue ();

            return true;
        } // End of Align ()

        public Type GetSerializationContract<SerializationContextType> (
            SerializationContextType context )
        where SerializationContextType : ISerializationContext
            => ImplGetSerializationContract ( context );


        protected virtual IEnumerable ImplDeserializeEnumerable (
            IEnumerable                                 value,
            IHierarchicalMetadataSerializationContext   context )
        {
            ISerializable<IValueUnit>                       serMgr          = null;
            IValueUnit                                      subCtxtLvl      = null;
            IValueUnit                                      subUnit         = null;
            IValueProvider<IEnumerable>                     valueSequence   = null;

            valueSequence = context.Hierarchy.Peek () as IValueSequence;

            if ( valueSequence          == null ) yield break;
            if ( valueSequence.Value    == null ) yield break;

            IEnumerator ctxtEnumerator = valueSequence.Value.GetEnumerator ();

            foreach ( Object item in value )
                if ( !ctxtEnumerator.MoveNext () )
                    yield break;
                else if ( (subUnit = item as IValueUnit) == null )
                    if ( item is String ) // String is enumerable...
                        yield return item;
                    else
                    {
                        //propValue = pd.GetValue ( InitDto );

                        //if ( _DeserializeEnumerable ( ref propValue, pd, valueItem.Value, context ) )
                        //    continue;
                        //else
                        //    propValue = valueItem.Value;
                    }
                else
                {
                    if ( (subCtxtLvl = ctxtEnumerator.Current as IValueUnit) == null )
                        yield return null;
                    else
                    {
                        context.Hierarchy.Push ( subCtxtLvl );

                        try     { yield return serMgr.Deserialize ( subUnit, context ); }
                        finally { context.Hierarchy.Pop (); }
                    }
                }

            yield break;
        } // End of ImplDeserializeEnumerable (...)

        protected virtual Object ImplDeserializeObject (
            IValueUnit                                  objectVu,
            IHierarchicalMetadataSerializationContext   context )
        {
            IValueUnit                                      objCtxtLvl  = null;
            ISerializable<IValueUnit>                       serMgr      = null;
            ValueDomObjectSerializationManagerCollection    serMgrColl  = null;
            ObjectSerializationManagerDescr                 serMgrDescr = null;
            String                                          typeName    = null;
            IValueItem                                      valueItem   = null;

            if ( (objCtxtLvl = context.Hierarchy.Peek () as IValueUnit) == null )
                return null;

            if ( (valueItem = objCtxtLvl [ "SerializationManager" ] as IValueItem)  == null )
                return null;

            if ( valueItem.Value == null )
                return null;

            if ( (typeName = valueItem.Value.ToString ()) == null )
                return null;

            if ( (serMgrColl = ValueDomObjectSerializationManagerCollection.Singleton) == null )
                return null;

            if ( (serMgrDescr = serMgrColl [ typeName ]) == null )
                return null;

            return (serMgr = serMgrDescr.SerializationManager) == null
                        ? null : serMgr.Deserialize ( objectVu, context );
        } // End of ImplDeserializeObject (...)

        protected virtual void ImplFillValues (
            IValueUnit                                  valueUnit,
            IHierarchicalMetadataSerializationContext   context )
        {
            IValueUnit                              curCtxLvl       = null;

            if ( context                                                == null ) return;
            if ( context.Hierarchy                                      == null ) return;
            if ( (curCtxLvl = context.Hierarchy.Peek () as IValueUnit)  == null ) return;

            //NumericStringValue              numericString   = null;
            PropertyDescriptorCollection    pdc             = null;
            String                          propStrValue    = String.Empty;
            Type                            propType        = null;
            Object                          propValue       = null;
            IValueUnit                      subUnit         = null;
            IValueItem                      valueItem       = null;
            IValueItemBase                  valueItemBase   = null;

            //pdc = TypeDescriptor.GetProperties ( ImplGetSerializationContract ( context ) );
            pdc = ImplGetSerializationContract ( context ).GetAllInterfaceProperties ();

            foreach ( PropertyDescriptor pd in pdc )
            {
                if ( (valueItemBase = valueUnit [ pd.Name ]         ) == null ) continue;
                if ( (valueItem     = valueItemBase as IValueItem   ) == null ) continue;

                if ( valueItem.ValueBinder == null )
                    if ( typeof ( IPrimitiveTypeValue ).IsAssignableFrom ( pd.PropertyType ) )
                        valueItem.ValueBinder = new ValueItemPrimitiveTypeValuePdBinder ( pd, this );
                    else
                        valueItem.ValueBinder = new ValueItemObjectValuePdBinder ( pd, this );

                if ( (subUnit = valueItem.Value as IValueUnit) == null )
                {
                    //if ( (numericString = valueItem.Value as NumericStringValue) != null )
                    //    propValue = numericString.ToType ( pd.PropertyType, null );
                    //else if ( valueItem.Value is String ) // String is enumerable...
                    //    propValue = valueItem.Value;
                    //else if ( _DispatchDeserializeEnumerable ( pd, valueItem.Value, context ) )
                    //    continue;
                    //else
                    //    propValue = valueItem.Value;
                    if ( _DispatchDeserializeEnumerable ( pd, valueItem.Value, context ) )
                        continue;
                    else if ( valueItem.AlignBoundValueToValue () )
                        continue;
                }
                else
                {
                    if ( (valueItem = curCtxLvl [ pd.Name ] as IValueItem) == null )
                        propValue = null;
                    else
                    {
                        context.Hierarchy.Push ( valueItem.Value as IValueUnit );

                        try { propValue = ImplDeserializeObject ( subUnit, context ); }
                        finally { context.Hierarchy.Pop (); }
                    }
                }

                propType = pd.PropertyType;

                //propValue = propStrValue; // Default serializer...

                if (    Type.GetTypeCode ( propType ) == TypeCode.DateTime
                        && (propStrValue = propValue as String ) != null )
                    pd.SetValue ( this, DateTime.ParseExact ( propStrValue, "o", null ) );
                else if ( ImplIsBuiltinType ( propType ) )
                    pd.SetValue ( this, propValue );
                else if ( ImplIsString ( propType ) )
                {
                    Type primValType = propType.GetInterface ( "IHasPrimitiveValue`1" );

                    if ( primValType == null )
                        pd.SetValue ( this, propValue );
                    else
                    {
                        PropertyDescriptor  pvPd = TypeDescriptor.GetProperties ( propType
                                                                ).Find ( "PrimitiveValue", true );
                        Object              pvObj = null;

                        if ( (pvObj = pd.GetValue ( this )) == null )
                            pd.SetValue ( this, pvObj = Activator.CreateInstance ( propType ) );

                        pvPd.SetValue ( pvObj, propValue );
                    }
                }
                else //if ( (subUnit = valueItemBase as IValueUnit) != null )
                    pd.SetValue ( this, propValue );
            }
        } // End of ImplFillValues (...)

        protected abstract Type ImplGetSerializationContract<SerializationContextType> (
            SerializationContextType context )
        where SerializationContextType : ISerializationContext;

        protected Boolean ImplIsBuiltinType ( Type type )
        {
            //if ( type.IsPrimitive ) return true;
            switch ( Type.GetTypeCode ( type ))
            {
                case TypeCode.DBNull: case TypeCode.Empty: case TypeCode.String:
                    return false;

                case TypeCode.Object:
                    break;

                default:
                    return true;
            }

            Type primValType = type.GetInterface ( "IHasPrimitiveValue`1" );

            if ( primValType == null                                    ) return false;
            if ( !primValType.GetGenericArguments () [ 0 ].IsPrimitive  ) return false;

            return true;
        } // End of ImplIsBuiltinType (...)

        protected Boolean ImplIsString ( Type type )
        {
            if ( type == typeof ( String ) ) return true;

            Type primValType = type.GetInterface ( "IHasPrimitiveValue`1" );
            
            if ( primValType == null                                            ) return false;
            if ( primValType.GetGenericArguments () [ 0 ] != typeof ( String )  ) return false;

            return true;
        } // End of ImplIsString (...)

        //protected override Type ImplFieldsViewType
        //{ get { return GetType ().GetInterface ( typeof ( SerializationContractType ).Name ); } }


        private Boolean _DeserializeEnumerable (
            PropertyDescriptor                          pd,
            Object                                      sequence,
            IHierarchicalMetadataSerializationContext   context )
        => false;

        private Boolean _DeserializeEnumerable (
            PropertyDescriptor                          pd,
            String                                      sequence,
            IHierarchicalMetadataSerializationContext   context )
        => false;

        private Boolean _DeserializeEnumerable (
            PropertyDescriptor                          pd,
            IValueProvider<IEnumerable>                 sequence,
            IHierarchicalMetadataSerializationContext   context )
        {
            return sequence == null
                        ? false
                        : _DeserializeEnumerable ( pd, sequence.Value, context );
        } // End of _DeserializeEnumerable (...)

        private Boolean _DeserializeEnumerable (
            PropertyDescriptor                          pd,
            IEnumerable                                 sequence,
            IHierarchicalMetadataSerializationContext   context )
        {
            if ( pd                                                     == null ) return false;
            if ( !typeof ( IEnumerable ).IsAssignableFrom ( pd.PropertyType )   ) return false;
            if ( sequence                                               == null ) return false;

            IValueUnit  curCtxLvl   = context.Hierarchy.Peek () as IValueUnit;
            Object      propValue   = pd.GetValue ( this );

            if ( curCtxLvl  == null ) return false;

            IValueItem      valueItem   = curCtxLvl [ pd.Name ] as IValueItem;
            IValueSequence  subCtxt     = null;

            if ( valueItem                                      == null ) return false;
            if ( (subCtxt = valueItem.Value as IValueSequence)  == null ) return false;

            context.Hierarchy.Push ( subCtxt );

            try
            {
                IEnumerator enumerator  = ImplDeserializeEnumerable (
                                                sequence, context ).GetEnumerator ();
                IList       retColl     = null;

                if ( enumerator         == null ) return false;
                if ( !enumerator.MoveNext ()    ) return false;

                if ( (retColl = propValue as IList ) == null )
                {
                    Type collType = pd.PropertyType;

                    if ( collType.IsInterface )
                    {
                        if ( collType.IsGenericType )
                            if ( collType.Name == "IList`1" )
                                collType = typeof ( System.Collections.Generic.List<> ).MakeGenericType ( collType.GenericTypeArguments );
                    }

                    System.Reflection.ConstructorInfo ci = collType.GetConstructor ( Type.EmptyTypes );

                    if ( ci                                                 == null ) return false;
                    if ( (retColl = ci.Invoke ( Type.EmptyTypes ) as IList) == null ) return false;

                    pd.SetValue ( this, retColl );//retVal = retColl;
                }

                do
                    retColl.Add ( enumerator.Current );
                while ( enumerator.MoveNext () );
            }
            finally { context.Hierarchy.Pop (); }

            return true;
        } // End of _DeserializeEnumerable (...)

        private Boolean _DispatchDeserializeEnumerable (
            PropertyDescriptor                          pd,
            dynamic                                     sequence,
            IHierarchicalMetadataSerializationContext   context )
        => _DeserializeEnumerable ( pd, sequence, context );

        private void _Reset ()
        {
            __context   = null;
            __valueUnit = null;
        } // End of _Reset ()


        Boolean IValueDomAlignable<IValueUnit, IValueUnit>.GetContext<ContextType> ( ref ContextType context )
        {
            if ( __context == null                                                  ) return false;
            if ( !typeof ( ContextType ).IsAssignableFrom ( __context.GetType () )  ) return false;

            context = (ContextType) __context;

            return true;
        } // End of IValueDomAlignable<IValueUnit, IValueUnit>.GetContext<> () Explicit Implementation
    } // End of Class ValueObject
} // End of Namespace Sharpframework.Implementations.Domains.Shared.Model
