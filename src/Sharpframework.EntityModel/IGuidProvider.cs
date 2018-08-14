using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpframework.EntityModel

{
    public interface IGuidProvider
    {
        Guid Guid { get; }
    }
}
