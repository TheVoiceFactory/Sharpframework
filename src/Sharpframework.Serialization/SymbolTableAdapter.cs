using System;
using System.IO;

using Sharpframework.Core;


namespace Sharpframework.Serialization
{
    public abstract class SymbolTableAdapter<AdapterType, RootType>
        : SingletonObject<AdapterType>
        , ISymbolTableAdapter<RootType>
    where RootType      : ISymbolTableRoot
    where AdapterType   : class
                        , ISymbolTableAdapter<RootType>
                        , new ()
    {
        //private static AdapterType __singleton;


        private IConvertibleString __converter;


        public SymbolTableAdapter () : this ( null ) { }

        public SymbolTableAdapter ( IConvertibleString converter ) { __converter = converter; }


        //static SymbolTableAdapter ()
        //{
        //    __singleton = default ( AdapterType );
        //} // End of Static Constructor


        //public static AdapterType Singleton
        //{
        //    get
        //    {
        //        if ( __singleton == null ) __singleton = new AdapterType ();

        //        return __singleton;
        //    }
        //}


        public IConvertibleString Converter { get => ImplConverter; }


        public RootType Read ( TextReader textReader )
            => ImplRead ( textReader );

        public TextWriter Write ( RootType root )
        {
            TextWriter tw = new StringWriter ();

            ImplWrite ( tw, root );

            return tw;
        } // End of Write (...)

        public void Write ( TextWriter tw, RootType root )
            => ImplWrite ( tw, root );


        protected virtual IConvertibleString ImplConverter { get => __converter; }

        protected abstract RootType ImplRead ( TextReader textReader );
        protected abstract void ImplWrite ( TextWriter tw, RootType root );
    } // End of Class SymbolTableAdapter<...>
} // End of Namespace Sharpframework.Serialization
