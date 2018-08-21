using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpframework.Propagation
{
    public abstract class Command<ClassType, PayloadType> : TopicMessage<ClassType, PayloadType>, ICommand <ClassType, PayloadType>, ICommand
    {

        public virtual string CommandName { get {return this.Name; } }

        protected override Dictionary<string, string> HeaderProperties
        {
            get
            {
                var prop = base.HeaderProperties;
                prop.Add("CommandName", CommandName);


                return prop;
            }
        }

    }

}
