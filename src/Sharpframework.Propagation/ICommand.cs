using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sharpframework.Propagation
{

    public interface ICommand:ITopicMessageContract

    {
        string CommandName { get; }
    }
    public interface ICommand<ClassType, PayloadType> : ICommand
    {

    }


}