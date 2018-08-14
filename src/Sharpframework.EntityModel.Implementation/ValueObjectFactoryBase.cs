using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

using Newtonsoft.Json;

using Sharpframework.EntityModel;
using Sharpframework.Core;
using Sharpframework.Core.GuidExtension;

using Sharpframework.Serialization;
using Sharpframework.Serialization.ValueDom;


namespace Sharpframework.EntityModel.Implementation
{
  

   

    // Alias
    using ObjectSerializationManagerDescr =
                   ValueDomObjectSerializationManagerCollection.SerializationManagerDescr;

    using SequenceSerializationManagerDescr =
                    ValueDomSequenceSerializationManagerCollection.SerializationManagerDescr;


    public abstract class ValueObjectFactoryBase<   InterfaceType,
                                                    ContractType,
                                                    ObjType,
                                                    InitDtoType,
                                                    FactoryType>
        : SingletonObject<FactoryType>
        , IFactory<InterfaceType>
        , ISerializable<IValueUnit, InterfaceType>
    where InterfaceType : IValueObject
    where InitDtoType   : new ()
    where FactoryType   : ValueObjectFactoryBase<   InterfaceType,
                                                    ContractType,
                                                    ObjType,
                                                    InitDtoType,
                                                    FactoryType>
                        , new ()
    {
        private static readonly JsonSerializerSettings  __DefaultJsonDeserializeSettings;
        private static readonly JsonSerializerSettings  __DefaultJsonSerializeSettings;

        private InitDtoType             __initDto;
        private JsonSerializerSettings  __jsonDeserializeSettings;
        private JsonSerializerSettings  __jsonSerializeSettings;


        static ValueObjectFactoryBase ()
        {
            __DefaultJsonDeserializeSettings    = new JsonSerializerSettings ();
            __DefaultJsonSerializeSettings      = new JsonSerializerSettings ();

            __DefaultJsonDeserializeSettings.TypeNameHandling   = TypeNameHandling.Auto;
            __DefaultJsonSerializeSettings.ContractResolver     = new InterfaceContractResolver ();
            __DefaultJsonSerializeSettings.TypeNameHandling     = TypeNameHandling.Auto;
        } // End of Static Constructor

        public ValueObjectFactoryBase ()
            : this ( __DefaultJsonSerializeSettings, __DefaultJsonDeserializeSettings ) { }

        public ValueObjectFactoryBase (
            JsonSerializerSettings serializeSettings,
            JsonSerializerSettings deserializeSettings )
        {
            _Reset ();

            __jsonDeserializeSettings   = deserializeSettings;
            __jsonSerializeSettings     = serializeSettings;
        } // End of Custom Constructor
   

        public InitDtoType InitDto
        {
            get
            {
                if ( __initDto == null )
                {
                    __initDto = new InitDtoType ();
                    ImplClearInitDto ();
                }

                return __initDto;
            }

            set { __initDto = value; }
        }

        public void ClearInitDto () { ImplClearInitDto (); }

        public InterfaceType Deserialize<SerializationContextType> ( IValueUnit valueUnit, SerializationContextType context )
            where SerializationContextType : ISerializationContext
        {
            return valueUnit == null
                        ? default ( InterfaceType )
                        : ImplDispatchDeserialize ( valueUnit, context );
        } // End of Deserialize (...)

        public IJsonContractDocument ExportToJson ( InterfaceType instance )
        { return ExportToJson ( instance, "0" ); }

        public IJsonContractDocument ExportToJson ( InterfaceType instance, String version )
        { return ImplExportToJson ( instance, version ); }

        public Type GetBuildedType ()
        { return ImplGetBuildedType (); }

        public InterfaceType GetByDto ()
        {
            try { return ImplGetNew ( InitDto ); }
            finally { ImplClearInitDto (); }
        } // End of CreateByDto ()

        public InterfaceType GetFromJsonString ( IJsonContractDocument json )
        { return ImplGetFromJsonString ( json ); }

        public InterfaceType GetNew ()
        { ClearInitDto (); return ImplGetNew ( InitDto ); }

        public IValueUnit Serialize<SerializationContextType> ( InterfaceType instance, SerializationContextType context )
            where SerializationContextType : ISerializationContext
        {
            return instance == null ? null : ImplDispatchSerialize ( instance, context );
        }


        protected abstract void ImplClearInitDto ();

        protected virtual InterfaceType ImplDeserialize (
            IValueUnit                                  valueUnit,
            IHierarchicalMetadataSerializationContext   context )
        {
            IValueUnit                              curCtxLvl       = null;

            if ( context                            == null ) return default ( InterfaceType );
            if ( context.Hierarchy                  == null ) return default ( InterfaceType );
            if ( (curCtxLvl = context.Hierarchy.Peek () as IValueUnit)
                                                    == null ) return default ( InterfaceType );

            NumericStringValue              numericString   = null;
            PropertyDescriptorCollection    pdc             = null;
            String                          propStrValue    = String.Empty;
            Type                            propType        = null;
            Object                          propValue       = null;
            IValueUnit                      subUnit         = null;
            IValueItem                      valueItem       = null;
            IValueItemBase                  valueItemBase   = null;

            pdc = TypeDescriptor.GetProperties ( typeof ( InterfaceType ) );

            foreach ( PropertyDescriptor pd in pdc )
            {
                if ( (valueItemBase = valueUnit [ pd.Name ] ) == null ) continue;

                if ( (valueItem = valueItemBase as IValueItem) != null )
                    if ( (subUnit = valueItem.Value as IValueUnit) == null )
                        if ( (numericString = valueItem.Value as NumericStringValue) != null )
                            propValue = numericString.ToType ( pd.PropertyType, null );
                        else if ( valueItem.Value is String ) // String is enumerable...
                            propValue = valueItem.Value;
                        else if ( _DispatchDeserializeEnumerable (
                                        InitDto, pd, valueItem.Value, context ) )
                            continue;
                        else
                            propValue = valueItem.Value;
                    else
                    {
                        if ( (valueItem = curCtxLvl [ pd.Name ] as IValueItem) == null )
                            propValue = null;
                        else
                        {
                            context.Hierarchy.Push ( valueItem.Value as IValueUnit );

                            try     { propValue = ImplDeserializeObject ( subUnit, context ); }
                            finally { context.Hierarchy.Pop (); }
                        }
                    }
                else
                    continue;

                propType = pd.PropertyType;

                //propValue = propStrValue; // Default serializer...

                if (    Type.GetTypeCode ( propType ) == TypeCode.DateTime
                        && (propStrValue = propValue as String ) != null )
                    pd.SetValue ( InitDto, DateTime.ParseExact ( propStrValue, "o", null ) );
                else if ( ImplIsBuiltinType ( propType ) )
                    pd.SetValue ( InitDto, propValue );
                else if ( ImplIsString ( propType ) )
                {
                    Type primValType = propType.GetInterface ( "IHasPrimitiveValue`1" );

                    if ( primValType == null )
                        pd.SetValue ( InitDto, propValue );
                    else
                    {
                        PropertyDescriptor  pvPd = TypeDescriptor.GetProperties ( propType
                                                                ).Find ( "PrimitiveValue", true );
                        Object              pvObj = null;

                        if ( (pvObj = pd.GetValue ( InitDto )) == null )
                            pd.SetValue ( InitDto, pvObj = Activator.CreateInstance ( propType ) );

                        pvPd.SetValue ( pvObj, propValue );
                    }
                }
                else //if ( (subUnit = valueItemBase as IValueUnit) != null )
                    pd.SetValue ( InitDto, propValue );
            }

            return GetByDto ();
        } // End of ImplDeserialize (...)

        protected virtual InterfaceType ImplDeserialize ( IValueUnit valueUnit, Object context )
        {
            return default ( InterfaceType );
        }

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

        protected virtual InterfaceType ImplDispatchDeserialize ( IValueUnit valueUnit, dynamic context )
        { return ImplDeserialize ( valueUnit, context ); }

        protected virtual IValueUnit ImplDispatchSerialize ( InterfaceType instance, dynamic context )
        {
            return ImplSerialize ( instance, context );
        } // End of ImplDispatchSerialize (...)

        protected virtual IJsonContractDocument ImplExportToJson (
            InterfaceType   instance,
            String          version )
        {
            return new JsonDocument (   JsonConvert.SerializeObject ( instance, __jsonSerializeSettings ),
                                        version );
        } // End of ImplExportToJson (...)

        protected virtual Type ImplGetBuildedType () { return typeof ( ObjType ); }

        protected virtual InterfaceType ImplGetFromJsonString ( IJsonContractDocument json )
        {
            Object retVal = JsonConvert.DeserializeObject ( json.Json, __jsonDeserializeSettings );

            return (InterfaceType) retVal;
        } // End of ImplGetFromJsonString (...)

        protected abstract InterfaceType ImplGetNew ( InitDtoType initDto );

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

        protected virtual IValueUnit ImplSerialize ( InterfaceType instance, IHierarchicalMetadataSerializationContext context )
        {
            if ( instance == null ) return null;

            IValueUnit                                  curCtxLvl   = context.Hierarchy.Peek () as IValueUnit;
            IEnumerable                                 enumerable  = null;
            //ArrayList                                   enumValues  = null;
            PropertyDescriptorCollection                pdc         = null;
            Type                                        propType    = null;
            Object                                      propValue   = null;
            ValueUnit                                   retVal      = new ValueUnit ();
            IValueUnit                                  subUnit     = null;

            pdc = TypeDescriptor.GetProperties ( typeof ( ContractType ) );

            foreach ( PropertyDescriptor pd in pdc )
            {
                if ( (propValue = pd.GetValue ( instance )) == null ) continue;
                if ( (propType  = propValue.GetType ())     == null ) continue;

                //propStrValue = propValue.ToString (); // Default serializer...

                //valueItem = new ValueItem ( pd.Name, propValue );

                //if ( String.IsNullOrWhiteSpace ( propStrValue ) ) continue;
                if ( ImplIsBuiltinType ( propType ) )
                    retVal.Add ( new ValueItem ( pd.Name, propValue ) );
                else if ( ImplIsString ( propType ) )
                    retVal.Add ( new ValueItem ( pd.Name, propValue ) );
                else if ( curCtxLvl == null )
                    return null;
                else if ( (enumerable = propValue as IEnumerable) != null )
                {
                    ISerializable<IValueSequence> ser = null;

                    Boolean test = propValue.GetSerializationManager ( ref ser, context);

                    ArrayList       ctxts   = new ArrayList ();
                    ValueSequence   ctxtSeq = new ValueSequence ( ctxts );
                    IValueUnit      ctxtVu  = new ValueUnit ();

                    ctxtVu.Add ( "Items", ctxtSeq );
                    curCtxLvl.Add ( pd.Name, ctxtVu );
                    context.Hierarchy.Push ( ctxtVu );
                    context.Hierarchy.Push ( ctxtSeq );

                    try
                    {
                        retVal.Add ( new ValueItem ( pd.Name, ser.Serialize ( propValue, context ) ) );
                    } finally { context.Hierarchy.Pop (); context.Hierarchy.Pop (); }
                }
                else if ( ImplSerializeObject (
                        out subUnit,
                            delegate ( IValueUnit vu ) { curCtxLvl.Add ( pd.Name, vu ); },
                            propValue as IValueObjectContract,
                            context ) )
                    retVal.Add ( new ValueItem ( pd.Name, subUnit ) );
            } // End of foreach (...)

            return retVal;
        } // End of ImplSerialize (...)

        protected virtual IValueUnit ImplSerialize ( InterfaceType instance, Object context )
        {
            return null;
        } // End of ImplSerialize (...)

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

            IValueUnit  subUnit = null;

            try
            {
                foreach ( Object item in objValue )
                    if ( ImplIsBuiltinType ( item.GetType () ) ) {
                        ctxts.Add ( null ); values.Add ( item ); }
                    else if ( ImplIsString ( item.GetType () ) ) {
                        ctxts.Add ( null ); values.Add ( item ); }
                    //else if ( ImplSerializeEnumerable ( retVal, pd.Name, item as IEnumerable, context ) )
                    //    continue;
                    else if ( ImplSerializeObject (
                            out subUnit,
                                delegate ( IValueUnit vu ) { ctxts.Add ( vu ); },
                                item as IValueObjectContract,
                                context ) )
                        values.Add ( subUnit );
            }
            finally { context.Hierarchy.Pop (); }

            return true;
        } // End of ImplSerializeEnumerable (...)

