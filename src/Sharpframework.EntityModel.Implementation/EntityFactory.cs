using System;

using Newtonsoft.Json;



namespace Sharpframework.EntityModel.Implementation
{
    public abstract class EntityFactory<    InterfaceType,
                                            ContractType,
                                            ObjType,
                                            InitDtoType,
                                            FactoryType>
        : EntityFactoryBase<    InterfaceType,
                                ContractType,
                                ObjType,
                                InitDtoType,
                                FactoryType>
    where InterfaceType : IEntity
    where ContractType  : IEntityContract
    where ObjType       : Entity<ContractType>
                        , InterfaceType
                        , new ()
    where InitDtoType   : new ()
    where FactoryType   : EntityFactory<    InterfaceType,
                                            ContractType,
                                            ObjType,
                                            InitDtoType,
                                            FactoryType>
                        , new ()
    {
        private static readonly JsonSerializerSettings  __DefaultJsonDeserializeSettings;
        private static readonly JsonSerializerSettings  __DefaultJsonSerializeSettings;

        private JsonSerializerSettings  __jsonDeserializeSettings;
        private JsonSerializerSettings  __jsonSerializeSettings;


        static EntityFactory ()
        {
            __DefaultJsonDeserializeSettings    = new JsonSerializerSettings ();
            __DefaultJsonSerializeSettings      = new JsonSerializerSettings ();

            __DefaultJsonDeserializeSettings.TypeNameHandling           = TypeNameHandling.Auto;
            __DefaultJsonDeserializeSettings.MetadataPropertyHandling   = MetadataPropertyHandling.ReadAhead;
            __DefaultJsonSerializeSettings.ContractResolver             = new ValueObjectContractResolver ();
            __DefaultJsonSerializeSettings.TypeNameHandling             = TypeNameHandling.All;
        } // End of Static Constructor

        public EntityFactory ()
            : this ( __DefaultJsonSerializeSettings, __DefaultJsonDeserializeSettings ) { }

        public EntityFactory (
                JsonSerializerSettings serializeSettings,
                JsonSerializerSettings deserializeSettings )
            : base ( serializeSettings, deserializeSettings )
        {
            _Reset ();

            __jsonDeserializeSettings   = deserializeSettings;
            __jsonSerializeSettings     = serializeSettings;
        } // End of Custom Constructor


        private void _Reset ()
        {
            __jsonDeserializeSettings   = null;
            __jsonSerializeSettings     = null;
        } // End of _Reset ()
    } // End of Class EntityFactory<...>


    public abstract class EntityFactory<    InterfaceType,
                                            ContractType,
                                            ObjType,
                                            FactoryType>
        : EntityFactory<    InterfaceType,
                            ContractType,
                            ObjType,
                            ObjType,
                            FactoryType>
    where InterfaceType : IEntity
    where ContractType  : IEntityContract
    where ObjType       : Entity<ContractType>
                        , InterfaceType
                        , new ()
    where FactoryType   : EntityFactory<    InterfaceType,
                                            ContractType,
                                            ObjType,
                                            FactoryType>
                        , new ()
    {
        protected override sealed void ImplClearInitDto ()
        { } // Do nothing

        protected override InterfaceType ImplGetNew ( ObjType initDto )
        {
            try
            {
                if ( initDto.Guid == Guid.Empty ) initDto.Guid = Guid.NewGuid ();

                return initDto;
            }
            finally { InitDto = new ObjType (); }
        } // End of MyGetNew (...)
    } // End of EntityFactory<...>


    public abstract class EntityFactoryElem<InterfaceType,
                                            ContractType,
                                            ObjType,
                                            InitDtoType,
                                            FactoryType>
        : EntityFactoryBase<InterfaceType,
                                ContractType,
                                ObjType,
                                InitDtoType,
                                FactoryType>
    where InterfaceType : IEntity
    where ContractType : IEntityContract
    where ObjType : Entity
                        , InterfaceType
                        , new()
    where InitDtoType : new()
    where FactoryType : EntityFactoryElem<InterfaceType,
                                            ContractType,
                                            ObjType,
                                            InitDtoType,
                                            FactoryType>
                        , new()
    {
        private static readonly JsonSerializerSettings __DefaultJsonDeserializeSettings;
        private static readonly JsonSerializerSettings __DefaultJsonSerializeSettings;

        private JsonSerializerSettings __jsonDeserializeSettings;
        private JsonSerializerSettings __jsonSerializeSettings;


        static EntityFactoryElem()
        {
            __DefaultJsonDeserializeSettings = new JsonSerializerSettings();
            __DefaultJsonSerializeSettings = new JsonSerializerSettings();

            __DefaultJsonDeserializeSettings.TypeNameHandling = TypeNameHandling.Auto;
            __DefaultJsonDeserializeSettings.MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead;
            __DefaultJsonSerializeSettings.ContractResolver = new ValueObjectContractResolver();
            __DefaultJsonSerializeSettings.TypeNameHandling = TypeNameHandling.All;
        } // End of Static Constructor

        public EntityFactoryElem()
            : this(__DefaultJsonSerializeSettings, __DefaultJsonDeserializeSettings) { }

        public EntityFactoryElem(
                JsonSerializerSettings serializeSettings,
                JsonSerializerSettings deserializeSettings)
            : base(serializeSettings, deserializeSettings)
        {
            _Reset();

            __jsonDeserializeSettings = deserializeSettings;
            __jsonSerializeSettings = serializeSettings;
        } // End of Custom Constructor


        private void _Reset()
        {
            __jsonDeserializeSettings = null;
            __jsonSerializeSettings = null;
        } // End of _Reset ()
    } // End of Class EntityFactory<...>


    public abstract class EntityFactoryElem<InterfaceType,
                                            ContractType,
                                            ObjType,
                                            FactoryType>
        : EntityFactoryElem<InterfaceType,
                            ContractType,
                            ObjType,
                            ObjType,
                            FactoryType>
    where InterfaceType : IEntity
    where ContractType : IEntityContract
    where ObjType : Entity
                        , InterfaceType
                        , new()
    where FactoryType : EntityFactoryElem<InterfaceType,
                                            ContractType,
                                            ObjType,
                                            FactoryType>
                        , new()
    {
        protected override sealed void ImplClearInitDto()
        { } // Do nothing

        protected override InterfaceType ImplGetNew(ObjType initDto)
        {
            try
            {
                if (initDto.Guid == Guid.Empty) initDto.Guid = Guid.NewGuid();

                return initDto;
            }
            finally { InitDto = new ObjType(); }
        } // End of MyGetNew (...)
    } // End of EntityFactory<...>
} // End of Namespace Sharpframework.EntityModel.Implementation

