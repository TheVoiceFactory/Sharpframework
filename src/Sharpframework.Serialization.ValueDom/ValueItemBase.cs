using System;


namespace Sharpframework.Serialization.ValueDom
{
    public class ValueItemBase
        : IValueItemBase
    {
        public ValueItemBase ( Object value ) { Value = value; }


        public Object Value { get; protected set; }
    } // End of Class ValueItemBase
} // End of Namespace Sharpframework.Serialization.ValueDom
