using Sharpframework.EntityModel.Implementation;
//using Sharpframework.Infrastructure.Serialization.Json;



namespace Test.EntityModel.Serialization
{
    public abstract class SubObjectFactory< InterfaceType,
                                            ContractType,
                                            ObjType,
                                            InitDtoType,
                                            FactoryType>
        : ValueObjectFactory<   InterfaceType,
                                ContractType,
                                ObjType,
                                InitDtoType,
                                FactoryType>
    where InterfaceType : ISubObject
    where ContractType  : ISubObjectContract
    where ObjType       : SubObject<ContractType> 
                        , InterfaceType
                        , new ()
    where InitDtoType   : new ()
    where FactoryType   : SubObjectFactory< InterfaceType,
                                            ContractType,
                                            ObjType,
                                            InitDtoType,
                                            FactoryType>
                        , new ()
    {
    } // End of Class SubObjectFactory<...>
} // End of Namespace Test.EntityModel.Serialization
