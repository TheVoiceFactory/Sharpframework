using System;

using Sharpframework.Core;


namespace Sharpframework.Serialization.Json
{
    public class JsonConvertibleString
        : ConvertibleString<JsonConvertibleString>
    {
        public JsonConvertibleString () : base ( null ) { }

        public JsonConvertibleString ( String stringValue ) : base ( stringValue ) { }


        protected override DateTime ImplToDateTime ( IFormatProvider provider )
            => DateTime.ParseExact ( StringValue, "o", null );
    } // End of Class JsonConvertibleString
} // End of Namespace Sharpframework.Serialization.Json