        protected virtual Boolean ImplSerializeEnumerable (
            out ArrayList                                   values,
                ValueUnit                                   parentUnit,
                String                                      objKey,
                IEnumerable                                 objValue,
                IHierarchicalMetadataSerializationContext   context )
        {
            values = new ArrayList ();

            if ( parentUnit                     == null ) return false;
            if ( String.IsNullOrWhiteSpace ( objKey )   ) return false;
            if ( objValue                       == null ) return false;
            if ( context                        == null ) return false;

            ArrayList   ctxts   = new ArrayList ();
            ValueUnit   ctxtVu  = new ValueUnit ();
            IValueUnit  subUnit = null;

            IValueUnit curCtxLvl   = context.Hierarchy.Peek () as IValueUnit;

            if ( curCtxLvl == null ) return false;

            ctxtVu.Add ( String.Empty, ctxts );
            curCtxLvl.Add ( objKey, ctxtVu );
            context.Hierarchy.Push ( ctxtVu );

            try
            {
                foreach ( Object item in objValue )
                    if ( ImplIsBuiltinType ( item.GetType () ) ) {
                        ctxts.Add ( null ); values.Add ( item ); }
                    else if ( ImplIsString ( item.GetType () ) ) {
                        ctxts.Add ( null ); values.Add ( item ); }
                    //else if ( ImplSerializeEnumerable ( retVal, pd.Name, item as IEnumerable, context ) )
                    //    continue;
                    else if ( ImplSerializeObject (
                            out subUnit,
                                delegate ( IValueUnit vu ) { ctxts.Add ( vu ); },
                                item as IValueObjectContract,
                                context ) )
                        values.Add ( subUnit );
            }
            finally { context.Hierarchy.Pop (); }

            return false;
        } // End of ImplSerializeEnumerable (...)

