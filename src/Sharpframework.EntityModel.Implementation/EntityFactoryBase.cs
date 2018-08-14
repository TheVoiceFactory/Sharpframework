using System;

using Newtonsoft.Json;
using Sharpframework.Core;


namespace Sharpframework.EntityModel.Implementation
{
    public abstract class EntityFactoryBase<    TobjInterface,
                                                TobjContractInterface,
                                                TconcreteObject,
                                                InitDtoType,
                                                FactoryType>
        : ValueObjectFactoryBase <  TobjInterface,
                                    TobjContractInterface,
                                    TconcreteObject,
                                    InitDtoType,
                                    FactoryType>
    where TobjInterface : IEntity
    where InitDtoType   : new ()
    where FactoryType   : EntityFactoryBase<    TobjInterface,
                                                TobjContractInterface,
                                                TconcreteObject,
                                                InitDtoType,
                                                FactoryType>
                        , new ()
    {
        private static readonly JsonSerializerSettings  __DefaultJsonDeserializeSettings;
        private static readonly JsonSerializerSettings  __DefaultJsonSerializeSettings;

        private JsonSerializerSettings  __jsonDeserializeSettings;
        private JsonSerializerSettings  __jsonSerializeSettings;


        static EntityFactoryBase ()
        {
            __DefaultJsonDeserializeSettings    = new JsonSerializerSettings ();
            __DefaultJsonSerializeSettings      = new JsonSerializerSettings ();

            __DefaultJsonDeserializeSettings.TypeNameHandling           = TypeNameHandling.Auto;
            __DefaultJsonDeserializeSettings.MetadataPropertyHandling   = MetadataPropertyHandling.ReadAhead;
            __DefaultJsonSerializeSettings.ContractResolver             = new InterfaceContractResolver ();
            __DefaultJsonSerializeSettings.TypeNameHandling             = TypeNameHandling.All;
        } // End of Static Constructor

        public EntityFactoryBase ()
            : this ( __DefaultJsonSerializeSettings, __DefaultJsonDeserializeSettings ) { }

        public EntityFactoryBase (
                JsonSerializerSettings serializeSettings,
                JsonSerializerSettings deserializeSettings )
            : base ( serializeSettings, deserializeSettings )
        {
            _Reset ();

            __jsonDeserializeSettings   = deserializeSettings;
            __jsonSerializeSettings     = serializeSettings;
        } // End of Custom Constructor


        protected override IJsonContractDocument ImplExportToJson (
            TobjInterface   instance,
            String          version )
        {
            //var settings = new JsonSerializerSettings
            //{
            //    ContractResolver = new InterfaceContractResolver(typeof(TobjContractInterface)),
            //    TypeNameHandling = TypeNameHandling.All
            //};

            return new JsonDocument (   JsonConvert.SerializeObject ( instance, __jsonSerializeSettings ),
                                        version,
                                        instance.Guid );
        } // End of MyExportToJson (...)

        //protected override TobjInterface MyGetFromJsonString ( IJsonContractDocument json )
        //{
        //    var settings = new JsonSerializerSettings
        //    {
        //        // ContractResolver = new InterfaceContractResolver(typeof(objContractInterface)),
        //        TypeNameHandling = TypeNameHandling.Auto,
        //        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
        //    };

        //    object obj = JsonConvert.DeserializeObject<TconcreteObject> ( json.Json, settings );

        //    return (TobjInterface) obj;
        //} // End of MyGetFromJsonString (...)


        private void _Reset ()
        {
            __jsonDeserializeSettings   = null;
            __jsonSerializeSettings     = null;
        } // End of _Reset ()
    } // End of Class EntityFactoryBase2<...>
} // End of Namespace Sharpframework.Infrastructure.Data

//namespace Sharpframework.Infrastructure.Data
//{
//    using Sharpframework.Domains.Framework.New;
//    using Sharpframework.Infrastructure.Data.New;


//    public abstract class EntityFactoryBase<InterfaceType, InitDtoType, FactoryType>
//        : ValueObjectFactoryBase<InterfaceType, InitDtoType, FactoryType>
//    where InterfaceType : IEntity
//    where InitDtoType   : new ()
//    where FactoryType   : EntityFactoryBase<InterfaceType, InitDtoType, FactoryType>
//                        , new ()
//    {
//    } // End of Class EntityFactoryBase<...>
//} // End of Namespace Sharpframework.Infrastructure.Data
