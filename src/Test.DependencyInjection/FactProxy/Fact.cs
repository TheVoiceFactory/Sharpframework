using System;


namespace Sharpframework.Propagation.Facts
{
    public class Fact : IFact
    {
        protected readonly Object ImplSender;


        public Fact ( Object sender ) => ImplSender = sender;
        //public readonly IServiceProvider Sp;


        //public Fact ( IServiceProvider sp ) { Sp = sp; }

    }
}