        protected virtual Boolean ImplSerializeObject (
            out IValueUnit                                  retVal,
                Action<IValueUnit>                          addCtxtDlg,
                IValueObjectContract                        objValue,
                IHierarchicalMetadataSerializationContext   context )
        {
            retVal = null;

            if ( addCtxtDlg == null ) return false;
            if ( objValue   == null ) return false;
            if ( context    == null ) return false;

            ISerializable<IValueUnit> serMgr      = null;
            IValueUnit                valueUnit   = null;

            //ISerializationManagerProvider<IValueUnit>   serMgrPrvdr = null;
            //serMgrPrvdr = objValue as ISerializationManagerProvider<IValueUnit>;

            //if ( serMgrPrvdr                                                == null ) return false;
            //if ( (serMgr = serMgrPrvdr.GetSerializationManager ( context )) == null ) return false;

            if ( !objValue.GetSerializationManager ( ref serMgr, context ) )
                return false;

            addCtxtDlg ( valueUnit = new ValueUnit () );
            context.Hierarchy.Push ( valueUnit );

            try     { retVal = serMgr.Serialize ( objValue, context ); }
            finally { context.Hierarchy.Pop (); }

            return true;
        } // End of ImplSerializeObject (...)


        private Boolean _DeserializeEnumerable (
            Object                                      initObj,
            PropertyDescriptor                          pd,
            Object                                      sequence,
            IHierarchicalMetadataSerializationContext   context )
        => false;
        
