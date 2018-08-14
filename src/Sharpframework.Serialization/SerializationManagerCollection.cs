using System;
using System.Reflection;

using Sharpframework.Core;


namespace Sharpframework.Serialization
{
    public abstract class SerializationManagerCollection<SerializedType, MyselfType>
        : SingletonObject<MyselfType>
    where SerializedType            : ISymbolTableItem
    where MyselfType                : SerializationManagerCollection<SerializedType, MyselfType>
                                    , new ()
    {
        protected class SerMgrColl
            : KeyedCollectionBase<String, SerializationManagerDescr>
        {
            protected override String GetKeyForItem ( SerializationManagerDescr item )
                => item == null ? null : item.TypeName;
        } // End of Class SerMgrColl


        public class SerializationManagerDescr
        {
            public SerializationManagerDescr (
                        String                                          typeName,
                        Type                                            type,
                        ObjectAllocator<ISerializable<SerializedType>>  allocator )
            {
                __allocator = allocator;
                __instance  = null;
                Type        = type;
                TypeName    = typeName;
            } // End of Custom Constructor

            private ObjectAllocator<ISerializable<SerializedType>>  __allocator;
            private ISerializable<SerializedType>                   __instance;

            public readonly Type                                    Type;
            public readonly String                                  TypeName;

            public ISerializable<SerializedType> SerializationManager
            {
                get
                {
                    if ( __instance == null && __allocator != null )
                        __instance = __allocator.Allocate ();

                    return __instance;
                }
            }
        }

        private class PropInfoAllocator
            : PropInfoObjectAllocator<ISerializable<SerializedType>>
        {
            public PropInfoAllocator ( PropertyInfo pi )
                : base ( pi ) { }
        }

        private class ConstructorAllocator
            : DefaultConstructorObjectAllocator<ISerializable<SerializedType>>
        {
            public ConstructorAllocator ( ConstructorInfo ci )
                : base ( ci ) { }
        }


        private static SerMgrColl __serMgrColl;


        static SerializationManagerCollection ()
        {
            __serMgrColl = new SerMgrColl ();
        } // End of Static Constructor


        public SerializationManagerDescr this [ String typeName ]
        {
            get
            {
                if ( String.IsNullOrWhiteSpace ( typeName ) ) return null;

                SerializationManagerDescr retVal;

                if ( __serMgrColl.TryGetValue ( typeName, out retVal ) ) return retVal;

                Type type = Type.GetType ( typeName );

                if ( type == null ) return null;

                ObjectAllocator<ISerializable<SerializedType>> allocator = null;

                if ( (allocator = ImplLookForSingleton ( type )) == null )
                    allocator = ImplLookForConstructor ( type );

                retVal = new SerializationManagerDescr ( typeName, type, allocator );

                __serMgrColl.Add ( retVal );

                return retVal;
            }
        }

        protected virtual ObjectAllocator<ISerializable<SerializedType>>
                            ImplLookForSingleton ( Type type )
        {
            PropertyInfo pi = type.GetProperty (
                                    "Singleton", 
                                    BindingFlags.Public
                                        | BindingFlags.Static
                                        | BindingFlags.FlattenHierarchy );

            if ( pi     == null             ) return null;
            if ( !pi.CanRead                ) return null;
            if ( type   != pi.PropertyType  ) return null;

            return pi.GetMethod == null ? null : new PropInfoAllocator ( pi );
        }

        protected virtual ObjectAllocator<ISerializable<SerializedType>>
                            ImplLookForConstructor ( Type type )
        {
            ConstructorInfo ci = type.GetConstructor ( Type.EmptyTypes );

            return ci == null ? null : new ConstructorAllocator ( ci );
        }
    } // End of Class SerializationManagerCollection<...>
} // End of Namespace Sharpframework.Serialization
