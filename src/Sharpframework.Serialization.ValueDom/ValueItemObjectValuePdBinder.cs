using System;
using System.ComponentModel;


namespace Sharpframework.Serialization.ValueDom
{
    public class ValueItemObjectValuePdBinder
        : IValueItemObjectValueBinder
    {
        private PropertyDescriptor  __pd;
        private Object              __boundObject;


        public ValueItemObjectValuePdBinder ( PropertyDescriptor pd, Object boundObject )
        {
            __pd            = pd;
            __boundObject   = boundObject;
        } // End of Custom Constructor


        public Object BoundObjectValue
        { get => ImplBoundObjectValue; set => ImplBoundObjectValue = value; }


        protected virtual Object ImplBoundObjectValue
        {
            get => __pd.GetValue ( __boundObject );
            set => __pd.SetValue ( __boundObject, value );
        }

        protected PropertyDescriptor ImplPd => __pd;
    } // End of Class ValueItemObjectValuePdBinder
} // End of Namespace Sharpframework.Serialization.ValueDom