        private Boolean _DeserializeEnumerable (
            Object                                      initObj,
            PropertyDescriptor                          pd,
            IValueProvider<IEnumerable>                 sequence,
            IHierarchicalMetadataSerializationContext   context )
        {
            return sequence == null
                        ? false
                        : _DeserializeEnumerable ( initObj, pd, sequence.Value, context );
        } // End of _DeserializeEnumerable (...)

        private Boolean _DeserializeEnumerable (
            Object                                      initObj,
            PropertyDescriptor                          pd,
            IValueSequence                              sequence,
            IHierarchicalMetadataSerializationContext   context )
        {
            if ( sequence                                               == null ) return false;
            if ( pd                                                     == null ) return false;
            if ( !typeof ( IEnumerable ).IsAssignableFrom ( pd.PropertyType )   ) return false;
            if ( sequence                                               == null ) return false;

            IValueUnit  curCtxLvl   = context.Hierarchy.Peek () as IValueUnit;
            Object      propValue   = pd.GetValue ( initObj );

            if ( curCtxLvl  == null ) return false;

            IValueUnit                                      ctxtVu          = null;
            ISerializable<IValueSequence>                   serMgr          = null;
            ValueDomSequenceSerializationManagerCollection  serMgrColl      = null;
            SequenceSerializationManagerDescr               serMgrDescr     = null;
            Type                                            serType         = null;
            String                                          serTypeName     = null;
            IValueSequence                                  subCtxt         = null;
            IValueItem                                      valueItem       = null;
            GuidTypeMapping                                 guidTypeMapping = null;

            if ( (guidTypeMapping = GuidTypeMapping.Singleton)              == null ) return false;
            if ( (serMgrColl = ValueDomSequenceSerializationManagerCollection.Singleton)
                                                                            == null ) return false;
            if ( (valueItem = curCtxLvl [ pd.Name ] as IValueItem)          == null ) return false;
            if ( (ctxtVu = valueItem.Value as IValueUnit)                   == null ) return false;
            if ( (valueItem = ctxtVu [ "SerializationManager" ] as IValueItem)
                                                                            == null ) return false;
            if ( valueItem.Value                                            == null ) return false;
            if ( (serTypeName = valueItem.Value.ToString ())                == null ) return false;
            if ( (serType = Type.GetType ( serTypeName ))                   == null ) return false;
            if ( (valueItem = ctxtVu [ "Items" ] as IValueItem)             == null ) return false;
            if ( (subCtxt = valueItem.Value as IValueSequence)              == null ) return false;

            context.Hierarchy.Push ( ctxtVu );

            try
            {
                if ( serType.IsGenericTypeDefinition )
                    if ( (valueItem = ctxtVu [ "SerializationManagerInstanceTypeParamGuids" ]
                                                as IValueItem) != null )
                    { // Look for arguments
                        GuidTypeMapping         localGuidMapping    = null;
                        Type                    pdPropType          = null;
                        Boolean                 resolved            = true;
                        IValueSequence          typePrmsGuids       = null;

                        if ( (typePrmsGuids = valueItem.Value as IValueSequence) == null )
                            return false;

                        localGuidMapping = new GuidTypeMapping ();

                        foreach ( Object typePrm in typePrmsGuids.Value )
                            if ( Guid.TryParse ( typePrm.ToString (), out Guid typePrmGuid ) )
                            {
                                if ( !guidTypeMapping.TryGetValue (
                                                    typePrmGuid, out Type guidMappedType ) )
                                    if ( (pdPropType = pd.PropertyType).IsArray )
                                        if ( (pdPropType = pdPropType.GetElementType ()) != null )
                                            if ( pdPropType.GetTypeGuid () == typePrmGuid )
                                                guidTypeMapping.Add (
                                                    typePrmGuid, guidMappedType = pdPropType );

                                localGuidMapping.Add ( typePrmGuid, guidMappedType );
                                resolved &= guidMappedType != null;
                            }

                        if ( !resolved )
                            if ( !_ResolveEnumerableTypeParams (
                                        localGuidMapping, subCtxt, context ) ) return false;

                        serType     = serType.MakeGenericType (
                                            localGuidMapping.Types.ToArray () );
                        serTypeName = serType.AssemblyQualifiedName;

                        if ( (serMgrDescr = serMgrColl [ serTypeName ])    == null ) return false;
                        if ( (serMgr = serMgrDescr.SerializationManager)   == null ) return false;
                    }
                    else
                        return false;
                else
                    return false; // TODO: Liste non generics...

                context.Hierarchy.Push ( subCtxt );

                try
                {
                    Object deserializeResult = serMgr.Deserialize ( sequence, context );

                    pd.SetValue ( initObj, deserializeResult );
                } finally { context.Hierarchy.Pop (); }
            } finally { context.Hierarchy.Pop (); }

            return true;
        } // End of _DeserializeEnumerable (...)

