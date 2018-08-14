using System;

using Sharpframework.Core;


namespace Sharpframework.Serialization
{
    public abstract class SerializationManager< InstanceType,
                                                AdapterType,
                                                SerializedType,
                                                SerializationContextType,
                                                MyselfType>
        : SingletonObject<MyselfType>
        , ISerializable<SerializedType, InstanceType>
    where AdapterType               : class
                                    , ISymbolTableAdapter
                                    , new ()
    where SerializedType            : ISymbolTableItem
    where SerializationContextType  : ISerializationContext
    where MyselfType                : SerializationManager< InstanceType,
                                                            AdapterType,
                                                            SerializedType,
                                                            SerializationContextType,
                                                            MyselfType>
                                    , new ()
    {
        private AdapterType __adapter;


        public SerializationManager () { __adapter = default ( AdapterType ); }


        public InstanceType Deserialize ( SerializedType serialization, SerializationContextType context )
            => ImplDeserialize ( serialization, context );

        public SerializedType Serialize ( InstanceType instance, SerializationContextType context )
            => _Serialize ( instance, context );


        protected virtual AdapterType ImplAdapter
        {
            get
            {
                if ( __adapter == null )
                    if ( typeof ( AdapterType ).IsSubclassOf ( typeof ( SingletonObject<AdapterType> ) ) )
                        __adapter = SingletonObject<AdapterType>.Singleton;
                    else
                        __adapter = new AdapterType ();

                return __adapter;
            }
        }


        //protected virtual Object ImpConvertToType ( IConvertible converter, Type targetType )
        //{
        //    if ( converter  == null ) return null;
        //    if ( targetType == null ) return null;

        //    return converter.ToType ( targetType, null );
        //} // End of ImpConvertToType (...)

        protected virtual Object ImpConvertToType ( IConvertibleString stringConverter, Type targetType )
        {
            if ( stringConverter    == null ) return null;
            if ( targetType         == null ) return null;

            return stringConverter.ToType ( targetType, null );
        } // End of ImpConvertToType (...)

        protected virtual Object ImpConvertToType ( Object sourceValue, Type targetType )
        {
            String              oldCnvStrValue  = null;
            IConvertibleString  stringConverter = null;

            if ( sourceValue                                == null ) return null;
            if ( targetType                                 == null ) return sourceValue;
            if ( ImplAdapter                                == null ) return sourceValue;
            if ( (stringConverter = ImplAdapter.Converter)  == null ) return sourceValue;

            oldCnvStrValue = stringConverter.StringValue;

            try
            {
                stringConverter.StringValue = sourceValue.ToString ();

                return stringConverter.ToType ( targetType, null );
            }
            finally { stringConverter.StringValue = oldCnvStrValue; }
        } // End of ImpConvertToType (...)

        protected abstract InstanceType ImplDeserialize ( SerializedType serialization, SerializationContextType context );

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

        protected abstract void ImplSerialize ( out SerializedType retVal, InstanceType instance, SerializationContextType context );

        protected virtual void ImplSerialize ( out SerializedType retVal, Object instance, Object context )
        {
            if ( !typeof ( InstanceType ).IsInstanceOfType ( instance ) )
                retVal = default ( SerializedType );
            else if ( !typeof ( SerializationContextType ).IsInstanceOfType ( context ) )
                retVal = default ( SerializedType );
            else
                ImplSerialize ( out retVal, (InstanceType)instance, (SerializationContextType) context );
        } // End of ImplSerialize (...)

        protected Boolean ImplSerializeObject<RetvalType, ContextType> (
            out RetvalType retVal,
                Action contextPrologDlg,
                Action contextEpilogDlg,
                Object objValue,
                ContextType context )
        where ContextType : ISerializationContext
            => ImplSerializeObject (
                    out retVal, null, contextPrologDlg, contextEpilogDlg, objValue, context );

        protected virtual Boolean ImplSerializeObject<RetvalType, ContextType> (
            out RetvalType                  retVal,
                ISerializable<RetvalType>   serMgr,
                Action                      contextPrologDlg,
                Action                      contextEpilogDlg,
                Object                      objValue,
                ContextType                 context )
        where ContextType : ISerializationContext
        {
            retVal = default ( RetvalType );

            if ( objValue   == null ) return false;
            if ( serMgr     == null ) return false;

            //ISerializable<RetvalType> serMgr = null;

            //if ( !objValue.GetSerializationManager ( ref serMgr, context ) )
            //    return false;

            contextPrologDlg?.Invoke ();

            try { retVal = serMgr.Serialize ( objValue, context ); }
            finally { contextEpilogDlg?.Invoke (); }

            return true;
        } // End of ImplSerializeObject<...> (...)


        private InstanceType _Deserialize ( SerializedType serialization, SerializationContextType context )
            => ImplDeserialize ( serialization, context );

        private InstanceType _Deserialize ( SerializedType serialization, Object context )
            => default ( InstanceType );

        private SerializedType _Serialize ( InstanceType instance, SerializationContextType context )
        {
            ImplSerialize ( out SerializedType retVal, instance, context );

            return retVal;
        } // End of _Serialize (...)

        private SerializedType _Serialize ( Object instance, Object context )
        {
            ImplSerialize ( out SerializedType retVal, instance, context );

            return retVal;
        } // End of _Serialize (...)

        private SerializedType _DispatchSerialize ( dynamic instance, dynamic context )
            => _Serialize ( instance, context );


        Object ISerializable<SerializedType>.Deserialize<SerContextType> ( SerializedType serialization, SerContextType context )
        { //=> _Deserialize ( serialization, (dynamic)context );
            Object ctxt = context;

            // Questo test manuale è necessario perché si è visto che, in presenza di generici,
            // l'uso di dynamic non solo non distribuisce correttamente la chiamata, ma addirit-
            // tura porta ad un loop all'interno del framework con corrispondente terminazione
            // del programma per Stack Overflow.
            if ( typeof ( SerializationContextType ).IsInstanceOfType ( ctxt ) )
                return _Deserialize ( serialization, (SerializationContextType) ctxt );
            else
                return _Deserialize ( serialization, ctxt );
        } // End of ISerializable<SerializedType>.Deserialize (...) Explicit Implementation

        SerializedType ISerializable<SerializedType>.Serialize<SerContextType> ( Object instance, SerContextType context )
        {
            // Questo test manuale è necessario perché si è visto che, in presenza di generici
            // o array, l'uso di dynamic non solo non distribuisce correttamente la chiamata,
            // ma addirittura porta ad un loop all'interno del framework con corrispondente ter-
            // minazione del programma per Stack Overflow.
            Type t = null;

            if ( instance != null && (t = instance.GetType ())!= null )
                if ( t.IsGenericType )
                    return _Serialize ( instance, context );
                else if ( t.IsArray )
                    return _Serialize ( instance, context );

            return _DispatchSerialize ( instance, context );
        } // End of ISerializable<SerializedType>.Serialize (...) Explicit Implementation

        SerializedType ISerializable<SerializedType, InstanceType>.Serialize<SerContextType> ( InstanceType instance, SerContextType context )
            => _Serialize ( instance, (dynamic) context );

        InstanceType ISerializable<SerializedType, InstanceType>.Deserialize<SerializationContextType1> ( SerializedType serialization, SerializationContextType1 context )
            => _Deserialize ( serialization, context );
    } // End of Class SerializationManager<...>
} // End of Namespace Sharpframework.Serialization
