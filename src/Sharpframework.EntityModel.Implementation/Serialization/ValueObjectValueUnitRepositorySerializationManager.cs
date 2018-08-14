using System;
using System.IO;

using Sharpframework.Core;
using Sharpframework.Serialization;
using Sharpframework.Serialization.ValueDom;



namespace Sharpframework.EntityModel.Implementation.Serialization
{
    public abstract class ValueObjectValueUnitRepositorySerializationManager<   InterfaceType,
                                                                                FactoryType,
                                                                                AdapterType,
                                                                                MyselfType>
        : ValueObjectValueUnitSerializationManager< InterfaceType,
                                                    FactoryType,
                                                    AdapterType,
                                                    IRepositorySerializationContext,
                                                    MyselfType>
    where InterfaceType : IValueObject
    where FactoryType   : SingletonObject<FactoryType>
                        , IFactory<InterfaceType>
                        , ISerializable<IValueUnit, InterfaceType>
                        , new ()
    where AdapterType   : ValueDomAdapter<AdapterType>
                        , new ()
    where MyselfType    : ValueObjectValueUnitRepositorySerializationManager<   InterfaceType,
                                                                                FactoryType,
                                                                                AdapterType,
                                                                                MyselfType>
                        , new ()
    {
        protected override InterfaceType ImplDeserialize (
            IValueUnit valueUnit, IRepositorySerializationContext context )
        {
            if ( valueUnit == null ) return default ( InterfaceType );

            IValueUnit ctxVu = null;
            IValueUnit tmpVu = valueUnit;
            IValueItem tmpVi = null;

            if ( tmpVu.Count == 2 && context.Metadata == context.Hierarchy.Peek () )
                if (    (tmpVi = tmpVu [ "$Metadata" ]                  ) != null
                    &&  (ctxVu = tmpVi.Value as IValueUnit              ) != null
                    &&  (tmpVi = tmpVu [ "$Data" ]                      ) != null
                    &&  (tmpVu = tmpVi.Value as IValueUnit              ) != null )
                {
                    valueUnit = tmpVu;

                    if ( context.Metadata.Count == 0 )
                        foreach ( IValueItem ctxItem in ctxVu )
                            context.Metadata.Add ( ctxItem.Key, ctxItem.Value );
                }

            return base.ImplDeserialize ( valueUnit, context );
        } // End of ImplDeserialize (...)

        protected override Boolean ImplSerializeData ( IRepositorySerializationContext context )
            => context != null && context.SerializeData;

        protected override void ImplWrite (
            TextWriter tw, IValueUnit source, IRepositorySerializationContext context )
        {
            if ( context.SerializeMetadata && context.Metadata != null )
                if ( source == null )
                    source = context.Metadata as IValueUnit;
                else
                {
                    IValueUnit wrapperVu = new ValueUnit ();

                    wrapperVu.Add ( "$Metadata", context.Metadata );
                    wrapperVu.Add ( "$Data", source );

                    source = wrapperVu;
                }

            base.ImplWrite ( tw, source, context );
        } // End of ImplWrite (...)
    } // End of Class ValueObjectValueUnitRepositorySerializationManager<...>
} // End of Namespace Sharpframework.EntityModel.Implementation.Serialization