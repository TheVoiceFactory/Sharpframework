using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;



namespace Sharpframework.EntityModel.Implementation
{
    public class ValueObjectContractResolver
        : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties (
            Type type,
            MemberSerialization memberSerialization )
        {
            Type serializationType = type;

            if ( typeof ( IValueObject ).IsAssignableFrom ( type ) )
                for ( Type subType = type.GetTypeInfo().BaseType ; subType != null ; subType = subType.GetTypeInfo().BaseType )
                    if ( !subType.GetTypeInfo().IsGenericType                         ) continue;
                    else if ( subType.GenericTypeArguments.Length != 1  ) continue;
                    else if ( subType.GetGenericTypeDefinition () == typeof ( ValueObject<> ) ) {
                        serializationType = subType.GenericTypeArguments [ 0 ]; break; }

            return base.CreateProperties ( serializationType, memberSerialization );
        } // End of CreateProperties (...)
    } // End of Class ValueObjectContractResolver
} // End of Namespace Sharpframework.Domains.Shared.Model
