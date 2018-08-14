using System;

using Sharpframework.Serialization;
using Sharpframework.Serialization.ValueDom;
using Sharpframework.EntityModel.Implementation;

namespace Test.EntityModel.Serialization
{
    public class MainObject
        : Entity
        , IMainObject
        , IMainObjectSerializationContract
        , ISerializationManagerProvider<IValueUnit>
    {
        public String Field1 { get; set; }

        public NameType Field2 { get; set; }

        public SubObject Field3 { get; set; }

        public Boolean Field4 { get; set; }

        ISubObject IMainObject.Field3 { get => Field3; }
        ISubObject IMainObjectSerializationContract.Field3
        { get => Field3; set => Field3 = (SubObject) value; }

        //public IList<ISubObject> Field5 { get; set; }

        //public IList<Object> Field6 { get; set; }

        //public IList Field6Bis { get; set; }

        //public IDictionary<String, ISubObject> Field7 { get; set; }

        public Double Field8 { get; set; }

        public DateTime DateTimeField { get; set; }

        //public Boolean [] BooleanArray { get; set; }

        //public DateTime [] DateTimeArray { get; set; }

        //public Int32 [] Int32Array { get; set; }

        //public Object [] MixedArray { get; set; }

        //public ISubObject [] SubObjectArray { get; set; }


        public MainObject () : base () { }

        public MainObject (    IValueUnit                                  valueUnit,
                                IHierarchicalMetadataSerializationContext   context )
            : base ( valueUnit, context ) { }


        public ISerializable<IValueUnit>
            GetSerializationManager<SerializationContextType> ( SerializationContextType context )
                where SerializationContextType : ISerializationContext
                    => ImplGetSerializationManager ( context );

        protected virtual ISerializable<IValueUnit>
            ImplGetSerializationManager<SerializationContextType> ( SerializationContextType context )
        where SerializationContextType : ISerializationContext
        {
            return MainObjectFactory.Singleton;
        }

        protected override Type ImplGetSerializationContract<SerializationContextType> (
            SerializationContextType context )
        {
            return typeof ( IMainObjectSerializationContract );
        }
    } // End of Class MainObject
} // End of Namespace Test.EntityModel.Serialization
