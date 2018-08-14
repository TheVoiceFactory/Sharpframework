using System;
//using Sharpframework.EntityModel;



namespace Sharpframework.EntityModel.Implementation
{
    public abstract class EntityBase<SerializationContractType>
        : CompositedIdObjectBase<SerializationContractType>
        , IEntity
    where SerializationContractType : IEntityContract
    {
        protected EntityBase () { _Reset (); Version = "1.0"; }

        protected EntityBase ( IUid uid )
        {
            _Reset ();

            Guid    = uid.Guid;
            Version = uid.Version;
        } // End of Custom Constructor


        // Serve per la Deserializzazione json
        //TODO deve essere ottimizzata attraverso una statica e un check
        public String EntityName
        {
            get
            {
                if ( String.IsNullOrWhiteSpace ( MyEntityName ) )
                    MyEntityName = GetType ().Name;

                return MyEntityName;
            }

            set { MyEntityName = value; }
        }

        public Guid Guid { get { return MyGuid; } set { MyGuid = value; } }

        public String Version { get; set; }


        protected virtual String MyEntityName { get; set; }

        protected Guid MyGuid { get; set; }


        protected override void ImplDispose(Boolean disposing)
        {
            try
            {
                _Reset();
            }
            finally { base.ImplDispose(disposing); }
        } // End of ImplDispose (...)


        private void _Reset()
        {
            MyGuid = Guid.Empty;
        } // End of _Reset ()


        Guid IGuidProvider.Guid { get { return MyGuid; } }
    } // End of Class EntityBase<...>


    public abstract class Entity<SerializationContractType>
        : EntityBase<SerializationContractType>
    where SerializationContractType : IEntityContract
    {
        protected Entity () { }

        protected Entity ( IUid uid ) : base ( uid ) { }


        protected override Type ImplContractType => typeof ( SerializationContractType );
    } // End of Class Entity<...>


    public abstract class Entity
        : EntityBase<IEntityContract>
    {
        protected Entity () { }

        protected Entity ( IUid uid ) : base ( uid ) { }
    } // End of Class Entity

    //public abstract class Entity
    //  : EntityUntyped
    //{ }

    //public abstract class Entity<SerializationContractType>
    //   : EntityUntyped

    //      where SerializationContractType : IEntityContract
    //{
    //    protected override Type ImplContractType
    //    { get { return typeof(SerializationContractType); } }


    //    protected Entity() { base._Reset(); Version = "1.0"; }

    //    protected Entity(IUid uid)
    //    {
    //        _Reset();

    //        Guid = uid.Guid;
    //        Version = uid.Version;
    //    } // End of Custom Constructor

    //}

    //public abstract class EntityUntyped
    //    : CompositedIdObjectUntyped 
    //    , IEntity

    //{
    //    protected EntityUntyped() { _Reset (); Version = "1.0"; }

    //    protected EntityUntyped( IUid uid )
    //    {
    //        _Reset ();

    //        Guid    = uid.Guid;
    //        Version = uid.Version;
    //    } // End of Custom Constructor


    //    // Serve per la Deserializzazione json
    //    //TODO deve essere ottimizzata attraverso una statica e un check
    //    public String EntityName
    //    {
    //        get
    //        {
    //            if ( String.IsNullOrWhiteSpace ( MyEntityName ) )
    //                MyEntityName = GetType ().Name;

    //            return MyEntityName;
    //        }

    //        set { MyEntityName = value; }
    //    }

    //    public Guid Guid { get { return MyGuid; } set { MyGuid = value; } }

    //    public String Version { get; set; }


    //    protected virtual String MyEntityName { get; set; }

    //    protected Guid MyGuid { get; set; }


    //    protected override void ImplDispose(Boolean disposing)
    //    {
    //        try
    //        {
    //            _Reset();
    //        }
    //        finally { base.ImplDispose(disposing); }
    //    } // End of ImplDispose (...)


    //    protected virtual void _Reset()
    //    {
    //        MyGuid = Guid.Empty;
    //    } // End of _Reset ()


    //    Guid IGuidProvider.Guid { get { return MyGuid; } }
    //} // End of Class Entity
    //public abstract class Entity
    //  : EntityUntyped
    //  , IEntity
    //{ }


} // End of Namespace Sharpframework.EntityModel.Implementation
