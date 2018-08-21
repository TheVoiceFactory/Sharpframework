using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sharpframework.Propagation
{
    public interface IResponse : ITopicMessageContract { string Correlation { get; } }
    public interface IResponse<ClassType, PayloadType> :IResponse , ITopicMessage<ClassType, PayloadType>
    {

    }

}