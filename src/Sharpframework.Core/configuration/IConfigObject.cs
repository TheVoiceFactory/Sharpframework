using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Sharpframework.Core.Config
{

    public interface IConfigObject<objectType> where objectType :IConfigObject <objectType>
    {
        objectType GetDefault();
        void Store();
        
    }
}