        private Type _GetAncestorTypeFromGuid ( Object obj, Guid guid)
        {
            if ( obj    == null ) return null;
            if ( guid   == null ) return null;

            Type curType = obj.GetType ();

            while ( curType != null )
                if ( curType.GetTypeGuid () == guid )
                    return curType;
                else
                    curType = curType.BaseType;

            foreach ( Type interfaceType in obj.GetType ().GetInterfaces () )
                if ( interfaceType.GetTypeGuid () == guid )
                    return interfaceType;

            return null;
        } // End of _GetAncestorTypeFromGuid (...)

        private Boolean _DeserializeEnumerable (
            Object                                      initObj,
            PropertyDescriptor                          pd,
            IEnumerable                                 sequence,
            IHierarchicalMetadataSerializationContext   context )
        {
            if ( pd                                                     == null ) return false;
            if ( !typeof ( IEnumerable ).IsAssignableFrom ( pd.PropertyType )   ) return false;
            if ( sequence                                               == null ) return false;

            IValueUnit  curCtxLvl   = context.Hierarchy.Peek () as IValueUnit;
            Object      propValue   = pd.GetValue ( initObj );

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

                    pd.SetValue ( initObj, retColl );//retVal = retColl;
                }

                do
                    retColl.Add ( enumerator.Current );
                while ( enumerator.MoveNext () );
            }
            finally { context.Hierarchy.Pop (); }

            return true;
        } // End of _DeserializeEnumerable (...)

        private Boolean _DispatchDeserializeEnumerable (
            Object                                      initObj,
            PropertyDescriptor                          pd,
            dynamic                                     sequence,
            IHierarchicalMetadataSerializationContext   context )
        => _DeserializeEnumerable ( initObj, pd, sequence, context );

        private void _Reset ()
        {
            __initDto                   = default ( InitDtoType );
            __jsonDeserializeSettings   = null;
            __jsonSerializeSettings     = null;
        } // End of _Reset ()

        private Boolean _ResolveEnumerableTypeParams (
            GuidTypeMapping                             typeParamsMapping,
            IValueSequence                              itemsCtxt,
            IHierarchicalMetadataSerializationContext   context )
        {
            Object          dummyItem       = null;
            GuidTypeMapping guidTypeMapping = null;
            IEnumerator     subCtxtEnum     = null;
            IValueUnit      subCtxtVu       = null;
            Type            typeParamType   = null;

            if ( (guidTypeMapping = GuidTypeMapping.Singleton)      == null ) return false;
            if ( itemsCtxt.Value                                    == null ) return false;
            if ( (subCtxtEnum = itemsCtxt.Value.GetEnumerator ())   == null ) return false;
            if ( !subCtxtEnum.MoveNext ()                                   ) return false;
            if ( (subCtxtVu = subCtxtEnum.Current as IValueUnit)    == null ) return false;

            context.Hierarchy.Push ( subCtxtVu );

            try     { dummyItem = ImplDeserializeObject ( new ValueUnit (), context ); }
            finally { context.Hierarchy.Pop (); }

            foreach ( GuidTypeMapping.Item gtm in typeParamsMapping )
                if ( gtm.Type != null )
                    continue;
                else if ( gtm.Guid != null )
                    if ( (typeParamType = _GetAncestorTypeFromGuid (
                                                    dummyItem, gtm.Guid )) == null )
                        return false;
                    else
                    {
                        gtm.Type = typeParamType;

                        if ( !guidTypeMapping.ContainsKey ( gtm.Guid ) )
                            guidTypeMapping.Add ( gtm.Guid, typeParamType );
                    }

            return true;
        } // End of _ResolveEnumerableTypeParams (...)


        Object ISerializable<IValueUnit>.Deserialize<SerializationContextType> ( IValueUnit serialization, SerializationContextType context )
        {
            throw new NotImplementedException ();
        }

        IValueUnit ISerializable<IValueUnit>.Serialize<SerializationContextType> ( Object instance, SerializationContextType context )
        {
            throw new NotImplementedException ();
        }
    } // End of Class ValueObjectFactoryBase<...>
} // End of Namespace Sharpframework.Infrastructure.Data

//namespace Sharpframework.Infrastructure.Data
//{
//    using System;
//    using System.Collections;
//    using System.ComponentModel;

//    using Serialization;
//    using ValueDom;
//    using Domains.Framework.New;


//    public abstract class ValueObjectFactoryBase<InterfaceType, InitDtoType, FactoryType>
//        : SingletonObject<FactoryType>
//        , IFactory<InterfaceType>
//        , ISerializable<IValueUnit, InterfaceType>
//    where InterfaceType : IValueObject
//    where InitDtoType   : new ()
//    where FactoryType   : ValueObjectFactoryBase<InterfaceType, InitDtoType, FactoryType>
//                        , new ()
//    {
//        public InterfaceType Deserialize<SerializationContextType> (
//            IValueUnit                  serialization,
//            SerializationContextType    context )
//        where SerializationContextType : ISerializationContext
//            => ImplDeserialize ( serialization, context );

