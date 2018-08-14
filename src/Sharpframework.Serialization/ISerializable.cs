using System;


namespace Sharpframework.Serialization
{
    public interface ISerializable<SerializedType>
    {
        Object Deserialize<SerializationContextType> ( SerializedType serialization, SerializationContextType context )
            where SerializationContextType : ISerializationContext;

        SerializedType Serialize<SerializationContextType> ( Object instance, SerializationContextType context )
            where SerializationContextType : ISerializationContext;
    } // End of Interface ISerializable<...>

    public interface ISerializable<SerializedType, InstanceType>
        : ISerializable<SerializedType>
    {
        new InstanceType Deserialize<SerializationContextType> ( SerializedType serialization, SerializationContextType context )
            where SerializationContextType : ISerializationContext;

        SerializedType Serialize<SerializationContextType> ( InstanceType instance, SerializationContextType context )
            where SerializationContextType : ISerializationContext;
    } // End of Interface ISerializable<...>
} // End of Namespace Sharpframework.Serialization
