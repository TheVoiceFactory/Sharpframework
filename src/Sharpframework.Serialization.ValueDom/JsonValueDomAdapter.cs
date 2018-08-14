using System;
using System.Collections;
using System.Globalization;
using System.IO;

using Sharpframework.Core;
using Sharpframework.Serialization.Json;


namespace Sharpframework.Serialization.ValueDom
{
    using ValueDom;


    public class JsonValueDomAdapter
        : ValueDomAdapter<JsonValueDomAdapter>
    {
        public JsonValueDomAdapter () : base ( JsonConvertibleString.Singleton ) { }


        protected void ImplDispatchWriteValue ( TextWriter tw, dynamic value )
        { if ( value == null ) tw.Write ( "null" ); else ImplWriteValue ( tw, value ); }

        protected Boolean ImplIsBuiltinType ( Type type )
        {
            //if ( type.IsPrimitive ) return true;
            switch ( Type.GetTypeCode ( type ))
            {
                case TypeCode.DBNull: case TypeCode.Empty: case TypeCode.String:
                    return false;

                case TypeCode.Object:
                    break;

                default:
                    return true;
            }

            Type primValType = type.GetInterface ( "IHasPrimitiveValue`1" );

            if ( primValType == null ) return false;
            if ( !primValType.GetGenericArguments () [ 0 ].IsPrimitive ) return false;

            return true;
        } // End of ImplIsBuiltinType (...)

        protected Boolean ImplIsString ( Type type )
        {
            if ( type == typeof ( String ) ) return true;

            Type primValType = type.GetInterface ( "IHasPrimitiveValue`1" );

            if ( primValType == null ) return false;
            if ( primValType.GetGenericArguments () [ 0 ] != typeof ( String ) ) return false;

            return true;
        } // End of ImplIsString (...)

        protected virtual ArrayList ImplParseArray (
            out JsonScanner.TokenBase   endToken,
                TextReader              textReader )
        {
            endToken = JsonScanner.EofToken.Singleton;

            if ( textReader == null ) return null;

            JsonScanner.NumericToken    numericToken    = null;
            ArrayList                   retVal          = new ArrayList ();
            //JsonScanner.StringToken     strToken      = null;
            JsonScanner.TokenBase       token           = null;
            Object                      value           = null;
            IValueProvider<Object>      valueProvider   = null;

            do
            {
                token = JsonScanner.Singleton.ScanToken ( textReader );

                if ( token == JsonScanner.SquaredOpenToken.Singleton )
                {
                }
                else if ( token == JsonScanner.CurlyOpenToken.Singleton )
                {
                    value = ImplParseUnit ( out token, textReader );

                    if ( token != JsonScanner.CurlyCloseToken.Singleton ) return null;
                }
                else if ( (numericToken = token as JsonScanner.NumericToken) != null )
                    value = new NumericStringValue ( numericToken.Value );
                else if ( (valueProvider = token as IValueProvider<Object>) != null )
                    value = valueProvider.Value;
                else
                    return null; // Unespected token

                retVal.Add ( value );

                token = JsonScanner.Singleton.ScanToken ( textReader );
            }
            while ( token == JsonScanner.CommaToken.Singleton );

            endToken = token;

            return retVal;
        } // End of ImplParseArray (...)

        protected virtual IValueUnit ImplParseUnit (
            out JsonScanner.TokenBase   endToken,
                TextReader              textReader )
        {
            endToken = JsonScanner.EofToken.Singleton;

            if ( textReader == null ) return null;

            JsonScanner.NumericToken    numericToken    = null;
            String                      key             = null;
            JsonScanner.TokenBase       token           = null;
            ValueUnit                   retVal          = new ValueUnit ();
            JsonScanner.StringToken     strToken        = null;
            Object                      value           = null;
            IValueProvider<Object>      valueProvider   = null;

            do
            {
                token = JsonScanner.Singleton.ScanToken ( textReader );

                if ( (strToken = token as JsonScanner.StringToken) == null )
                    return null; // Unespected token
                else
                    key = strToken.Value;

                token = JsonScanner.Singleton.ScanToken ( textReader );

                if ( token != JsonScanner.ColonToken.Singleton )
                    return null; // Unespected token

                token = JsonScanner.Singleton.ScanToken ( textReader );

                if ( (strToken = token as JsonScanner.StringToken) != null )
                    value = strToken.Value;
                else if ( token == JsonScanner.SquaredOpenToken.Singleton )
                {
                    value = ImplParseArray ( out token, textReader );

                    if ( token == JsonScanner.SquaredCloseToken.Singleton )
                        value = new ValueSequence ( (IEnumerable) value );
                    else
                        return null;
                }
                else if ( token == JsonScanner.CurlyOpenToken.Singleton )
                {
                    value = ImplParseUnit ( out token, textReader );

                    if ( token != JsonScanner.CurlyCloseToken.Singleton ) return null;
                }
                else if ( (numericToken = token as JsonScanner.NumericToken) != null )
                    value = new NumericStringValue ( numericToken.Value );
                else if ( (valueProvider = token as IValueProvider<Object>) != null )
                    value = valueProvider.Value;
                else
                    return null; // Unespected token

                retVal.Add ( new ValueItem ( key, value ) );

                token = JsonScanner.Singleton.ScanToken ( textReader );
            }
            while ( token == JsonScanner.CommaToken.Singleton );

            endToken = token;

            return retVal;
        } // End of ImplParseUnit (...)

        protected override IValueUnit ImplRead ( TextReader textReader )
        {
            IValueUnit              retVal  = null;
            JsonScanner.TokenBase   token   = JsonScanner.Singleton.ScanToken ( textReader );

            if ( token != JsonScanner.CurlyOpenToken.Singleton )
                return null; // Unespected token

            retVal = ImplParseUnit ( out token, textReader );

            return token == JsonScanner.CurlyCloseToken.Singleton ? retVal : null;
        } // End of ImplRead (...)

        protected override void ImplWrite ( TextWriter tw, IValueUnit root )
        {
            if ( tw     == null ) return;
            if ( root   == null ) return;

            _Write ( tw, root );
        } // End of ImplWrite (...)

        protected virtual void ImplWriteValue ( TextWriter tw, Boolean value )
            => tw.Write ( value.ToString ().ToLowerInvariant () );

        protected virtual void ImplWriteValue ( TextWriter tw, Byte value )
            => tw.Write ( Convert.ToString ( value, CultureInfo.InvariantCulture ) );

        protected virtual void ImplWriteValue ( TextWriter tw, Char value )
        {
            tw.Write ( '"' );
            tw.Write ( Convert.ToString ( value, CultureInfo.InvariantCulture ) );
            tw.Write ( '"' );
        } // End of ImplWriteValue (...)

        protected virtual void ImplWriteValue ( TextWriter tw, DateTime value )
        {
            tw.Write ( '"' );
            tw.Write ( value.ToString ( "o" ) );
            tw.Write ( '"' );
        } // End of ImplWriteValue (...)

        protected virtual void ImplWriteValue ( TextWriter tw, Decimal value )
            => tw.Write ( Convert.ToString ( value, CultureInfo.InvariantCulture ) );

        protected virtual void ImplWriteValue ( TextWriter tw, Double value )
            => tw.Write ( Convert.ToString ( value, CultureInfo.InvariantCulture ) );

        protected virtual void ImplWriteValue ( TextWriter tw, Int16 value )
            => tw.Write ( Convert.ToString ( value, CultureInfo.InvariantCulture ) );

        protected virtual void ImplWriteValue ( TextWriter tw, Int32 value )
            => tw.Write ( Convert.ToString ( value, CultureInfo.InvariantCulture ) );

        protected virtual void ImplWriteValue ( TextWriter tw, Int64 value )
            => tw.Write ( Convert.ToString ( value, CultureInfo.InvariantCulture ) );

        protected virtual void ImplWriteValue ( TextWriter tw, Object value )
        {
            tw.Write ( value );
        } // End of ImplWriteValue (...)

        protected virtual void ImplWriteValue ( TextWriter tw, IValueProvider<Object> value )
        {
            ImplDispatchWriteValue ( tw, value.Value );
        } // End of ImplWriteValue (...)

        protected virtual void ImplWriteValue ( TextWriter tw, SByte value )
            => tw.Write ( Convert.ToString ( value, CultureInfo.InvariantCulture ) );

        protected virtual void ImplWriteValue ( TextWriter tw, Single value )
            => tw.Write ( Convert.ToString ( value, CultureInfo.InvariantCulture ) );

        protected virtual void ImplWriteValue ( TextWriter tw, String value )
        {
            tw.Write ( '"' );
            tw.Write ( value.Replace ( "\"", "\\\"" ) ); 
            tw.Write ( '"' );
        } // End of ImplWriteValue (...)

        protected virtual void ImplWriteValue ( TextWriter tw, IEnumerable value )
        {
            tw.Write ( '[' );

            if ( value != null )
            {
                String      itemSep = String.Empty;
                IValueUnit  vu      = null;

                foreach ( Object item in value )
                {
                    tw.Write ( itemSep );

                    if ( (vu = item as IValueUnit) != null )
                        _Write ( tw, vu );
                    else
                        ImplDispatchWriteValue ( tw, item );

                    itemSep = ",";
                }
            }

            tw.Write ( ']' );
        } // End of ImplWriteValue (...)

        protected virtual void ImplWriteValue ( TextWriter tw, UInt16 value )
            => tw.Write ( Convert.ToString ( value, CultureInfo.InvariantCulture ) );

        protected virtual void ImplWriteValue ( TextWriter tw, UInt32 value )
            => tw.Write ( Convert.ToString ( value, CultureInfo.InvariantCulture ) );

        protected virtual void ImplWriteValue ( TextWriter tw, UInt64 value )
            => tw.Write ( Convert.ToString ( value, CultureInfo.InvariantCulture ) );

        private void _Write ( TextWriter tw, IValueUnit valueUnit )
        {
            tw.Write ( '{' );

            String      itemSep = String.Empty;
            IValueUnit  subUnit = null;

            foreach ( IValueItem valueItem in valueUnit )
            {
                tw.Write ( itemSep );
                tw.Write ( '"' );
                tw.Write ( valueItem.Key );
                tw.Write ( '"' );
                tw.Write ( ':' );

                if ( (subUnit = valueItem.Value as IValueUnit) == null )
                    ImplDispatchWriteValue ( tw, valueItem.Value );
                else
                    _Write ( tw, subUnit );

                itemSep = ",";
            }

            tw.Write ( '}' );
        } // End of _Write (...)
    } // End of Class JsonValueDomAdapter
} // End of Namespace Sharpframework.Serialization.ValueDom