//        public InterfaceType GetByDto () => ImplGetByDto ();

//        public IValueUnit Serialize<SerializationContextType> ( InterfaceType instance, SerializationContextType context )
//            where SerializationContextType : ISerializationContext
//        {
//            return instance == null ? null : ImplDispatchSerialize ( instance, context );
//        }

//        public IValueUnit Serialize<SerializationContextType> ( object instance, SerializationContextType context ) where SerializationContextType : ISerializationContext
//        {
//            throw new System.NotImplementedException ();
//        }

//        object ISerializable<IValueUnit>.Deserialize<SerializationContextType> ( IValueUnit serialization, SerializationContextType context )
//        {
//            throw new System.NotImplementedException ();
//        }



//        protected virtual InterfaceType ImplGetByDto () => default ( InterfaceType );

//        protected abstract InterfaceType ImplDeserialize<SerializationContextType> (
//            IValueUnit                  serialization,
//            SerializationContextType    context )
//        where SerializationContextType : ISerializationContext;

//        protected virtual IValueUnit ImplDispatchSerialize ( InterfaceType instance, dynamic context )
//        {
//            return ImplSerialize ( instance, context );
//        } // End of ImplDispatchSerialize (...)

//        protected Boolean ImplIsBuiltinType ( Type type )
//        {
//            //if ( type.IsPrimitive ) return true;
//            switch ( Type.GetTypeCode ( type ))
//            {
//                case TypeCode.DBNull: case TypeCode.Empty: case TypeCode.String:
//                    return false;

//                case TypeCode.Object:
//                    break;

//                default:
//                    return true;
//            }

//            Type primValType = type.GetInterface ( "IHasPrimitiveValue`1" );

//            if ( primValType == null                                    ) return false;
//            if ( !primValType.GetGenericArguments () [ 0 ].IsPrimitive  ) return false;

//            return true;
//        } // End of ImplIsBuiltinType (...)

//        protected Boolean ImplIsString ( Type type )
//        {
//            if ( type == typeof ( String ) ) return true;

//            Type primValType = type.GetInterface ( "IHasPrimitiveValue`1" );

//            if ( primValType == null                                            ) return false;
//            if ( primValType.GetGenericArguments () [ 0 ] != typeof ( String )  ) return false;

//            return true;
//        } // End of ImplIsString (...)

//        protected virtual IValueUnit ImplSerialize ( InterfaceType instance, IHierarchicalMetadataSerializationContext context )
//        {
//            if ( instance == null ) return null;

//            IValueUnit                                  curCtxLvl   = context.Hierarchy.Peek () as IValueUnit;
//            IEnumerable                                 enumerable  = null;
//            //ArrayList                                   enumValues  = null;
//            PropertyDescriptorCollection                pdc         = null;
//            Type                                        propType    = null;
//            Object                                      propValue   = null;
//            ValueUnit                                   retVal      = new ValueUnit ();
//            IValueUnit                                  subUnit     = null;

//            pdc = TypeDescriptor.GetProperties ( instance.GetSerializationContract ( context ) );

//            foreach ( PropertyDescriptor pd in pdc )
//            {
//                if ( (propValue = pd.GetValue ( instance )) == null ) continue;
//                if ( (propType  = propValue.GetType ())     == null ) continue;

//                //propStrValue = propValue.ToString (); // Default serializer...

//                //valueItem = new ValueItem ( pd.Name, propValue );

//                //if ( String.IsNullOrWhiteSpace ( propStrValue ) ) continue;
//                if ( ImplIsBuiltinType ( propType ) )
//                    retVal.Add ( new ValueItem ( pd.Name, propValue ) );
//                else if ( ImplIsString ( propType ) )
//                    retVal.Add ( new ValueItem ( pd.Name, propValue ) );
//                else if ( curCtxLvl == null )
//                    return null;
//                else if ( (enumerable = propValue as IEnumerable) != null )
//                {
//                    ISerializable<IValueSequence> ser = null;

//                    Boolean test = propValue.GetSerializationManager ( ref ser, context);

//                    ArrayList       ctxts   = new ArrayList ();
//                    ValueSequence   ctxtSeq = new ValueSequence ( ctxts );
//                    IValueUnit      ctxtVu  = new ValueUnit ();

//                    ctxtVu.Add ( "Items", ctxtSeq );
//                    curCtxLvl.Add ( pd.Name, ctxtVu );
//                    context.Hierarchy.Push ( ctxtVu );
//                    context.Hierarchy.Push ( ctxtSeq );

//                    try
//                    {
//                        retVal.Add ( new ValueItem ( pd.Name, ser.Serialize ( propValue, context ) ) );
//                    } finally { context.Hierarchy.Pop (); context.Hierarchy.Pop (); }
//                }
//                else if ( ImplSerializeObject (
//                        out subUnit,
//                            delegate ( IValueUnit vu ) { curCtxLvl.Add ( pd.Name, vu ); },
//                            propValue as IValueObjectSerializationContract,
//                            context ) )
//                    retVal.Add ( new ValueItem ( pd.Name, subUnit ) );
//            } // End of foreach (...)

