using System;
using System.Collections.Generic;
using Sharpframework.Propagation;

namespace Sharpframework.Propagation
{
    public abstract class SystemCommand<ClassType, PayloadType> : TopicMessage<ClassType, PayloadType>, ISystemCommand<ClassType, PayloadType>, ISystemCommand
    {
        public virtual string CommandName { get { return this.Name; } }
        public virtual string TargetID { get; set; }

        protected override Dictionary<string, string> HeaderProperties
        {
            get
            {
                var prop = base.HeaderProperties;
                prop.Add("CommandName", CommandName);
                prop.Add("TargetID", TargetID.ToString());

                return prop;
            }
        }
    }
}