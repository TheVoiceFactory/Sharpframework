using System;
using System.Globalization;


namespace Sharpframework.Core
{
    public abstract class ConvertibleString<MyselfType>
        : SingletonObject<MyselfType>
        , IConvertibleString
    where MyselfType : class, new ()
    {
        protected const NumberStyles IntegerNumberStyle     = NumberStyles.AllowLeadingSign
                                                            | NumberStyles.AllowExponent;

        protected const NumberStyles FractionalNumberStyle  = NumberStyles.AllowLeadingSign
                                                            | NumberStyles.AllowExponent
                                                            | NumberStyles.AllowDecimalPoint;


        public ConvertibleString () : this ( null ) { }

        public ConvertibleString ( String stringValue )
        { StringValue = stringValue == null ? null : stringValue.Trim (); }


        public String StringValue { get; set; }

        public TypeCode GetTypeCode () => TypeCode.Object;

        public Boolean ToBoolean ( IFormatProvider provider )   => ImplToBoolean ( provider );
        public Byte ToByte ( IFormatProvider provider )         => ImplToByte ( provider );
        public Char ToChar  ( IFormatProvider provider )        => ImplToChar ( provider );
        public DateTime ToDateTime ( IFormatProvider provider ) => ImplToDateTime ( provider );
        public Decimal ToDecimal ( IFormatProvider provider )   => ImplToDecimal ( provider );
        public Double ToDouble ( IFormatProvider provider )     => ImplToDouble ( provider );
        public Int16 ToInt16 ( IFormatProvider provider )       => ImplToInt16 ( provider );
        public Int32 ToInt32 ( IFormatProvider provider )       => ImplToInt32 ( provider );
        public Int64 ToInt64 ( IFormatProvider provider )       => ImplToInt64 ( provider );
        public SByte ToSByte ( IFormatProvider provider )       => ImplToSByte ( provider );
        public Single ToSingle ( IFormatProvider provider )     => ImplToSingle ( provider );
        public String ToString ( IFormatProvider provider )     => StringValue;

        public Object ToType ( Type conversionType, IFormatProvider provider )
            => ImplToType ( conversionType, provider );

        public UInt16 ToUInt16 ( IFormatProvider provider )     => ImplToUInt16 ( provider );
        public UInt32 ToUInt32 ( IFormatProvider provider )     => ImplToUInt32 ( provider );
        public UInt64 ToUInt64 ( IFormatProvider provider )     => ImplToUInt64 ( provider );


        protected virtual Boolean ImplToBoolean ( IFormatProvider provider )
            => !(String.IsNullOrEmpty ( StringValue ) || StringValue.Trim ( '0' ).Length == 0);

        protected virtual Byte ImplToByte ( IFormatProvider provider )
            => Byte.Parse ( StringValue,
                            provider == null ? CultureInfo.InvariantCulture : provider );

        protected virtual Char ImplToChar ( IFormatProvider provider )
        {
            if ( StringValue        == null ) return '\0';
            if ( StringValue.Length == 1    ) return StringValue [ 0 ];

            throw new InvalidCastException ();
        } // End of ToChar (...)

        protected virtual DateTime ImplToDateTime ( IFormatProvider provider )
            => DateTime.Parse ( StringValue, provider );

        protected virtual Decimal ImplToDecimal ( IFormatProvider provider )
            => Decimal.Parse (  StringValue,
                                IntegerNumberStyle,
                                provider == null ? CultureInfo.InvariantCulture : provider );

        protected virtual Double ImplToDouble ( IFormatProvider provider )
            => Double.Parse (   StringValue,
                                FractionalNumberStyle,
                                provider == null ? CultureInfo.InvariantCulture : provider );

        protected virtual Int16 ImplToInt16 ( IFormatProvider provider )
            => Int16.Parse (    StringValue,
                                IntegerNumberStyle,
                                provider == null ? CultureInfo.InvariantCulture : provider );

        protected virtual Int32 ImplToInt32 ( IFormatProvider provider )
            => Int32.Parse (    StringValue,
                                IntegerNumberStyle,
                                provider == null ? CultureInfo.InvariantCulture : provider );

        protected virtual Int64 ImplToInt64 ( IFormatProvider provider )
            => Int64.Parse (    StringValue,
                                IntegerNumberStyle,
                                provider == null ? CultureInfo.InvariantCulture : provider );

        protected virtual SByte ImplToSByte ( IFormatProvider provider )
            => SByte.Parse (    StringValue,
                                NumberStyles.AllowLeadingSign,
                                provider == null ? CultureInfo.InvariantCulture : provider );

        protected virtual Single ImplToSingle ( IFormatProvider provider )
            => Single.Parse (   StringValue,
                                FractionalNumberStyle,
                                provider );

        protected virtual String ImplToString ( IFormatProvider provider ) => StringValue;

        protected virtual Object ImplToType ( Type conversionType, IFormatProvider provider )
        {
            switch ( Type.GetTypeCode ( conversionType) )
            {
                case TypeCode.Boolean   : return ToBoolean  ( provider );
                case TypeCode.Byte      : return ToByte     ( provider );
                case TypeCode.Char      : return ToChar     ( provider );
                case TypeCode.DateTime  : return ToDateTime ( provider );
                case TypeCode.Decimal   : return ToDecimal  ( provider );
                case TypeCode.Double    : return ToDouble   ( provider );
                case TypeCode.Int16     : return ToInt16    ( provider );
                case TypeCode.Int32     : return ToInt32    ( provider );
                case TypeCode.Int64     : return ToInt64    ( provider );
                case TypeCode.SByte     : return ToSByte    ( provider );
                case TypeCode.Single    : return ToSingle   ( provider );
                case TypeCode.String    : return ToString   ( provider );
                case TypeCode.UInt16    : return ToUInt16   ( provider );
                case TypeCode.UInt32    : return ToUInt32   ( provider );
                case TypeCode.UInt64    : return ToUInt64   ( provider );
            } // End of switch (...)

            return null;
        } // End of ToType (...)


        protected virtual UInt16 ImplToUInt16 ( IFormatProvider provider )
            => UInt16.Parse (   StringValue,
                                NumberStyles.AllowExponent,
                                provider == null ? CultureInfo.InvariantCulture : provider );

        protected virtual UInt32 ImplToUInt32 ( IFormatProvider provider )
            => UInt32.Parse (   StringValue,
                                NumberStyles.AllowExponent,
                                provider == null ? CultureInfo.InvariantCulture : provider );

        protected virtual UInt64 ImplToUInt64 ( IFormatProvider provider )
            => UInt64.Parse (   StringValue,
                                NumberStyles.AllowExponent,
                                provider == null ? CultureInfo.InvariantCulture : provider );
    } // End of Class ConvertibleString<...>
} // End of Namespace Sharpframework.Core
