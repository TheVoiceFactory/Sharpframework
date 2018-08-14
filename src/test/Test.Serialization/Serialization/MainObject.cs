using System;
using System.Collections;
using System.Collections.Generic;

using Sharpframework.Serialization;
using Sharpframework.Serialization.ValueDom;
using Sharpframework.EntityModel.Implementation;

namespace Test.EntityModel.Serialization
{
    //using Sharpframework.EntityModel.Implementation;
    

    public abstract class MainObject<SerializationContractType>
        : Entity<SerializationContractType>
        , IMainObject
        , ISerializationManagerProvider<IValueUnit>
    where SerializationContractType : IMainObjectContract
    {
        public String Field1 { get; set; }

        public NameType Field2 { get; set; }

        public ISubObject Field3 { get; set; }

        public Boolean Field4 { get; set; }

        public IList<ISubObject> Field5 { get; set; }

        public IList<Object> Field6 { get; set; }

        public IList Field6Bis { get; set; }

        public IDictionary<String, ISubObject> Field7 { get; set; }

        public Double Field8 { get; set; }

        public DateTime DateTimeField { get; set; }

        public Boolean [] BooleanArray { get; set; }

        public DateTime [] DateTimeArray { get; set; }

        public Int32 [] Int32Array { get; set; }

        public Object [] MixedArray { get; set; }

        public ISubObject [] SubObjectArray { get; set; }


        public ISerializable<IValueUnit>
            GetSerializationManager<SerializationContextType> ( SerializationContextType context )
                where SerializationContextType : ISerializationContext
                    => ImplGetSerializationManager ( context );

        protected virtual ISerializable<IValueUnit>
            ImplGetSerializationManager<SerializationContextType> ( SerializationContextType context )
        where SerializationContextType : ISerializationContext
        {
            return MainObjectRepoSerMgr.Singleton;
        }
    } // End of Class MainObject<...>

    public sealed class MainObject
        : MainObject<IMainObjectContract>
    { }
} // End of Namespace Test.EntityModel.Serialization
