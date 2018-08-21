using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Reflection;

namespace Sharpframework.Propagation
{
    public abstract class TopicMessage<ClassType, PayloadType> : ITopicMessage<ClassType, PayloadType>
    {
        public TopicMessage()
        {
        }

        private static string SerializeMsg(ITopicMessage<ClassType, PayloadType> payload)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            //   ContractResolver = new InterfaceContractResolver(typeof(TobjContractInterface)),

            var res = JsonConvert.SerializeObject(payload, settings);
            return res;
        }

        public static ClassType DeserializeMsg(string str)
        {
            var settings = new JsonSerializerSettings();
            //   ContractResolver = new InterfaceContractResolver(typeof(TobjContractInterface)),
            settings.TypeNameHandling = TypeNameHandling.Auto;

            settings.ObjectCreationHandling = ObjectCreationHandling.Replace; //CARLOADD

            var res = JsonConvert.DeserializeObject(str, settings);
            return (ClassType)res;
        }

        virtual public string Name { get { return this.GetType().Name; } }
        public IDictionary<string, string> UserHeaderProperties { get; set; } = new Dictionary<string, string>();
        private Dictionary<string, string> _hp;
        virtual protected Dictionary<string, string> HeaderProperties
        {
            get
            {
                _hp = new Dictionary<string, string>(); ;
                _hp.Add("Name", this.Name);
                _hp.Add("FullName", this.FullName);
                _hp.Add("Family", this.Family);
                _hp.Add("Origin", this.Origin );

                return this._hp;//OKKIO se ritorni la proprieta' va in loop !!!
            }
        }

        IReadOnlyDictionary<string, string> ITopicMessageContract.HeaderProperties
        {
            get
            {
                return (IReadOnlyDictionary<string, string>)HeaderProperties;
            }
        }

        public PayloadType Payload { get; set; }

        public object PayloadObject
        {
            get { return Payload; }
        }

        public string FullName { get { return this.GetType().FullName; } }

        public string Family { get { return this.GetType().GetTypeInfo().BaseType.Name ; } }

        public string Origin { get; set; } = "";

        virtual public ClassType Deserialize(string str)
        {
            return DeserializeMsg(str);
        }

        virtual public string Serialize()
        {
            return SerializeMsg(this);
        }
    }
    public class GenericStringMessage : TopicMessage<GenericStringMessage, string>
    { public GenericStringMessage(string message)
        {
            this.Payload = message;
        }

    }
}