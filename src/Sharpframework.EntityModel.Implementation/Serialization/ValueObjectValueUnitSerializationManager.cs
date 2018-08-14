using System;

using Sharpframework.Core;
using Sharpframework.Serialization;
using Sharpframework.Serialization.ValueDom;



namespace Sharpframework.EntityModel.Implementation.Serialization
{
    public abstract class ValueObjectValueUnitSerializationManager< InterfaceType,
                                                                    FactoryType,
                                                                    AdapterType,
                                                                    SerializationContextType,
                                                                    MyselfType>
        : ValueObjectSerializationManager<  InterfaceType,
                                            FactoryType,
                                            AdapterType,
                                            IValueUnit,
                                            SerializationContextType,
                                            MyselfType>
        , ISerializable<IValueUnit, InterfaceType>
    where InterfaceType             : IValueObject
    where FactoryType               : SingletonObject<FactoryType>
                                    , IFactory<InterfaceType>
                                    , ISerializable<IValueUnit, InterfaceType>
                                    , new ()
    where AdapterType               : ValueDomAdapter<AdapterType>
                                    , new ()
    where SerializationContextType  : ISerializationContext
                                    , IHierarchicalMetadataProvider<IValueUnit, IValueItemBase>
    where MyselfType                : ValueObjectValueUnitSerializationManager<
                                                                    InterfaceType,
                                                                    FactoryType,
                                                                    AdapterType,
                                                                    SerializationContextType,
                                                                    MyselfType>
                        , new ()
    {
        protected override AdapterType ImplSymTblAdapter => ValueDomAdapter<AdapterType>.Singleton;

        protected override void ImplSerialize ( out IValueUnit retVal, InterfaceType instance, SerializationContextType context )
        {
            IHierarchicalMetadataProvider<IValueUnit, IValueItemBase>   ctx         = null;
            IValueUnit                                                  curCtxLvl   = null;

            ctx         = context as IHierarchicalMetadataProvider<IValueUnit, IValueItemBase>;
            curCtxLvl   = ctx.Hierarchy.Peek () as IValueUnit;

            if ( curCtxLvl != null && ctx != null && ctx.Metadata != null )
                curCtxLvl.Add ( "SerializationManager", GetType ().AssemblyQualifiedName );

            retVal = ImplFactory.Serialize ( instance, context );
        } // End of ImplSerialize (...)

        protected override InterfaceType ImplDeserialize ( IValueUnit valueUnit, SerializationContextType context )
        {
            IValueUnit                                          curCtxLvl   = null;
            SerializationManager<   InterfaceType,
                                    AdapterType,
                                    IValueUnit,
                                    SerializationContextType,
                                    MyselfType>                 repoSerMgr  = null;
            IValueItem                                          valueItem   = null;

            if ( (curCtxLvl = context.Hierarchy.Peek () as IValueUnit) == null )
                return default ( InterfaceType );

            valueItem = curCtxLvl [ "SerializationManager" ] as IValueItem;

            if ( valueItem != null )
                if ( GetType ().AssemblyQualifiedName.Equals ( valueItem.Value ) )
                    return base.ImplDeserialize ( valueUnit, context );
                else
                {
                    repoSerMgr = Activator.CreateInstance ( Type.GetType ( valueItem.Value.ToString () ) )
                                    as SerializationManager<InterfaceType,
                                                            AdapterType,
                                                            IValueUnit,
                                                            SerializationContextType,
                                                            MyselfType>;

                    if ( repoSerMgr != null ) return repoSerMgr.Deserialize ( valueUnit, context );
                }

            return default ( InterfaceType );
        } // End of ImplDeserialize (...)
    } // End of Class ValueObjectValueUnitSerializationManager<...>
} // End of Namespace Sharpframework.EntityModel.Implementation.Serialization
