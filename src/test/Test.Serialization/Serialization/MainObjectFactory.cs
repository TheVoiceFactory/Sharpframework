//using Sharpframework.Implementations.Domains.Shared.Model;
//using Sharpframework.Domains.Shared.Serialization.ValueDom;
//using Sharpframework.Implementations.Domains.Shared.Model;
using Sharpframework.EntityModel.Implementation.Serialization;
using Sharpframework.EntityModel.Implementation;

namespace Test.EntityModel.Serialization
{
    public abstract class MainObjectFactory<    InterfaceType,
                                                ContractType,
                                                ObjType,
                                                InitDtoType,
                                                FactoryType>
        : EntityFactory<    InterfaceType,
                            ContractType,
                            ObjType,
                            InitDtoType,
                            FactoryType>
    where InterfaceType : IMainObject
    where ContractType  : IMainObjectContract
    where ObjType       : MainObject<ContractType>
                        , InterfaceType
                        , new ()
    where InitDtoType   : new ()
    where FactoryType   : MainObjectFactory<    InterfaceType,
                                                ContractType,
                                                ObjType,
                                                InitDtoType,
                                                FactoryType>
                        , new ()
    {
    } // End of Class MainObjectFactory<...>


    public abstract class MainObjectFactory<    InterfaceType,
                                                ContractType,
                                                ObjType,
                                                FactoryType>
        : EntityFactory<    InterfaceType,
                            ContractType,
                            ObjType,
                            FactoryType>
    where InterfaceType : IMainObject
    where ContractType  : IMainObjectContract
    where ObjType       : MainObject<ContractType>
                        , InterfaceType
                        , new ()
    where FactoryType   : MainObjectFactory<    InterfaceType,
                                                ContractType,
                                                ObjType,
                                                FactoryType>
                        , new ()
    {
    } // End of Class MainObjectFactory<...>


    public sealed class MainObjectRepoSerMgr
        : JsonValueObjectValueUnitRepositorySerializationManager<
                            IMainObject,
                            MainObjectFactory,
                            MainObjectRepoSerMgr>
    {
    } // End of Class MainObjectRepoJsonConverter

    public sealed class MainObjectFactory
      : MainObjectFactory<  IMainObject,
                            IMainObjectContract,
                            MainObject,
                            MainObjectFactory>
    {
    } // End of Class MainObjectFactory
} // End of Namespace Test.EntityModel.Serialization
