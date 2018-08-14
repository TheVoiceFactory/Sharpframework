using System;

//using Sharpframework.Implementations.Domains.Shared.Model;

using Sharpframework.Serialization;
using Sharpframework.Serialization.ValueDom;
using Sharpframework.EntityModel.Implementation;

namespace Test.EntityModel.Serialization
{
    //using Sharpframework.EntityModel.Implementation;
    //using Sharpframework.Implementations.Domains.Shared.Model;


    public abstract class SubObject<SerializationContractType>
        : ValueObject<SerializationContractType>
        , ISubObject
        , ISerializationManagerProvider<IValueUnit>
    where SerializationContractType : ISubObjectContract
    {
        public String SubObjField1 { get; set; }
        public String SubObjField2 { get; set; }


        public ISerializable<IValueUnit>
            GetSerializationManager<SerializationContextType> ( SerializationContextType context )
                where SerializationContextType : ISerializationContext
                    => ImplGetSerializationManager ( context );

        protected virtual ISerializable<IValueUnit>
            ImplGetSerializationManager<SerializationContextType> ( SerializationContextType context )
        where SerializationContextType : ISerializationContext
        {
            return SubObjectRepoSerMgr.Singleton;
        }
    } // End of Class MainObject<...>


    public sealed class SubObject
        : SubObject<ISubObjectContract>
    {
    } // End of Class SubObject
} // End of Namespace Test.EntityModel.Serialization
