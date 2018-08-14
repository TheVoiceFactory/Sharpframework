using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace Sharpframework.Core
{
    public class InterfaceContractResolver
        : DefaultContractResolver
    {
        //private readonly Type[] _interfaceTypes;
        private readonly List<Type> _interfaceTypes;
        private readonly List<Type> __skippedTypes;

        private readonly ConcurrentDictionary<Type, Type> _typeToSerializeMap;

        public InterfaceContractResolver(params Type[] interfaceTypes)
        {
            //_interfaceTypes = interfaceTypes;
            _interfaceTypes = new List<Type> ( interfaceTypes );
            __skippedTypes = new List<Type> ();


            _typeToSerializeMap = new ConcurrentDictionary<Type, Type>();
        }

        protected override IList<JsonProperty> CreateProperties (
            Type type,
            MemberSerialization memberSerialization )
        {
            var typeToSerialize = _typeToSerializeMap.GetOrAdd(
                type,
                t => _interfaceTypes.FirstOrDefault(
                    it => it.IsAssignableFrom(t)) ?? t);

            List<Type>          processing  = null;
            IList<JsonProperty> retVal      = null;

            processing  = new List<Type> ();
            retVal      = base.CreateProperties ( typeToSerialize, memberSerialization );

            foreach ( JsonProperty jsonProp in retVal )
                _ComputeInterface ( jsonProp.PropertyType, processing );

            return retVal;
        } // End of CreateProperties (...)

        private void _ComputeGenericInterface ( Type t, List<Type> processing )
        {
            processing.Add ( t );

            try
            {
                foreach ( Type subInterface in t.GetInterfaces () )
                    try
                    {
                        if      ( !_IsGoodInterface ( subInterface )    ) continue;
                        else if ( !subInterface.GetTypeInfo().IsGenericType           ) continue;
                        else
                        {
                            processing.Add ( subInterface );

                            foreach ( Type typeParam in subInterface.GetGenericArguments () )
                                _ComputeInterface ( typeParam, processing );
                        }
                    }
                    finally
                    {
                        if ( !__skippedTypes.Contains ( subInterface ) )
                            if ( processing.Contains ( subInterface ) )
                            {
                                processing.Remove ( subInterface );
                                __skippedTypes.Add ( subInterface );
                            }
                            else
                            {
                                _ComputeGenericInterface ( subInterface, processing );

                                if ( !_interfaceTypes.Contains ( subInterface ) )
                                    if ( !__skippedTypes.Contains ( subInterface ) )
                                        __skippedTypes.Add ( subInterface );
                            }
                    }
            }
            finally { processing.Remove ( t ); }
        } // End of _ComputeGenericInterface (...)

        private void _ComputeInterface ( Type t, List<Type> processing )
        {
            if ( __skippedTypes.Contains ( t )  ) return;
            if ( _interfaceTypes.Contains ( t ) ) return;
            if ( processing.Contains ( t )      ) return;

            processing.Add ( t );

            if ( _IsGoodInterface ( t ) ) _interfaceTypes.Add ( t );

            _ComputeGenericInterface ( t, processing );

            processing.Remove ( t );
        } // End of _ComputeInterface (...)

        private Boolean _IsGoodInterface ( Type t )
        {
            if      ( __skippedTypes.Contains ( t )     ) return false;
            else if ( !t.GetTypeInfo().IsInterface                    ) { }
            else if ( _interfaceTypes.Contains ( t )    ) return false;
            else
                return true;

            __skippedTypes.Add ( t );

            return false;
        } // End of _IsGoodInterface (...)
    } // End of Class InterfaceContractResolver
} // End of Namespace Sharpframework.Core
