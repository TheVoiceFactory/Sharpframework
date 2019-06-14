using System;


namespace Sharpframework.EntityModel.Implementation
{
    public class UId
        : IUid
        , IEquatable<IUid>
    {
        public UId () : this ( Guid.NewGuid (), "1.0" ) { }

        public UId ( Guid guid ) : this ( guid, "1.0" ) { }

        public UId ( IUid uid ) : this ( uid.Guid, uid.Version ) { }

        public UId ( Guid guid, String version )
        {
            Guid    = guid;
            Version = version;
        }

        public String Version { get; private set; }

        public Guid Guid { get; private set; }


        public override int GetHashCode () => base.GetHashCode ();

        public override Boolean Equals ( Object obj )
        {
            IUid other = obj as IUid;

            return other == null ? base.Equals ( obj ) : Equals ( other );
        }

        public Boolean Equals ( IUid other )
            => other != null && Guid == other.Guid && Version == other.Version;
    }
}
