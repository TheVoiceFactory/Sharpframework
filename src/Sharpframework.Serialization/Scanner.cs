﻿using Sharpframework.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Sharpframework.Serialization
{
    public abstract class Scanner<ScannerType, TokenType>
        where ScannerType : Scanner<ScannerType, TokenType>
                            , new()
        where TokenType : Scanner<ScannerType, TokenType>.TokenBase
    {
        public class TokenCollection
            : KeyedCollection<string, TokenBase>
        {
            public TokenCollection()
            {
            }

            public TokenCollection(IEnumerable<TokenBase> items)
            {
                if (items != null)
                {
                    foreach (TokenBase token in items)
                    {
                        Add(token);
                    }
                }
            }

            public new TokenBase this[string key]
            {
                get
                {

                    return TryGetValue(key, out TokenBase retVal) ? retVal : null;
                }
            }

            protected override string GetKeyForItem(TokenBase item)
            { return item == null ? null : item.StringToken; }
        } // End of Class TokenCollection

        public abstract class TokenBase
        {
            private static string __token;
            private static object __tokenType;

            static TokenBase()
            {
                __tokenType = null;
            }

            protected TokenBase(object tokenType, string token)
            {
                __token = token;
                __tokenType = tokenType == null ? this : tokenType;
            } // End of Custom Constructor

            public static object Eof => EofToken.Singleton.TokenType;
            public static object False => FalseToken.Singleton.TokenType;
            public static object Minus => MinusToken.Singleton.TokenType;
            public static object QuotationMarks => QuotationMarksToken.Singleton.TokenType;
            public static object True => TrueToken.Singleton.TokenType;

            public static TokenCollection LanguageTokens => __languageTokens;

            public object TokenType => __tokenType;
            public string StringToken => __token;
        } // End of Class TokenBase

        public abstract class Token<SelfTokenType>
            : TokenBase
        where SelfTokenType : Token<SelfTokenType>, new()
        {
            private static SelfTokenType __singleton;

            static Token()
            {
                __singleton = null;
            }

            public Token(string token) : this(__singleton, token)
            {
            }

            protected Token(object tokenType, string token)
                : base(__singleton, token) { }

            public static SelfTokenType Singleton
            {
                get
                {
                    if (__singleton == null)
                    {
                        __singleton = new SelfTokenType();
                    }

                    return __singleton;
                }
            }
        } // End of Class Token<...>

        public sealed class NumericToken
            : ValueTokenBase<string, NumericToken>
        {
            public NumericToken() : base(null)
            {
            }
        } // End of Class StringToken

        public abstract class OperatorTokenBase<OperatorTokenType>
            : Token<OperatorTokenType>
        where OperatorTokenType : OperatorTokenBase<OperatorTokenType>, new()
        {
            public OperatorTokenBase(string operatorStr) : base(operatorStr)
            {
            }

            protected OperatorTokenBase(object tokenType, string token)
                : base(__singleton, token) { }
        } // End of lass OperatorTokenBase<...>

        public abstract class SeparatorTokenBase<SeparatorTokenType>
            : Token<SeparatorTokenType>
        where SeparatorTokenType : SeparatorTokenBase<SeparatorTokenType>, new()
        {
            public SeparatorTokenBase(string separator) : base(separator)
            {
            }

            protected SeparatorTokenBase(object tokenType, string token)
                : base(__singleton, token) { }
        } // End of lass SeparatorTokenBase<...>

        public abstract class ValueTokenBase<ValueType, ValueTokenType>
            : Token<ValueTokenType>
            , IValueProvider<object>
        where ValueTokenType : ValueTokenBase<ValueType, ValueTokenType>, new()
        {
            public ValueTokenBase(string strValue) : base(strValue)
            {
            }

            public ValueType Value { get; set; }

            protected ValueTokenBase(object tokenType, string token)
                : base(__singleton, token) { }

            object IValueProvider<object>.Value => Value;
        } // End of Class FixedValueTokenBase<...>

        public sealed class StringToken
            : ValueTokenBase<string, StringToken>
        {
            public StringToken() : base(null)
            {
            }
        } // End of Class StringToken

        public abstract class FixedValueTokenBase<ValueType, ValueTokenType>
            : Token<ValueTokenType>
            , IValueProvider<object>
        where ValueTokenType : FixedValueTokenBase<ValueType, ValueTokenType>, new()
        {
            public FixedValueTokenBase(ValueType value, string strValue)
                : base(strValue) { Value = value; }

            public ValueType Value { get; private set; }

            protected FixedValueTokenBase(object tokenType, string token)
                : base(__singleton, token) { }

            object IValueProvider<object>.Value => Value;
        } // End of Class FixedValueTokenBase<...>

        public sealed class EofToken
            : Token<EofToken>

        {
            public EofToken() : base(null)
            {
            }
        }

        public sealed class FalseToken
            : FixedValueTokenBase<bool, FalseToken>

        {
            public FalseToken() : base(false, "false")
            {
            }
        }

        public sealed class MinusToken
            : OperatorTokenBase<MinusToken>

        {
            public MinusToken() : base("-")
            {
            }
        }

        public sealed class QuotationMarksToken
            : SeparatorTokenBase<QuotationMarksToken>

        {
            public QuotationMarksToken() : base("\"")
            {
            }
        }

        public sealed class TrueToken
            : FixedValueTokenBase<bool, TrueToken>

        {
            public TrueToken() : base(true, "true")
            {
            }
        }

        public sealed class BackslashToken
            : SeparatorTokenBase<BackslashToken>

        {
            public BackslashToken() : base("\\")
            {
            }
        }

        private static TokenCollection __languageTokens;
        private static ScannerType __singleton;

        static Scanner()
        {
            __languageTokens = null;
            __singleton = null;
        } // End of Static Constructor

        // QUESTO!!!
        protected virtual IEnumerable<TokenBase> ImplLanguageTokens
        {
            get
            {
                yield return BackslashToken.Singleton;
                yield return EofToken.Singleton;
                yield return FalseToken.Singleton;
                yield return MinusToken.Singleton;
                yield return QuotationMarksToken.Singleton;
                yield return TrueToken.Singleton;

                yield break;
            }
        }

        public static TokenCollection LanguageTokens
        {
            get
            {
                if (__languageTokens == null)
                {
                    __languageTokens = new TokenCollection(Singleton.ImplLanguageTokens);
                }

                return __languageTokens;
            }
        }

        public static ScannerType Singleton
        {
            get
            {
                if (__singleton == null)
                {
                    __singleton = new ScannerType();
                }

                return __singleton;
            }
        }

        //protected virtual String ImplScanString ( TextReader textReader )
        //{
        //    Int32           asc = -1;
        //    StringBuilder   sb  = new StringBuilder ();

        //    if ( !SkipWhitespaces ( textReader ) ) return null;

        //    while ( textReader.Peek () != -1 )
        //        switch ( asc = textReader.Read () )
        //        {
        //            case '"'    : return sb.ToString ();

        //            case '\\'   :
        //                if ( (asc = textReader.Read ()) == -1 ) return null;

        //                switch ( asc )
        //                {
        //                    case '"'    : sb.Append ( '"'   ); break;
        //                    case '\\'   : sb.Append ( '\\'  ); break;
        //                    case '/'    : sb.Append ( '/'   ); break;
        //                    case 'b'    : sb.Append ( '\b'  ); break;
        //                    case 'f'    : sb.Append ( '\f'  ); break;
        //                    case 'n'    : sb.Append ( '\n'  ); break;
        //                    case 'r'    : sb.Append ( '\r'  ); break;
        //                    case 't'    : sb.Append ( '\t'  ); break;
        //                    case 'u'    :
        //                        //int remainingLength = json.Length - index;
        //                        //if ( remainingLength >= 4 )
        //                        //{
        //                        //    // parse the 32 bit hex into an integer codepoint
        //                        //    uint codePoint;
        //                        //    if ( !(success = UInt32.TryParse ( new string ( json, index, 4 ), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint )) )
        //                        //    {
        //                        //        return "";
        //                        //    }
        //                        //    // convert the integer codepoint to a unicode char and add to string
        //                        //    s.Append ( Char.ConvertFromUtf32 ( (int)codePoint ) );
        //                        //    // skip 4 chars
        //                        //    index += 4;
        //                        //}
        //                        //else
        //                        //{
        //                        //    break;
        //                        //}
        //                        break;
        //                }
        //                break;

        //            default: sb.Append ( (Char) asc ); break;
        //        }

        //    return null;
        //} // ImplScanString (...)

        protected bool ImplIsDigit(int character)
        {
            switch (character)
            {
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '0':
                    return true;

                default: return false;
            }
        } // End of ImplIsDigit (...)

        protected virtual TokenBase ImplScanNumericElement(TextReader textReader)
        {
            int asc = -1;
            TokenBase retVal = null;
            StringBuilder sb = null;

            if (!SkipWhitespaces(textReader))
            {
                return null;
            }

            sb = new StringBuilder();

            if (textReader.Peek() == '-')
            {
                sb.Append('-');
                retVal = MinusToken.Singleton;
                textReader.Read();
            }

            if ((asc = textReader.Peek()) == '0')
            {
                sb.Append((char)asc);
                textReader.Read();
            }
            else
            {
                while (ImplIsDigit(asc))
                {
                    sb.Append((char)asc);
                    textReader.Read();
                    asc = textReader.Peek();
                }
            }

            if (sb.Length < 1)
            {
                return retVal;
            }

            if (asc == '.')
            {
                sb.Append((char)asc);
                textReader.Read();

                if (!ImplIsDigit(asc = textReader.Peek()))
                {
                    return retVal; // Invalid numeric token
                }

                sb.Append((char)asc);
                textReader.Read();

                while (ImplIsDigit(asc = textReader.Peek()))
                {
                    sb.Append((char)asc);
                    textReader.Read();
                }
            }

            if (asc == 'e' || asc == 'E')
            {
                sb.Append((char)asc);
                textReader.Read();

                if ((asc = textReader.Peek()) == '+' || asc == '-')
                {
                    sb.Append((char)asc);
                    textReader.Read();
                }

                if (!ImplIsDigit(asc))
                {
                    return retVal; // Invalid numeric token
                }

                sb.Append((char)asc);
                textReader.Read();

                while (ImplIsDigit(asc = textReader.Peek()))
                {
                    sb.Append((char)asc);
                    textReader.Read();
                }
            }

            NumericToken.Singleton.Value = sb.ToString();

            return NumericToken.Singleton;
        } // End of ImplScanNumericElement (...)

        protected virtual StringToken ImplScanString(TextReader textReader)
        {
            int asc = -1;
            StringBuilder sb = null;

            if (!SkipWhitespaces(textReader))
            {
                return null;
            }

            if ((asc = textReader.Peek()) != '"')
            {
                return null;
            }

            sb = new StringBuilder();

            textReader.Read();

            while ((asc = textReader.Read()) != -1)
            {
                switch (asc)
                {
                    case '"':
                        StringToken.Singleton.Value = sb.ToString();
                        return StringToken.Singleton;

                    case '\\':
                        if ((asc = textReader.Read()) == -1)
                        {
                            return null;
                        }

                        switch (asc)
                        {
                            case '"': sb.Append('"'); break;
                            case '\\': sb.Append('\\'); break;
                            case '/': sb.Append('/'); break;
                            case 'b': sb.Append('\b'); break;
                            case 'f': sb.Append('\f'); break;
                            case 'n': sb.Append('\n'); break;
                            case 'r': sb.Append('\r'); break;
                            case 't': sb.Append('\t'); break;
                            case 'u':
                                //int remainingLength = json.Length - index;
                                //if ( remainingLength >= 4 )
                                //{
                                //    // parse the 32 bit hex into an integer codepoint
                                //    uint codePoint;
                                //    if ( !(success = UInt32.TryParse ( new string ( json, index, 4 ), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint )) )
                                //    {
                                //        return "";
                                //    }
                                //    // convert the integer codepoint to a unicode char and add to string
                                //    s.Append ( Char.ConvertFromUtf32 ( (int)codePoint ) );
                                //    // skip 4 chars
                                //    index += 4;
                                //}
                                //else
                                //{
                                //    break;
                                //}
                                break;
                        }
                        break;

                    default: sb.Append((char)asc); break;
                }
            }

            return null;
        } // ImplScanString (...)

        //protected virtual TokenBase ImplScanToken ( TextReader textReader )
        //{
        //    if ( textReader == null ) return null;

        //    Char            ch      = ' ';
        //    Int32           chAsc   = -1;
        //    StringBuilder   sb      = new StringBuilder ();
        //    TokenBase       token   = null;

        //    if ( !SkipWhitespaces ( textReader ) ) return null;

        //    while ( (chAsc = textReader.Read ()) != -1 )
        //    {
        //        sb.Append ( ch = (Char) chAsc );

        //        token = LanguageTokens [ sb.ToString () ];

        //        if ( token != null )
        //            if ( token == QuotationMarksToken.Singleton )
        //            {
        //                String strValue = ScanString ( textReader );

        //                if ( strValue == null ) return EofToken.Singleton;

        //                StringToken.Singleton.Value = strValue;

        //                return StringToken.Singleton;
        //            }
        //            else
        //            {
        //                return token;
        //            }

        //        token = LanguageTokens [ ch.ToString () ];

        //        if ( token != null ) return null; // Ritornare un identifier.

        //        if ( ch == '-' || ch >= '0' && ch <= '9' )
        //            ; // is numeric or date
        //        else
        //            ; // is an identifier
        //    }

        //    return EofToken.Singleton;
        //} // End of ImplScanToken (...)

        protected virtual TokenBase ImplScanToken(TextReader textReader)
        {
            if (textReader == null)
            {
                return null;
            }

            char ch = ' ';
            int chAsc = -1;
            StringBuilder sb = new StringBuilder();
            TokenBase token = null;

            if (!SkipWhitespaces(textReader))
            {
                return null;
            }

            while ((chAsc = textReader.Peek()) != -1)
            {
                if ((token = ImplScanString(textReader)) != null)
                {
                    return token;
                }
                else if ((token = ImplScanNumericElement(textReader)) != null)
                {
                    return token;
                }

                sb.Append(ch = (char)chAsc);

                token = LanguageTokens[sb.ToString()];

                try
                {
                    if (token != null)
                    {
                        return token;
                    }

                    token = LanguageTokens[ch.ToString()];

                    if (token != null)
                    {
                        return null; // Ritornare un identifier.
                    }

                    if (ch == '-' || ch >= '0' && ch <= '9')
                    {
                        ; // is numeric or date
                    }
                    else
                    {
                        ; // is an identifier
                    }
                }
                finally { textReader.Read(); }
            }

            return EofToken.Singleton;
        } // End of ImplScanToken (...)

        public static bool SkipWhitespaces(TextReader textReader)
        {
            if (textReader == null)
            {
                return false;
            }

            int asc = 0;

            while ((asc = textReader.Peek()) != -1)
            {
                switch (asc)
                {
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                        textReader.Read(); break;

                    default: return true;
                }
            }

            return false;
        } // End of SkipWhitespaces (...)

        public string ScanString(TextReader textReader)
        {
            StringToken retVal = ImplScanString(textReader);

            return retVal == null ? null : retVal.Value;
        } // End of ScanString (...)

        public TokenBase ScanToken(TextReader textReader)
        { return ImplScanToken(textReader); }
    } // End of Class Scanner
} // End of Namespace Sharpframework.Serialization