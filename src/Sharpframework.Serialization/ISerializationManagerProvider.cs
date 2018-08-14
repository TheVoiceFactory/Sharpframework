
namespace Sharpframework.Serialization
{
    public interface ISerializationManagerProvider<SerializedType>
    {
        ISerializable<SerializedType>
            GetSerializationManager<SerializationContextType> ( SerializationContextType context )
                where SerializationContextType : ISerializationContext;
    } // End of Interface ISerializationManagerProvider<...>
} // End of Namespace Sharpframework.Serialization
