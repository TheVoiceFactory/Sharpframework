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


        public ExecEntityVerbFact ( Object sender ) : base ( sender ) { }

        public ExecEntityVerbFact ( Object sender, ExecDescr execDescr )
            : base ( sender ) => this.execDescr = execDescr;

        public ExecEntityVerbFact ( Object sender, IUid uid )
            : this ( sender, uid, null ) { }

        public ExecEntityVerbFact ( Object sender, IUid uid, ExecDescr execDescr )
             : base ( sender, execDescr ) => this.uid = new UId ( uid );


        Guid IGuidProvider.Guid => uid == null ? Guid.Empty : uid.Guid;

        String IVersionProvider.Version => uid == null ? String.Empty : uid.Version;


        protected override bool ImplSkip ( TargetType target )
            => uid == null || !uid.Equals ( target ) || base.ImplSkip ( target );
    }
}
