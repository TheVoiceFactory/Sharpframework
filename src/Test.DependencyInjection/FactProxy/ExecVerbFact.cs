﻿using System;


namespace Sharpframework.Propagation.Facts
{
    public class ExecVerbFact<TargetType>
        : Fact
        , IExecVerbFact<TargetType>
    {
        public ExecDescr execDescr;

        public abstract class ExecDescr
        {
            public Boolean Exec ( TargetType target )
                => target != null && ImplExec ( target );


            protected abstract Boolean ImplExec ( TargetType target );
        }


        public ExecVerbFact () : this ( null ) { }

        public ExecVerbFact ( ExecDescr execDescr )
            => this.execDescr = execDescr;


        public Boolean Exec ( TargetType target )
        {
            if ( target     == null ) return false;
            if ( execDescr  == null ) return false;

            return ImplSkip ( target ) || execDescr.Exec ( target );
        }
        //public ExecVerbFact ( IServiceProvider sp ) : base ( sp ) { }


        protected virtual Boolean ImplSkip ( TargetType target ) => false;
    }
}
