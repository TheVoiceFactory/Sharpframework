using System;
using System.Collections.Generic;

namespace Sharpframework.Propagation
{
    public abstract class Request<ClassType, PayloadType> : TopicMessage<ClassType, PayloadType>, IRequest<ClassType, PayloadType>, IRequest
    {
        public Request() { }

        private string _correlation;

        public virtual string Correlation
        {
            get
            {
                if (_correlation == null) { _correlation = Guid.NewGuid().ToString(); }
                return _correlation;
            }

            set { _correlation = value; }
        }

        protected override Dictionary<string, string> HeaderProperties
        {
            get
            {
                var prop = base.HeaderProperties;
                prop.Add("Correlation", Correlation);
                return prop;
            }
        }
    }
}