using System;
using System.Collections;
using System.Collections.Generic;


namespace Sharpframework.Serialization.ValueDom
{
    using Sharpframework.Serialization;

    public static class SerializationManagerProviderExtension
    {
        protected static class SerializationManagerProviderExtensionImpl
        {
            public static Boolean
                GetSerializationManager<SerializationContextType, SerializedType> ( Object obj, ref ISerializable<SerializedType> serializer, SerializationContextType context )
                    where SerializationContextType : ISerializationContext
            {
                return false;
            }

            public static Boolean
                GetSerializationManager<SerializationContextType, SerializedType> ( IEnumerable sequence, ref ISerializable<SerializedType> serializer, SerializationContextType context )
                    where SerializationContextType : ISerializationContext
            {
                return false;
            }

            public static Boolean
                GetSerializationManager<SerializationContextType, SerializedType, ListItemType> ( List<ListItemType> list, ref ISerializable<SerializedType> serializer, SerializationContextType context )
                    where SerializationContextType : ISerializationContext
            {
                serializer = JsonGenericListRepositorySerializationManager<ListItemType>.Singleton
                                as ISerializable<SerializedType>;

                return serializer != null;
            }

            public static Boolean
                GetSerializationManager<SerializationContextType, SerializedType, ArrayItemType> ( ArrayItemType [] array, ref ISerializable<SerializedType> serializer, SerializationContextType context )
                    where SerializationContextType : ISerializationContext
            {
                serializer = JsonArrayRepositorySerializationManager<ArrayItemType>.Singleton
                                as ISerializable<SerializedType>;

                return serializer != null;
            }

            public static Boolean
                GetSerializationManager<SerializationContextType, SerializedType, KeyType, ItemType> ( Dictionary<KeyType, ItemType> list, ref ISerializable<SerializedType> serializer, SerializationContextType context )
                    where SerializationContextType : ISerializationContext
            {
                serializer = JsonGenericDictionaryRepositorySerializationManager<KeyType, ItemType>.Singleton
                                as ISerializable<SerializedType>;

                return serializer != null;
            }
        }


        public static Boolean
            GetSerializationManager<SerializationContextType, SerializedType> ( this Object obj, ref ISerializable<SerializedType> serializer, SerializationContextType context )
                where SerializationContextType : ISerializationContext
        {
            if ( obj == null ) return false;

            ISerializationManagerProvider<SerializedType> serMgrPrvdr;

            serMgrPrvdr = obj as ISerializationManagerProvider<SerializedType>;

            if ( serMgrPrvdr == null )
                return SerializationManagerProviderExtensionImpl.GetSerializationManager (
                                                        (dynamic) obj, ref serializer, context );

            return (serializer = serMgrPrvdr.GetSerializationManager ( context )) != null;
        }

    } // End of Class SerializationManagerProviderExtension
} // End of Namespace Sharpframework.Serialization.ValueDom
