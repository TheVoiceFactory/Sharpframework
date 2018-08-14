using System;

using Sharpframework.Serialization;
using Sharpframework.Serialization.ValueDom;
using Sharpframework.EntityModel.Implementation;


namespace Test.EntityModel.Serialization
{
    public class SubObject
        : ValueObject
        , ISubObject
        , ISubObjectSerializationContract
        , ISerializationManagerProvider<IValueUnit>
    {
        public String SubObjField1 { get; set; }
        public String SubObjField2 { get; set; }


        public SubObject () : base () { }

        public SubObject ( IValueUnit                                  valueUnit,
                            IHierarchicalMetadataSerializationContext   context )
            : base ( valueUnit, context ) { }


        public ISerializable<IValueUnit>
            GetSerializationManager<SerializationContextType> ( SerializationContextType context )
                where SerializationContextType : ISerializationContext
                    => ImplGetSerializationManager ( context );


        protected override Type ImplGetSerializationContract<SerializationContextType> ( SerializationContextType context )
        {
            return typeof ( ISubObjectSerializationContract );
        }

        protected virtual ISerializable<IValueUnit>
            ImplGetSerializationManager<SerializationContextType> ( SerializationContextType context )
        where SerializationContextType : ISerializationContext
        {
            return SubObjectRepoSerMgr.Singleton;
        }
    } // End of Class SubObject
} // End of Namespace Test.EntityModel.Serialization
