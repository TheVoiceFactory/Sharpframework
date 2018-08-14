using System;
//using System.Globalization;


namespace Sharpframework.Core
{
    public abstract class NumericStringValue<MyselfType>
        : ConvertibleString<MyselfType>
    where MyselfType : NumericStringValue<MyselfType>, new ()
    {
        public NumericStringValue () : base () { }

        public NumericStringValue ( String stringValue ) : base ( stringValue) { }

        public new String StringValue { get { return base.StringValue; } }


        protected override String ImplToString ( IFormatProvider provider )
            => StringValue;

        protected override DateTime ImplToDateTime ( IFormatProvider provider )
            => throw new NotImplementedException ();
        //private const NumberStyles IntegerNumberStyle       = NumberStyles.AllowLeadingSign
        //                                                    | NumberStyles.AllowExponent;

        //private const NumberStyles FractionalNumberStyle    = NumberStyles.AllowLeadingSign
        //                                                    | NumberStyles.AllowExponent
        //                                                    | NumberStyles.AllowDecimalPoint;

        //public NumericStringValue ( String stringValue )
        //{ StringValue = stringValue == null ? null : stringValue.Trim (); }
        //public TypeCode GetTypeCode () => TypeCode.Object;

        //public Boolean ToBoolean ( IFormatProvider provider )
        //    => !(String.IsNullOrEmpty ( StringValue ) || StringValue.Trim ( '0' ).Length == 0);

        //public Byte ToByte ( IFormatProvider provider )
        //    => Byte.Parse ( StringValue,
        //                    provider == null ? CultureInfo.InvariantCulture : provider );

        //public Char ToChar ( IFormatProvider provider )
        //{
        //    if ( StringValue        == null ) return '\0';
        //    if ( StringValue.Length == 1    ) return StringValue [ 0 ];

        //    throw new InvalidCastException ();
        //} // End of ToChar (...)

        //public DateTime ToDateTime ( IFormatProvider provider )
        //    => throw new NotImplementedException ();

        //public Decimal ToDecimal ( IFormatProvider provider )
        //    => Decimal.Parse (  StringValue,
        //                        IntegerNumberStyle,
        //                        provider == null ? CultureInfo.InvariantCulture : provider );

        //public Double ToDouble ( IFormatProvider provider )
        //    => Double.Parse (   StringValue,
        //                        FractionalNumberStyle,
        //                        provider == null ? CultureInfo.InvariantCulture : provider );

        //public Int16 ToInt16 ( IFormatProvider provider )
        //    => Int16.Parse (    StringValue,
        //                        IntegerNumberStyle,
        //                        provider == null ? CultureInfo.InvariantCulture : provider );

        //public Int32 ToInt32 ( IFormatProvider provider )
        //    => Int32.Parse (    StringValue,
        //                        IntegerNumberStyle,
        //                        provider == null ? CultureInfo.InvariantCulture : provider );

        //public Int64 ToInt64 ( IFormatProvider provider )
        //    => Int64.Parse (    StringValue,
        //                        IntegerNumberStyle,
        //                        provider == null ? CultureInfo.InvariantCulture : provider );

        //public SByte ToSByte ( IFormatProvider provider )
        //    => SByte.Parse (    StringValue,
        //                        NumberStyles.AllowLeadingSign,
        //                        provider == null ? CultureInfo.InvariantCulture : provider );

        //public Single ToSingle ( IFormatProvider provider )
        //    => Single.Parse (   StringValue,
        //                        FractionalNumberStyle,
        //                        provider );

        //public String ToString ( IFormatProvider provider ) => StringValue;

        //    public Object ToType ( Type conversionType, IFormatProvider provider )
        //    {
        //        switch ( Type.GetTypeCode ( conversionType) )
        //        {
        //            case TypeCode.Boolean   : return ToBoolean  ( provider );
        //            case TypeCode.Byte      : return ToByte     ( provider );
        //            case TypeCode.Char      : return ToChar     ( provider );
        //            case TypeCode.DateTime  : return ToDateTime ( provider );
        //            case TypeCode.Decimal   : return ToDecimal  ( provider );
        //            case TypeCode.Double    : return ToDouble   ( provider );
        //            case TypeCode.Int16     : return ToInt16    ( provider );
        //            case TypeCode.Int32     : return ToInt32    ( provider );
        //            case TypeCode.Int64     : return ToInt64    ( provider );
        //            case TypeCode.SByte     : return ToSByte    ( provider );
        //            case TypeCode.Single    : return ToSingle   ( provider );
        //            case TypeCode.String    : return ToString   ( provider );
        //            case TypeCode.UInt16    : return ToUInt16   ( provider );
        //            case TypeCode.UInt32    : return ToUInt32   ( provider );
        //            case TypeCode.UInt64    : return ToUInt64   ( provider );
        //        } // End of switch (...)

        //        return null;
        //    } // End of ToType (...)

        //    public UInt16 ToUInt16 ( IFormatProvider provider )
        //        => UInt16.Parse (   StringValue,
        //                            NumberStyles.AllowExponent,
        //                            provider == null ? CultureInfo.InvariantCulture : provider );

        //    public UInt32 ToUInt32 ( IFormatProvider provider )
        //        => UInt32.Parse (   StringValue,
        //                            NumberStyles.AllowExponent,
        //                            provider == null ? CultureInfo.InvariantCulture : provider );

        //    public UInt64 ToUInt64 ( IFormatProvider provider )
        //        => UInt64.Parse (   StringValue,
        //                            NumberStyles.AllowExponent,
        //                            provider == null ? CultureInfo.InvariantCulture : provider );
    } // End of Class NumericStringValue<...>


    public sealed class NumericStringValue
        : NumericStringValue<NumericStringValue>
    {
        public NumericStringValue () : base () { }

        public NumericStringValue ( String stringValue ) : base ( stringValue) { }
    } // End of Class NumericStringValue
} // End of Namespace Sharpframework.Core
