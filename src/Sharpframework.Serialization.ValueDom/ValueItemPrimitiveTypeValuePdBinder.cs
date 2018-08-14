using System;
using System.ComponentModel;
using System.Reflection;

using Sharpframework.Core;


namespace Sharpframework.Serialization.ValueDom
{
    public class ValueItemPrimitiveTypeValuePdBinder
        : ValueItemObjectValuePdBinder
    {
        public ValueItemPrimitiveTypeValuePdBinder (
            PropertyDescriptor pd, Object boundObject )
        : base ( pd, boundObject ) { }


        protected override Object ImplBoundObjectValue
        {
            get
            {
                IPrimitiveTypeValue primType = base.ImplBoundObjectValue as IPrimitiveTypeValue;

                return primType == null ? null : primType.Value;
            }

            set
            {
                IValueSetter valueSetter = null;

                if ( ImplBoundObjectValue == null )
                {
                    ConstructorInfo ci  = ImplPd.PropertyType.GetConstructor ( Type.EmptyTypes );

                    if ( ci == null ) return;
                    if ( (valueSetter = ci.Invoke ( null ) as IValueSetter) == null ) return;

                    base.ImplBoundObjectValue = valueSetter;
                }
                else if ( (valueSetter = ImplBoundObjectValue as IValueSetter) == null )
                    return;

                valueSetter.SetValue ( value );
            }
        }
    } // End of Class ValueItemPrimitiveTypeValuePdBinder
} // End of Namespace Sharpframework.Serialization.ValueDom
