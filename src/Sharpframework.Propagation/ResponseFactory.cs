using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sharpframework.Propagation
{

    public class ResponseFactory<ClassType, PayloadType> where ClassType : Response<ClassType, PayloadType>, new()
    {
        private IRequest _request;
        private PayloadType _payload;

        public ResponseFactory<ClassType, PayloadType> RespondTo(IRequest request)
        {
            this._request = request;
            return this;
        }
        public ResponseFactory<ClassType, PayloadType>  WithPayload(PayloadType payload)
        {
            this._payload = payload;
            return this;
        }
        public static implicit operator ClassType (ResponseFactory<ClassType, PayloadType> rb)
        {
            // ClassType o = (ClassType)Activator.CreateInstance(typeof(ClassType),rb._payload, rb._request );
            ClassType o = new ClassType();
            
           o.Correlation = rb._request.Correlation;
           o.Payload = rb._payload;

            return o;
        }
    }

}