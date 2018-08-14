using Sharpframework.Core;


namespace Sharpframework.Serialization.ValueDom
{
    using Sharpframework.Serialization;


    public abstract class ValueDomAdapter<AdapterType>
        : SymbolTableAdapter<AdapterType, IValueUnit>
        , IValueDomAdapter
    where AdapterType : class, IValueDomAdapter, new ()
    {
        public ValueDomAdapter () : base () { }

        public ValueDomAdapter ( IConvertibleString converter ) : base ( converter ) { }
    } // End of Class ValueDomSerializer<...>
} // End of Namespace Sharpframework.Serialization.ValueDom
