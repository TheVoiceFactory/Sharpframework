using Sharpframework.EntityModel.Implementation;
using Sharpframework.EntityModel.Implementation.Serialization;


namespace Test.EntityModel.Serialization
{
    public abstract class SubObjectFactory< InterfaceType,
                                            ContractType,
                                            ObjType,
                                            FactoryType>
        : ValueObjectFactory<   InterfaceType,
                                ContractType,
                                ObjType,
                                FactoryType>
    where InterfaceType : ISubObject
    where ContractType  : ISubObjectContract
    where ObjType       : SubObject<ContractType>
                        , InterfaceType
                        , new ()
    where FactoryType   : SubObjectFactory< InterfaceType,
                                            ContractType,
                                            ObjType,
                                            FactoryType>
                        , new ()
    {
    } // End of Class SubObjectFactory<...>


    public sealed class SubObjectFactory
        : SubObjectFactory< ISubObject,
                            ISubObjectContract,
                            SubObject,
                            SubObjectFactory>
    {
    } // End of Class SubObjectFactory

    public sealed class SubObjectRepoSerMgr
        : JsonValueObjectValueUnitRepositorySerializationManager<
                            ISubObject,
                            SubObjectFactory,
                            SubObjectRepoSerMgr>
    {
    } // End of Class MainObjectRepoJsonConverter
} // End of Namespace Test.EntityModel.Serialization
