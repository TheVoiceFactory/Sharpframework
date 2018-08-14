using System;

using Sharpframework.Serialization.ValueDom;


namespace Sharpframework.EntityModel.Implementation
{
    public abstract class Entity
        : CompositedIdObject
        , IEntity
        , IEntitySerializationContract
    {
        protected Entity () { _Reset (); Version = "1.0"; }

        protected Entity ( IUid uid )
        {
            _Reset ();

            Guid    = uid.Guid;
            Version = uid.Version;
        } // End of Custom Constructor

        protected Entity (  IValueUnit                                  valueUnit,
                            IHierarchicalMetadataSerializationContext   context )
            : base ( valueUnit, context ) { }


        // Serve per la Deserializzazione json
        //TODO deve essere ottimizzata attraverso una statica e un check
        public String EntityName
        {
            get
            {
                if ( String.IsNullOrWhiteSpace ( ImplEntityName ) )
                    ImplEntityName = GetType ().Name;

                return ImplEntityName;
            }

            set { ImplEntityName = value; }
        }

        public Guid Guid { get { return ImplGuid; } set { ImplGuid = value; } }

        public String Version { get; set; }


        protected virtual String ImplEntityName { get; set; }

        protected Guid ImplGuid { get; set; }


        protected override void ImplDispose ( Boolean disposing )
        {
            try
            {
                _Reset ();
            }
            finally { base.ImplDispose ( disposing ); }
        } // End of ImplDispose (...)


        private void _Reset ()
        {
            ImplGuid = Guid.Empty;
        } // End of _Reset ()


        Guid IGuidProvider.Guid { get { return ImplGuid; } }
    } // End of Class Entity
} // End of Namespace Sharpframework.EntityModel.Implementation