//            return retVal;
//        } // End of ImplSerialize (...)

//        protected virtual IValueUnit ImplSerialize ( InterfaceType instance, Object context )
//        {
//            return null;
//        } // End of ImplSerialize (...)

//        protected virtual Boolean ImplSerializeEnumerable (
//            out ArrayList                                   values,
//                IEnumerable                                 objValue,
//                IHierarchicalMetadataSerializationContext   context )
//        {
//            values = new ArrayList ();

//            if ( objValue   == null ) return false;
//            if ( context    == null ) return false;

//            IValueSequence valueSeq = context.Hierarchy.Peek () as IValueSequence;

//            if ( valueSeq == null ) return false;

//            IList ctxts = ((IValueItemBase) valueSeq).Value as IList;

//            if ( ctxts == null ) return false;

//            IValueUnit  subUnit = null;

//            try
//            {
//                foreach ( Object item in objValue )
//                    if ( ImplIsBuiltinType ( item.GetType () ) ) {
//                        ctxts.Add ( null ); values.Add ( item ); }
//                    else if ( ImplIsString ( item.GetType () ) ) {
//                        ctxts.Add ( null ); values.Add ( item ); }
//                    //else if ( ImplSerializeEnumerable ( retVal, pd.Name, item as IEnumerable, context ) )
//                    //    continue;
//                    else if ( ImplSerializeObject (
//                            out subUnit,
//                                delegate ( IValueUnit vu ) { ctxts.Add ( vu ); },
//                                item as IValueObjectSerializationContract,
//                                context ) )
//                        values.Add ( subUnit );
//            }
//            finally { context.Hierarchy.Pop (); }

//            return true;
//        } // End of ImplSerializeEnumerable (...)

//        protected virtual Boolean ImplSerializeEnumerable (
//            out ArrayList                                   values,
//                ValueUnit                                   parentUnit,
//                String                                      objKey,
//                IEnumerable                                 objValue,
//                IHierarchicalMetadataSerializationContext   context )
//        {
//            values = new ArrayList ();

//            if ( parentUnit                     == null ) return false;
//            if ( String.IsNullOrWhiteSpace ( objKey )   ) return false;
//            if ( objValue                       == null ) return false;
//            if ( context                        == null ) return false;

//            ArrayList   ctxts   = new ArrayList ();
//            ValueUnit   ctxtVu  = new ValueUnit ();
//            IValueUnit  subUnit = null;

//            IValueUnit curCtxLvl   = context.Hierarchy.Peek () as IValueUnit;

//            if ( curCtxLvl == null ) return false;

//            ctxtVu.Add ( String.Empty, ctxts );
//            curCtxLvl.Add ( objKey, ctxtVu );
//            context.Hierarchy.Push ( ctxtVu );

//            try
//            {
//                foreach ( Object item in objValue )
//                    if ( ImplIsBuiltinType ( item.GetType () ) ) {
//                        ctxts.Add ( null ); values.Add ( item ); }
//                    else if ( ImplIsString ( item.GetType () ) ) {
//                        ctxts.Add ( null ); values.Add ( item ); }
//                    //else if ( ImplSerializeEnumerable ( retVal, pd.Name, item as IEnumerable, context ) )
//                    //    continue;
//                    else if ( ImplSerializeObject (
//                            out subUnit,
//                                delegate ( IValueUnit vu ) { ctxts.Add ( vu ); },
//                                item as IValueObjectSerializationContract,
//                                context ) )
//                        values.Add ( subUnit );
//            }
//            finally { context.Hierarchy.Pop (); }

//            return false;
//        } // End of ImplSerializeEnumerable (...)

//        protected virtual Boolean ImplSerializeObject (
//            out IValueUnit                                  retVal,
//                Action<IValueUnit>                          addCtxtDlg,
//                IValueObjectSerializationContract           objValue,
//                IHierarchicalMetadataSerializationContext   context )
//        {
//            retVal = null;

//            if ( addCtxtDlg == null ) return false;
//            if ( objValue   == null ) return false;
//            if ( context    == null ) return false;

//            ISerializable<IValueUnit> serMgr      = null;
//            IValueUnit                valueUnit   = null;

//            //ISerializationManagerProvider<IValueUnit>   serMgrPrvdr = null;
//            //serMgrPrvdr = objValue as ISerializationManagerProvider<IValueUnit>;

//            //if ( serMgrPrvdr                                                == null ) return false;
//            //if ( (serMgr = serMgrPrvdr.GetSerializationManager ( context )) == null ) return false;

//            if ( !objValue.GetSerializationManager ( ref serMgr, context ) )
//                return false;

//            addCtxtDlg ( valueUnit = new ValueUnit () );
//            context.Hierarchy.Push ( valueUnit );

//            try     { retVal = serMgr.Serialize ( objValue, context ); }
//            finally { context.Hierarchy.Pop (); }

//            return true;
//        } // End of ImplSerializeObject (...)

//        Type IFactory<InterfaceType>.GetBuildedType ()
//        {
//            throw new NotImplementedException ();
//        }

