using Sharpframework.Core;
using Sharpframework.Serialization;
//using Sharpframework.Infrastructure.Serialization.Json;
using Sharpframework.Serialization.ValueDom;


namespace Sharpframework.EntityModel.Implementation.Serialization
{
    public abstract class JsonValueObjectValueUnitRepositorySerializationManager<
                            InterfaceType,
                            FactoryType,
                            MyselfType>
        : ValueObjectValueUnitRepositorySerializationManager<
                            InterfaceType,
                            FactoryType,
                            JsonValueDomAdapter,
                            MyselfType>
    where InterfaceType : IValueObject
    where FactoryType   : SingletonObject<FactoryType>
                        , IFactory<InterfaceType>
                        , ISerializable<IValueUnit, InterfaceType>
                        , new ()
    where MyselfType    : JsonValueObjectValueUnitRepositorySerializationManager<
                                InterfaceType,
                                FactoryType,
                                MyselfType>
                        , new ()
    {
    } // End of Class JsonValueObjectValueUnitRepositorySerializationManager<...>
} // End of Namespace Sharpframework.EntityModel.Implementation.Serialization
