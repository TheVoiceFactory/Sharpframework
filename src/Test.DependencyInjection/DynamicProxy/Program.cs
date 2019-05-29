using System;
using System.Collections.Generic;
using System.Text;

namespace Test.DependencyInjection.DynamicProxy
{
    public interface IPippo
    {
        Int32 Pluto ( String str );
        String Description { get; set; }
    }
    public class Pippo : IPippo
    {
        public Pippo () { }
        public Pippo ( IServiceProvider sp ) { }
        public string Description { get => throw new NotImplementedException (); set => throw new NotImplementedException (); }

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