//        InterfaceType IFactory<InterfaceType>.GetFromJsonString ( IJsonContractDocument json )
//        {
//            throw new NotImplementedException ();
//        }

//        IJsonContractDocument IFactory<InterfaceType>.ExportToJson ( InterfaceType instance, string Version )
//        {
//            throw new NotImplementedException ();
//        }

//        InterfaceType IObjectAllocator<InterfaceType>.GetNew ()
//        {
//            throw new NotImplementedException ();
//        }
//    } // End of 
//} // End of Namespace Sharpframework.Infrastructure.Data



namespace Sharpframework.EntityModel.Implementation
{
    public abstract class _ValueObjectFactory<InterfaceType,
                                                ContractType,
                                                ObjType,
                                                InitDtoType,
                                                FactoryType>
        : ValueObjectFactoryBase<InterfaceType,
                                    ContractType,
                                    ObjType,
                                    InitDtoType,
                                    FactoryType>
    where InterfaceType : IValueObject
    where ContractType : IValueObjectContract
    where ObjType : ValueObject<ContractType>
                        , InterfaceType
                        , new()
    where InitDtoType : new()
    where FactoryType : ValueObjectFactory<InterfaceType,
                                                ContractType,
                                                ObjType,
                                                InitDtoType,
                                                FactoryType>
                        , new()
    {
        private static readonly JsonSerializerSettings __DefaultJsonDeserializeSettings;
        private static readonly JsonSerializerSettings __DefaultJsonSerializeSettings;

        private JsonSerializerSettings __jsonDeserializeSettings;
        private JsonSerializerSettings __jsonSerializeSettings;


        static _ValueObjectFactory()
        {
            __DefaultJsonDeserializeSettings = new JsonSerializerSettings();
            __DefaultJsonSerializeSettings = new JsonSerializerSettings();

            __DefaultJsonDeserializeSettings.TypeNameHandling = TypeNameHandling.Auto;
            __DefaultJsonSerializeSettings.ContractResolver = new ValueObjectContractResolver();
            __DefaultJsonSerializeSettings.TypeNameHandling = TypeNameHandling.Auto;
        } // End of Static Constructor

        public _ValueObjectFactory()
            : this(__DefaultJsonSerializeSettings, __DefaultJsonDeserializeSettings) { }

        public _ValueObjectFactory(
                JsonSerializerSettings serializeSettings,
                JsonSerializerSettings deserializeSettings)
            : base(serializeSettings, deserializeSettings)
        {
            _Reset();

            __jsonDeserializeSettings = deserializeSettings;
            __jsonSerializeSettings = serializeSettings;
        } // End of Custom Constructor


        private void _Reset()
        {
            __jsonDeserializeSettings = null;
            __jsonSerializeSettings = null;
        } // End of _Reset ()
    } // End of Class ValueObjectFactory<...>


    public abstract class _ValueObjectFactory<InterfaceType,
                                                ContractType,
                                                ObjType,
                                                FactoryType>
        : ValueObjectFactory<InterfaceType,
                                ContractType,
                                ObjType,
                                ObjType,
                                FactoryType>
    where InterfaceType : IValueObject
    where ContractType : IValueObjectContract
    where ObjType : ValueObject<ContractType>
                        , InterfaceType
                        , new()
    where FactoryType : ValueObjectFactory<InterfaceType,
                                                ContractType,
                                                ObjType,
                                                FactoryType>
                        , new()
    {
        protected override sealed void ImplClearInitDto()
        { } // Do nothing

        protected override InterfaceType ImplGetNew(ObjType initDto)
        { try { return initDto; } finally { InitDto = new ObjType(); } }
    } // End of ValueObjectFactoryBase<...>
} // End of Namespace Sharpframework.Domains.Shared.Model

//namespace Sharpframework.Infrastructure.Data
//{
//    using System;
//    using System.Collections;
//    using System.ComponentModel;

//    using Serialization;
//    using ValueDom;
//    using Domains.Framework.New;
//    using Sharpframework.Implementations.Domains.Shared.Model.New;


//    public abstract class ValueObjectFactory<InterfaceType, InitDtoType, FactoryType>
//        : SingletonObject<FactoryType>
//    where InterfaceType : IValueObject
//    where InitDtoType   : new ()
//    where FactoryType   : ValueObjectFactoryBase<InterfaceType, InitDtoType, FactoryType>
//                        , new ()
//    {
//        public abstract class AbsValueObjectInitDto
//        {
//            private ValueObject __core;

//            private ValueObject _Core
//            {
//                get
//                {
//                    if ( __core == null )
//                    {
//                        __core = ImplCore as ValueObject;
//                        // Init Default Values...
//                    }

//                    return __core;
//                }
//            }

//            public void Clear () => ImplClear ();

//            protected virtual void ImplClear () { __core = null; }

//            protected abstract InterfaceType ImplCore { get; }
//        } // End of Class AbsValueObjectInitDto


//        private InitDtoType __initDto;


//        protected virtual InitDtoType ImplInitDto
//        {
//            get
//            {
//                if ( __initDto == null ) __initDto = new InitDtoType ();

//                return __initDto;
//            }
//        }
//    }
//}
