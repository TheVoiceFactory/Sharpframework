using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sharpframework.Propagation
{
    public interface IRequest: ITopicMessageContract   { string Correlation { get; } }
    public interface IRequest<ClassType, PayloadType>: ITopicMessage<ClassType , PayloadType >, IRequest
    {
       new string Correlation { get; }

    }

}