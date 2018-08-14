using System;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Sharpframework.Core.Config
{
    public static class Configurator<configObjectType> where configObjectType : IConfigObject<configObjectType>//, new()
    {

        private static Logger log = LogManager.GetCurrentClassLogger();

        private static string GetName()
        {
            string directory = AppContext.BaseDirectory;
            string fileContent = string.Empty;
            string finalPath = string.Empty;
   
            try
            {
                String nameOverride = Path.Combine(directory, @"global.name"); 
                log.Debug("Looking for name: " + nameOverride);
                fileContent = File.ReadAllText(nameOverride);
            }
            catch (Exception ex)
            {
                log.Debug("ERROR global.name file: " + ex.Message + " " + fileContent);
            }
            string fullName = Assembly.GetEntryAssembly().CodeBase ;
            string myName = Path.GetFileNameWithoutExtension(fullName);
          
            string fileName = !string.IsNullOrEmpty(fileContent) ? 
                            fileContent + "." + typeof(configObjectType).Name :
                         myName  + "." + typeof(configObjectType).Name;

            finalPath = Path.Combine(directory, fileName + ".json");

            try
            {
                fileContent = File.ReadAllText(finalPath + @".redirector");
            }
            catch (Exception ex)
            {
                log.Debug("ERROR Config file: " + ex.Message + "  " + fileContent);
            }
            try
            {
                string s = Path.Combine(directory, @"global.redirector");
                log.Info("Looking for: " + s);
                fileContent = System.IO.File.ReadAllText(s);
            }
            catch (Exception ex)
            {
                log.Debug("ERROR Config file: " + ex.Message + "  " + fileContent);

            }

            if (!String.IsNullOrWhiteSpace(fileContent))
                finalPath = Path.Combine(fileContent, ( fileName + ".json" ) );
            log.Info("Config file: " + finalPath);

            return finalPath;
        }

        //PC: herit form DefaultContractResolver : https://stackoverflow.com/questions/24106986/json-net-force-serialization-of-all-private-fields-and-all-fields-in-sub-classe
        private class ConfiguratorContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var props = type.GetProperties( BindingFlags.NonPublic )
                                .Select( p => base.CreateProperty(p, memberSerialization) )
                                .Union( type.GetFields(BindingFlags.NonPublic )
                                .Select( f => base.CreateProperty(f, memberSerialization)) )
                                .ToList();
                props.ForEach(p => { p.Writable = true; p.Readable = true; });
                return props;
            }
        }

        public static configObjectType Config
        {
            get
            {
                var file = GetName();
                configObjectType ret;
                if (System.IO.File.Exists(file))
                {
                    using (StringReader reader = new StringReader(File.ReadAllText(file)))
                    {
                        var contractResolver = new ConfiguratorContractResolver();
                        //contractResolver.DefaultMembersSearchFlags |= BindingFlags.NonPublic;
                        var settings = new JsonSerializerSettings
                        {
                            ContractResolver = contractResolver
                        };
                        string json = reader.ReadToEnd();
                        ret = JsonConvert.DeserializeObject<configObjectType>(json);
                    }
                }
                else
                {
                    ret = Activator.CreateInstance<configObjectType>().GetDefault();
                    //  ret = new configObjectType().GetDefault();// Activator.CreateInstance<configObjectType>().GetDefault();
                    if (ret == null) { ret = Activator.CreateInstance<configObjectType>(); }// new configObjectType(); }//assume default declared in line on properties
                    ret.Store();
                }

                return ret;
            }
        }

        public static void StoreConfig(IConfigObject<configObjectType> toSave)
        {
            string ret;
            ret = JsonConvert.SerializeObject(toSave);

            //PC: https://stackoverflow.com/questions/27735447/vnext-argument-1-cannot-convert-from-string-to-system-io-stream
            using (StreamWriter writer = new StreamWriter(File.OpenWrite(GetName())))
            {
                writer.Write(ret);
                writer.Flush();
                //writer.Close(); Implementing IDisposable means that you can use a using statement, which will implicitly call the Dispose() method, thus closing the stream. 
            }
        }

        public static void Delete()
        {
            if (File.Exists(GetName()))
            {
                File.Delete(GetName());
            }

        }
    }
}