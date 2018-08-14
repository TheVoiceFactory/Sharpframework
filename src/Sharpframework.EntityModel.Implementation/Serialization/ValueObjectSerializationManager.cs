using System;
using System.IO;

using Sharpframework.Core;


namespace Sharpframework.EntityModel.Implementation.Serialization
{
    using Sharpframework.Serialization;
    using Sharpframework.Serialization.ValueDom;


    public abstract class ValueObjectSerializationManager<  InterfaceType,
                                                            FactoryType,
                                                            AdapterType,
                                                            SerializedType,
                                                            SerializationContextType,
                                                            MyselfType>
        : SerializationManager< InterfaceType,
                                AdapterType,
                                SerializedType,
                                SerializationContextType,
                                MyselfType>
        , ITextualSerializable<InterfaceType, SerializationContextType>
    where InterfaceType             : IValueObject
    where FactoryType               : SingletonObject<FactoryType>
                                    , IFactory<InterfaceType>
                                    , ISerializable<SerializedType, InterfaceType>
                                    , new ()
    where AdapterType               : class
                                    , ISymbolTableAdapter<SerializedType>
                                    , new ()
    where SerializedType            : ISymbolTableRoot
    where SerializationContextType  : ISerializationContext
                                    , IHierarchicalMetadataProvider<IValueUnit, IValueItemBase>
    where MyselfType                : ValueObjectSerializationManager<  InterfaceType,
                                                                        FactoryType,
                                                                        AdapterType,
                                                                        SerializedType,
                                                                        SerializationContextType,
                                                                        MyselfType>
                                    , new ()
    {
        public InterfaceType Deserialize (  TextReader                  textReader,
                                            SerializationContextType    context )
        {
            if ( textReader == null ) return default ( InterfaceType );
            if ( context    == null ) return default ( InterfaceType );

            return ImplDeserialize ( textReader, context );
        } // End of Deserialize (...)

        public TextWriter Serialize (   SerializedType              serialization,
                                        SerializationContextType    context )
        {
            TextWriter tw = new StringWriter ();

            Serialize ( tw, serialization, context );

            return tw;
        } // End of Serialize (...)

        public void Serialize ( TextWriter                  textwriter,
                                SerializedType              serialization,
                                SerializationContextType    context )
        {
            if ( textwriter     == null ) return;
            if ( serialization  == null ) return;
            if ( context        == null ) return;

            ImplWrite ( textwriter, serialization, context );
        } // End of Serialize (...)

        public void Serialize ( TextWriter                  textWriter,
                                InterfaceType               instance,
                                SerializationContextType    context )
            => ImplSerialize ( textWriter, instance, context );


        protected static FactoryType ImplFactory => SingletonObject<FactoryType>.Singleton;


        protected abstract AdapterType ImplSymTblAdapter { get; }


        protected override InterfaceType ImplDeserialize ( 
                SerializedType              serialization,
                SerializationContextType    context )
            => ImplFactory.Deserialize ( serialization, context );

        protected virtual InterfaceType ImplDeserialize (   TextReader                  textReader,
                                                            SerializationContextType    context )
        {
            SerializedType resultVu = ImplSymTblAdapter.Read ( textReader );

            return ImplDeserialize ( resultVu, context );
        } // End of ImplDeserialize (...)

        protected virtual Boolean ImplSerializeData ( SerializationContextType context )
        {
            ISerializeData tmp = context as ISerializeData;

            return tmp == null || tmp.SerializeData;
        } // End of ImplSerializeData (...)

        protected virtual void ImplSerialize (  TextWriter                  textWriter,
                                                InterfaceType               instance,
                                                SerializationContextType    context )
        {
            SerializedType source;

            if ( ImplSerializeData ( context ) )
                ImplSerialize ( out source, instance, context );
            else
                source = default ( SerializedType );

            ImplWrite ( textWriter, source, context );
        } // End of ImplSerialize (...)

        protected virtual void ImplWrite (  TextWriter                  tw,
                                            SerializedType              source,
                                            SerializationContextType    context )
            => ImplSymTblAdapter.Write ( tw, source );


        TextWriter ITextualSerializable<InterfaceType, SerializationContextType>
            .Serialize ( InterfaceType instance, SerializationContextType context )
        {
            TextWriter tw = new StringWriter ();

            Serialize ( tw, instance, context );

            return tw;
        } // End of ITextualSerializable<...>.Serialize Explicit Implementation
    } // End of Class ValueObjectSerializationManager<...>
} // End of Namespace Sharpframework.EntityModel.Implementation.Serialization
