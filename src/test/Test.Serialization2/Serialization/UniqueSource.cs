using System;

using Sharpframework.Serialization;
using Sharpframework.Serialization.ValueDom;

using Sharpframework.EntityModel;
using Sharpframework.EntityModel.Implementation;
using Sharpframework.EntityModel.Implementation.Serialization;

namespace Test.EntityModel.Serialization
{
#region "Interfaces"
    public interface IBaseObjectSerializationContract
        : IEntitySerializationContract
    {
        IContainedObject SubObj { get; set; }
        Double Quantity { get; set; }
    } // End of Interface IBaseObjectSerializationContract

    public interface IBaseObject
        : IEntity
    {
        IContainedObject SubObj { get; }
        Double Quantity { get; }

        Boolean SetQuantity ( Double newQuantity );
        Boolean SetSubObj ( IContainedObject newSubObj );
    } // End of Interface IBaseObject

    public interface IContainedObject
        : IValueObject
    {
        String Name { get; }

        Boolean SetName ( String newName );
    } // End of Interface IContainedObject

    public interface IContainedObjectSerializationContract
        : IValueObjectSerializationContract
    {
        String Name { get; set; }
    } // End of Interface IContainedObjectSerializationContract

    public interface IDerivedObject
        : IBaseObject
    {
        DateTime TimeStamp { get; }

        Boolean SetTimeStamp ( DateTime newTimestamp );
    } // End of Interface IDerivedObject

    public interface IDerivedObjectSerializationContract
        : IBaseObjectSerializationContract
    {
        DateTime TimeStamp { get; set; }
    } // End of Interface IDerivedObjectSerializationContract
#endregion // Interfaces

#region "Factories"
    public abstract class BaseObjectFactory<InterfaceType, FactoryType>
        : EntityFactory<InterfaceType, FactoryType>
    where InterfaceType : IBaseObject
    where FactoryType   : BaseObjectFactory<InterfaceType, FactoryType>
                        , new ()
    {
        protected class BaseObject
            : Entity
            , IBaseObject
            , IBaseObjectSerializationContract
            , ISerializationManagerProvider<IValueUnit>
        {
            public BaseObject () : base () { }

            public BaseObject ( IValueUnit                                  valueUnit,
                                IHierarchicalMetadataSerializationContext   context )
                : base ( valueUnit, context ) { }


            public IContainedObject SubObj { get; set; }
            public Double Quantity { get; set; }


            public ISerializable<IValueUnit>
                GetSerializationManager<SerializationContextType> ( SerializationContextType context )
                    where SerializationContextType : ISerializationContext
                        => ImplGetSerializationManager ( context );

            public Boolean SetQuantity ( Double newQuantity )
            { Quantity = newQuantity; return true; }

            public Boolean SetSubObj ( IContainedObject newSubObj )
            { SubObj = newSubObj; return true; }


            protected virtual ISerializable<IValueUnit>
                ImplGetSerializationManager<SerializationContextType> ( SerializationContextType context )
            where SerializationContextType : ISerializationContext
            {
                return BaseObjectRepoSerMgr.Singleton;
            }

            protected override Type ImplGetSerializationContract<SerializationContextType> (
                SerializationContextType context )
            {
                return typeof ( IBaseObjectSerializationContract );
            }
        } // End of Class BaseObject
    } // End of Class BaseObjectFactory<...>

    public sealed class BaseObjectFactory
        : BaseObjectFactory<IBaseObject, BaseObjectFactory>
    {
        protected override IBaseObject ImplDeserialize<SerializationContextType> (
            IValueUnit                  serialization,
            SerializationContextType    context )
        => new BaseObject ( serialization, context as IHierarchicalMetadataSerializationContext );

        protected override IBaseObject ImplGetNew () => new BaseObject ();
    } // End of Class BaseObjectFactory

    public sealed class ContainedObjectFactory
        : ValueObjectFactoryBase<IContainedObject, ContainedObjectFactory>
    {
        private class ContainedObject
            : Entity
            , IContainedObject
            , IContainedObjectSerializationContract
            , ISerializationManagerProvider<IValueUnit>
        {
            public ContainedObject () : base () { }

            public ContainedObject (    IValueUnit                                  valueUnit,
                                        IHierarchicalMetadataSerializationContext   context )
                : base ( valueUnit, context ) { }


            public String Name { get; set; }


            public ISerializable<IValueUnit>
                GetSerializationManager<SerializationContextType> ( SerializationContextType context )
                    where SerializationContextType : ISerializationContext
                        => ImplGetSerializationManager ( context );

            public Boolean SetName ( String newName )
            { Name = newName; return true; }


            protected virtual ISerializable<IValueUnit>
                ImplGetSerializationManager<SerializationContextType> ( SerializationContextType context )
            where SerializationContextType : ISerializationContext
            {
                return ContainedObjectRepoSerMgr.Singleton;
            }

            protected override Type ImplGetSerializationContract<SerializationContextType> (
                SerializationContextType context )
            {
                return typeof ( IContainedObjectSerializationContract );
            }
        } // End of Class ContainedObjectFactory


        protected override IContainedObject ImplDeserialize<SerializationContextType> (
            IValueUnit                  serialization,
            SerializationContextType    context )
        => new ContainedObject ( serialization, context as IHierarchicalMetadataSerializationContext );

        protected override IContainedObject ImplGetNew () => new ContainedObject ();
    }


