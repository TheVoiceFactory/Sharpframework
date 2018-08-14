using System;

using Sharpframework.Core;


namespace Sharpframework.Serialization.ValueDom
{
    public class ValueItem
        : ValueItemBase
        , IValueItem
    {
        public ValueItem ( String key, Object value )
            : base ( value ) { Key = key; }


        public String                       Key         { get; }
        public IValueItemObjectValueBinder  ValueBinder { get; set; }


        public Boolean AlignBoundValueToValue ()
        {
            if ( ValueBinder == null ) return false;

            if ( ValueBinder.BoundObjectValue == null )
                ValueBinder.BoundObjectValue = Value;
            else if ( Type.GetTypeCode ( ValueBinder.BoundObjectValue.GetType () )
                        == TypeCode.DateTime )
                return _BoundDateAlign ();
            else if ( typeof ( NumericStringValue ).IsInstanceOfType ( Value ) )
                return _BoundNumberAlign ();
            else
                ValueBinder.BoundObjectValue = Value;

            return true;
        } // End of AlignBoundValueToValue ()

        public Boolean AlignValueToBoundValue ()
        {
            IValueDomAlignable<IValueUnit, IValueUnit> valueDomAlignable = null;

            if ( ValueBinder == null ) return false;

            valueDomAlignable = ValueBinder.BoundObjectValue
                                    as IValueDomAlignable<IValueUnit, IValueUnit>;

            if ( valueDomAlignable != null ) return valueDomAlignable.Align ();

            Value = ValueBinder.BoundObjectValue;

            return true;
        } // End of AlignValueToBoundValue ()


        private Boolean _BoundDateAlign ()
        {
            if ( Value                                  == null             ) return false;
            if ( Type.GetTypeCode ( Value.GetType () )  != TypeCode.String  ) return false;

            ValueBinder.BoundObjectValue = DateTime.ParseExact ( Value as String, "o", null );

            return true;
        } // End of ImplBoundValueAlign (...)

        private Boolean _BoundNumberAlign ()
        {
            NumericStringValue numericString = Value as NumericStringValue;

            if ( numericString == null ) return false;

            ValueBinder.BoundObjectValue = numericString.ToType (
                                                ValueBinder.BoundObjectValue.GetType (), null );

            return true;
        } // End of ImplBoundValueAlign (...)

        private Boolean _BoundValueSetterAlign ()
        {
            IValueSetter setter =  ValueBinder.BoundObjectValue as IValueSetter;

            if ( setter == null ) return false;

            setter.SetValue ( Value );

            return true;
        } // End of ImplBoundValueAlign (...)
    } // End of Class ValueItem
} // End of Namespace Sharpframework.Serialization.ValueDom
