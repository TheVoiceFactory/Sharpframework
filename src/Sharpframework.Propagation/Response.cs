using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpframework.Propagation
{
    public abstract  class Response<ClassType, PayloadType> : TopicMessage<ClassType, PayloadType>, IResponse<ClassType, PayloadType>, IResponse where ClassType : Response<ClassType, PayloadType>
    {
        public Response(PayloadType payload, IRequest req)
        {
            this.Payload = payload;
            this.Correlation = req.Correlation;
        }
        public Response() { }
        public string Correlation { get; set; } = "";

        protected override Dictionary<string, string> HeaderProperties
        {
            get
            {
                var prop = base.HeaderProperties;
                prop.Add("Correlation", Correlation);
                return prop;

            }
        }




        //public  Response<ClassType, PayloadType> RespondTo(IRequest request)
        //{
        //    ClassType o = (ClassType)Activator.CreateInstance(typeof(ClassType));
        //    ((ClassType)o).Correlation = request.Correlation;
        //    ((ClassType)o).Payload = _pay;
        //    _pay = default(PayloadType );
        //    return this;
        //}
        //private static PayloadType _pay;
        //public Response<ClassType, PayloadType> With(PayloadType payload)
        //{
        //    _pay = payload;
        //    return this;
        //}
    }

}