    public abstract class DerivedObjectFactory<InterfaceType, FactoryType>
        : BaseObjectFactory<InterfaceType, FactoryType>
    where InterfaceType : IDerivedObject
    where FactoryType   : DerivedObjectFactory<InterfaceType, FactoryType>
                        , new ()
    {
        protected class DerivedObject
            : BaseObject
            , IDerivedObject
            , IDerivedObjectSerializationContract
        {
            public DerivedObject () : base () { }

            public DerivedObject    (   IValueUnit                                  valueUnit,
                                        IHierarchicalMetadataSerializationContext   context )
                : base ( valueUnit, context ) { }


            public DateTime TimeStamp { get; set; }


            public Boolean SetTimeStamp ( DateTime newTimestamp )
            { TimeStamp = newTimestamp; return true; }


            protected override ISerializable<IValueUnit>
                ImplGetSerializationManager<SerializationContextType> ( SerializationContextType context )
            {
                return DerivedObjectRepoSerMgr.Singleton;
            }

            protected override Type ImplGetSerializationContract<SerializationContextType> (
                SerializationContextType context )
            {
                return typeof ( IDerivedObjectSerializationContract );
            }
        } // End of Class DerivedObject
    } // End of Class DerivedObjectFactory<...>

    public sealed class DerivedObjectFactory
        : DerivedObjectFactory<IDerivedObject, DerivedObjectFactory>
    {
        protected override IDerivedObject ImplDeserialize<SerializationContextType> (
            IValueUnit                  serialization,
            SerializationContextType    context )
        => new DerivedObject ( serialization, context as IHierarchicalMetadataSerializationContext );

        protected override IDerivedObject ImplGetNew () => new DerivedObject ();
    } // End of Class BaseObjectFactory

#endregion // Factories

#region "Serialization Managers"
    public abstract class JsonBaseObjectValueUnitRepositorySerializationManager<
                            InterfaceType,
                            FactoryType,
                            MyselfType>
        : JsonValueObjectValueUnitRepositorySerializationManager<
                            InterfaceType,
                            FactoryType,
                            MyselfType>
    where InterfaceType : IBaseObject
    where FactoryType   : BaseObjectFactory<InterfaceType, FactoryType>
                        , new ()
    where MyselfType    : JsonBaseObjectValueUnitRepositorySerializationManager<
                                InterfaceType,
                                FactoryType,
                                MyselfType>
                        , new ()
    {
    } // End of Class JsonBaseObjectValueUnitRepositorySerializationManager<...>

    public sealed class BaseObjectRepoSerMgr
        : JsonBaseObjectValueUnitRepositorySerializationManager<
                            IBaseObject,
                            BaseObjectFactory,
                            BaseObjectRepoSerMgr>
    {
    } // End of Class BaseObjectRepoSerMgr

    public sealed class ContainedObjectRepoSerMgr
        : JsonValueObjectValueUnitRepositorySerializationManager<
                            IContainedObject,
                            ContainedObjectFactory,
                            ContainedObjectRepoSerMgr>
    {
    } // End of Class ContainedObjectRepoSerMgr

    public abstract class JsonDerivedObjectValueUnitRepositorySerializationManager<
                            InterfaceType,
                            FactoryType,
                            MyselfType>
        : JsonBaseObjectValueUnitRepositorySerializationManager<
                            InterfaceType,
                            FactoryType,
                            MyselfType>
    where InterfaceType : IDerivedObject
    where FactoryType   : DerivedObjectFactory<InterfaceType, FactoryType>
                        , new ()
    where MyselfType    : JsonDerivedObjectValueUnitRepositorySerializationManager<
                                InterfaceType,
                                FactoryType,
                                MyselfType>
                        , new ()
    {
    } // End of Class JsonBaseObjectValueUnitRepositorySerializationManager<...>

    public sealed class DerivedObjectRepoSerMgr
        : JsonBaseObjectValueUnitRepositorySerializationManager<
                            IDerivedObject,
                            DerivedObjectFactory,
                            DerivedObjectRepoSerMgr>
    {
    } // End of Class DerivedObjectRepoSerMgr
    #endregion // Serialization Managers
} // End of Namespace Test.EntityModel.Serialization