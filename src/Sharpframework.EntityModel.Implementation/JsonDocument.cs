using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


using System.Collections.Concurrent;
using System.Text;
using Sharpframework.EntityModel;

namespace Sharpframework.EntityModel.Implementation
{

    public class JsonDocument : IJsonContractDocument
    {
        public JsonDocument(string jsonP, string versionP, Guid guid)
        { Json = jsonP; Version = versionP; Guid = guid; }
        public JsonDocument(string jsonP, string versionP)
        { Json = jsonP; Version = versionP; }
        public Guid Guid { get; private set; }

        public string Json { get; private set; }
        public string Version { get; private set; }
    }

}