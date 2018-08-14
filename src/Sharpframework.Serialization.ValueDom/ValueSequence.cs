using System.Collections;


namespace Sharpframework.Serialization.ValueDom
{
    public class ValueSequence
        : ValueItemBase
        , IValueSequence
    {
        private IEnumerable __sequence;


        public ValueSequence () : this ( new ArrayList () ) { }

        public ValueSequence ( IEnumerable sequence )
            : base ( sequence ) { __sequence = sequence; }


        public new IEnumerable Value => __sequence;
    } // End of Class ValueSequence
} // End of Namespace Sharpframework.Serialization.ValueDom
