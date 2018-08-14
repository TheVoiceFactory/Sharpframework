using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Sharpframework.Core.Config
{

   public  class  ConfigObject<objectType> : IConfigObject<objectType> where objectType : IConfigObject<objectType>, new()
    {
        public ConfigObject()
        { }
        public virtual objectType GetDefault() { return new objectType() ; }// return Activator.CreateInstance<objectType>(); }

        public void Store()
        {
            Configurator<objectType>.StoreConfig(this);
        }
    }

}