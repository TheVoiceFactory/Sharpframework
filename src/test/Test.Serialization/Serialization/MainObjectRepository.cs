//using System;

//using Sharpframework.Infrastructure;
////using Sharpframework.Implementations.Infrastructure.Data.Postgres;
//using Sharpframework.EntityModel;


//namespace Test.Demo.DBSerialization
//{
//    public abstract class MainObjectRepository< EntityType,
//                                                StorageAdapterType,
//                                                FactoryType,
//                                                RepositoryType>
//        : RepositoryBase<   EntityType,
//                            StorageAdapterType,
//                            FactoryType,
//                            RepositoryType>
//    where EntityType            : IMainObject
//    where StorageAdapterType    : IRepositorySerializer
//    where FactoryType           : IFactory<EntityType>
//                                , new ()
//    where RepositoryType        : MainObjectRepository< EntityType,
//                                                        StorageAdapterType,
//                                                        FactoryType,
//                                                        RepositoryType>
//                                , new ()
//    {
//        protected MainObjectRepository ( StorageAdapterType adapter, FactoryType factory )
//            : base ( adapter, factory ) { }
//    } // End of Class MainObjectRepository<...>

//    public abstract class MainObjectRepository< EntityType,
//                                                FactoryType,
//                                                RepositoryType>
//        : MainObjectRepository< EntityType,
//                                StorageAdapter,
//                                FactoryType,
//                                RepositoryType>
//    where EntityType            : IMainObject
//    where FactoryType           : IFactory<EntityType>
//                                , new ()
//    where RepositoryType        : MainObjectRepository< EntityType,
//                                                    FactoryType,
//                                                    RepositoryType>
//                                , new ()
//    {
//        protected MainObjectRepository ( FactoryType factory )
//            : base ( new StorageAdapter ( new PGStorage ( String.Empty, "Serialization" ) ), factory ) { }
//    } // End of Class MainObjectRepository<...>

//    public sealed class MainObjectRepository
//        : MainObjectRepository< IMainObject,
//                                MainObjectFactory,
//                                MainObjectRepository>
//    {
//        public MainObjectRepository ()
//            : base ( MainObjectFactory.Singleton )
//        { }
//    } // End of Class MainObjectRepository
//} // End of Namespace Test.Demo.DBSerialization
