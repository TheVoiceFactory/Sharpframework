using System;
using System.Collections.Generic;
using System.Text;

using Sharpframework.Roslyn.DynamicProxy;


namespace Test.DependencyInjection.DynamicProxy
{
    [EpilogInterceptor ( typeof ( ExternalMethods ), nameof (ExternalMethods.Epilog )) ]
    [PrologInterceptor ( typeof ( ExternalMethods ), nameof (ExternalMethods.Prolog )) ]
    public interface IPippo
    {
        void Paperino ( Int32 num, String str );

        Int32 Pluto ( String str );

        String Description { get; set; }
    }

    public class Pippo : IPippo
    {
        public Pippo () { }
        public Pippo ( IServiceProvider sp ) { }
        public String Description { get; set; }

        public void Paperino ( Int32 num, String str )
        {
            Console.WriteLine ( "Called method Pippo.Paperino (...)" );
        }

        public int Pluto ( string str )
        {
            return str == null ? 0 : str.Length;
        }
    }
    class Program
    {
        static void Main ( string [] args )
        {
            ProxyServiceContainer psc = new ProxyServiceContainer ();

            psc.AddService<IPippo, Pippo> ();

            psc.GetService ( typeof ( IPippo ) );

            Console.ReadKey ();

            return;
        }
    }
}
