using System;
using Sharpframework.EntityModel;
using Sharpframework.EntityModel.Implementation;


namespace Sharpframework.Propagation.Facts
{
    public class ExecEntityVerbFact<TargetType>
        : ExecVerbFact<TargetType>
        , IExecEntityVerbFact<TargetType>
    where TargetType : IUid
    {
        public UId uid;


        public ExecEntityVerbFact () : base () { }

        public ExecEntityVerbFact ( ExecDescr execDescr )
            => this.execDescr = execDescr;

        public ExecEntityVerbFact ( IUid uid ) : this ( null, uid ) { }

        public ExecEntityVerbFact ( ExecDescr execDescr, IUid uid )
             : base ( execDescr )
                => this.uid = new UId ( uid );


        Guid IGuidProvider.Guid => uid == null ? Guid.Empty : uid.Guid;

        String IVersionProvider.Version => uid == null ? String.Empty : uid.Version;


        protected override bool ImplSkip ( TargetType target )
            => uid == null || !uid.Equals ( target ) || base.ImplSkip ( target );
    }
}
