using Newtonsoft.Json;

//using Sharpframework.Core;


namespace Sharpframework.EntityModel.Implementation
{
    public abstract class ValueObjectFactory<   InterfaceType,
                                                ContractType,
                                                ObjType,
                                                InitDtoType,
                                                FactoryType>
        : ValueObjectFactoryBase<   InterfaceType,
                                    ContractType,
                                    ObjType,
                                    InitDtoType,
                                    FactoryType>
    where InterfaceType : IValueObject
    where ContractType  : IValueObjectContract
    where ObjType       : ValueObject<ContractType>
                        , InterfaceType
                        , new ()
    where InitDtoType   : new ()
    where FactoryType   : ValueObjectFactory<   InterfaceType,
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


        static ValueObjectFactory ()
        {
            __DefaultJsonDeserializeSettings    = new JsonSerializerSettings ();
            __DefaultJsonSerializeSettings      = new JsonSerializerSettings ();

            __DefaultJsonDeserializeSettings.TypeNameHandling   = TypeNameHandling.Auto;
            __DefaultJsonSerializeSettings.ContractResolver     = new ValueObjectContractResolver ();
            __DefaultJsonSerializeSettings.TypeNameHandling     = TypeNameHandling.Auto;
        } // End of Static Constructor

        public ValueObjectFactory ()
            : this ( __DefaultJsonSerializeSettings, __DefaultJsonDeserializeSettings ) { }

        public ValueObjectFactory (
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
    } // End of Class ValueObjectFactory<...>


    public abstract class ValueObjectFactory<   InterfaceType,
                                                ContractType,
                                                ObjType,
                                                FactoryType>
        : ValueObjectFactory<   InterfaceType,
                                ContractType,
                                ObjType,
                                ObjType,
                                FactoryType>
    where InterfaceType : IValueObject
    where ContractType  : IValueObjectContract
    where ObjType       : ValueObject<ContractType>
                        , InterfaceType
                        , new ()
    where FactoryType   : ValueObjectFactory<   InterfaceType,
                                                ContractType,
                                                ObjType,
                                                FactoryType>
                        , new ()
    {
        protected override sealed void ImplClearInitDto ()
        { } // Do nothing

        protected override InterfaceType ImplGetNew ( ObjType initDto )
        { try { return initDto; } finally { InitDto = new ObjType (); } }
    } // End of ValueObjectFactoryBase<...>
} // End of Namespace Sharpframework.Domains.Shared.Model

//namespace Sharpframework.Infrastructure.Data
//{
//    using System;
//    using System.Collections;
//    using System.ComponentModel;

//    using Serialization;
//    using ValueDom;
//    using Domains.Framework.New;
//    using Sharpframework.Implementations.Domains.Shared.Model.New;


//    public abstract class ValueObjectFactory<InterfaceType, InitDtoType, FactoryType>
//        : SingletonObject<FactoryType>
//    where InterfaceType : IValueObject
//    where InitDtoType   : new ()
//    where FactoryType   : ValueObjectFactoryBase<InterfaceType, InitDtoType, FactoryType>
//                        , new ()
//    {
//        public abstract class AbsValueObjectInitDto
//        {
//            private ValueObject __core;

//            private ValueObject _Core
//            {
//                get
//                {
//                    if ( __core == null )
//                    {
//                        __core = ImplCore as ValueObject;
//                        // Init Default Values...
//                    }

//                    return __core;
//                }
//            }

//            public void Clear () => ImplClear ();

//            protected virtual void ImplClear () { __core = null; }

//            protected abstract InterfaceType ImplCore { get; }
//        } // End of Class AbsValueObjectInitDto


//        private InitDtoType __initDto;


//        protected virtual InitDtoType ImplInitDto
//        {
//            get
//            {
//                if ( __initDto == null ) __initDto = new InitDtoType ();

//                return __initDto;
//            }
//        }
//    }
//}
