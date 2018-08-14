using System;
using System.Collections.Generic;


namespace Sharpframework.Serialization.Json
{
    public class JsonScanner
        : Scanner<JsonScanner, JsonScanner.JsonToken>
    {
        public class JsonToken : TokenBase
        {
            public JsonToken ( Object tokenType, String token )
                : base ( tokenType, token ) { }


            public static Object Colon          => ColonToken.Singleton.TokenType;
            public static Object Comma          => CommaToken.Singleton.TokenType;
            public static Object CurlyClose     => CurlyCloseToken.Singleton.TokenType;
            public static Object CurlyOpen      => CurlyOpenToken.Singleton.TokenType;
            public static Object Null           => NullToken.Singleton.TokenType;
            public static Object SquaredClose   => SquaredCloseToken.Singleton.TokenType;
            public static Object SquaredOpen    => SquaredOpenToken.Singleton.TokenType;
        } // End of Class JsonToken


        public sealed class ColonToken
            : SeparatorTokenBase<ColonToken>
        { public ColonToken () : base ( ":" ) { } }

        public sealed class CommaToken
            : SeparatorTokenBase<CommaToken>
        { public CommaToken () : base ( "," ) { } }

        public sealed class CurlyCloseToken
            : SeparatorTokenBase<CurlyCloseToken>
        { public CurlyCloseToken () : base ( "}" ) { } }

        public sealed class CurlyOpenToken
            : SeparatorTokenBase<CurlyOpenToken>
        { public CurlyOpenToken () : base ( "{" ) { } }

        public sealed class NullToken
            : FixedValueTokenBase<Object, NullToken>
        { public NullToken () : base ( null, "null" ) { } }

        public sealed class SquaredCloseToken
            : SeparatorTokenBase<SquaredCloseToken>
        { public SquaredCloseToken () : base ( "]" ) { } }

        public sealed class SquaredOpenToken
            : SeparatorTokenBase<SquaredOpenToken>
        { public SquaredOpenToken () : base ( "[" ) { } }


        static JsonScanner ()
        {
        } // End of Static Constructor

        protected override IEnumerable<TokenBase> ImplLanguageTokens
        {
            get
            {
                yield return ColonToken.Singleton;
                yield return CommaToken.Singleton;
                yield return CurlyCloseToken.Singleton;
                yield return CurlyOpenToken.Singleton;
                yield return NullToken.Singleton;
                yield return SquaredCloseToken.Singleton;
                yield return SquaredOpenToken.Singleton;

                foreach ( TokenBase token in base.ImplLanguageTokens )
                    yield return token;

                yield break;
            }
        }
    } // End of Class JsonScanner
} // End of Namespace Sharpframework.Serialization.Json
