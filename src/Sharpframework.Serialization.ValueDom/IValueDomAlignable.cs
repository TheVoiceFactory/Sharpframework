using System;

using Sharpframework.Core;


namespace Sharpframework.Serialization.ValueDom
{
    //public interface IValueDomAlignable
    //    : IValueDomAlignable<IValueUnit, IValueUnit>
    //{ }

    public interface IValueDomAlignable<DataItemType, MetadataItemType>
        : IDataProvider<DataItemType>
        , IMetadataProvider<MetadataItemType>
    {
        Boolean Align ();

         Boolean GetContext<ContextType> ( ref ContextType context )
            where ContextType : IContext;
    } // End of Interface IValueDomAlignable<...>
} // End of Namespace Sharpframework.Serialization.ValueDom