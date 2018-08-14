using System.IO;


namespace Sharpframework.Serialization
{
    public interface ITextualSerializable<InstanceType>
    {
        InstanceType Deserialize ( TextReader serialization );

        TextWriter Serialize<SerializationContextType> ( InstanceType instance, SerializationContextType context )
            where SerializationContextType : ISerializationContext;

        void Serialize<SerializationContextType> ( TextWriter textWriter, InstanceType instance, SerializationContextType context )
            where SerializationContextType : ISerializationContext;
    } // End of Interface ITextualSerializable<...>

    public interface ITextualSerializable<InstanceType, SerializationContextType>
        where SerializationContextType : ISerializationContext
    {
        InstanceType Deserialize ( TextReader serialization, SerializationContextType context );

        TextWriter Serialize ( InstanceType instance, SerializationContextType context );

        void Serialize ( TextWriter textWriter, InstanceType instance, SerializationContextType context );
    } // End of Interface ITextualSerializable<...>
} // End of Namespace Sharpframework.Serialization
