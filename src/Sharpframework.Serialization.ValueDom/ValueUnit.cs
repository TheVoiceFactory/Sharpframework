using System;

using Sharpframework.Core;


namespace Sharpframework.Serialization.ValueDom
{
    public class ValueUnit
        : KeyedCollectionBase<String, IValueItem>
        , IValueUnit
    {
        new IValueItem this [ String key ]
        { get { return Contains ( key ) ? base [ key ] : default ( IValueItem ); } }


        protected override String GetKeyForItem ( IValueItem item )
        { return item.Key; }


        public void Add ( String key, Object value )
        {
            base.Add ( new ValueItem ( key, value ) );
        } // End of Add (...)

        Object IValueProvider<Object>.Value => this;
    } // End of Class ValueUnit
} // End of Namespace Sharpframework.Serialization.ValueDom
