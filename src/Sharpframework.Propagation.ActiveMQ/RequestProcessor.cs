using System;
using NLog;
using System.Threading;
using Sharpframework.Propagation;

namespace Sharpframework.Propagation.Activemq
{

    internal class RequestProcessor
    {
        public int Timeout { get; private set; }
        private RequestProcessor() { }

        private AutoResetEvent semaphore = new AutoResetEvent(false);
        private TopicManagerAMQP Broker;
        private IRequest Request { get; set; }
        private IResponse Response { get; set; }

        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        internal RequestProcessor(TopicManagerAMQP broker, IRequest request, int timeout)
        {
            Broker = broker;
            Request = request;
            Timeout = timeout;
        }

        internal IResponse ProcessRequest()
        {   
            Broker.GeneralMessageReceived += Broker_ResponseReceived;
            //semaphore.WaitOne(Timeout, true);
            semaphore.WaitOne(Timeout);
            Broker.GeneralMessageReceived -= Broker_ResponseReceived;
            if (this.Response == null)
            {
                throw new TimeoutException();
            }
            logger.Trace("Response : " + this.Response.Serialize());
            return this.Response;
        }

        private void Broker_ResponseReceived(object sender, ITopicMessageContract e)
        {
            var response = e as IResponse;
            if (response == null) { return; }

            logger.Trace("Response : " + response.Serialize());

            if (response.Correlation == this.Request.Correlation)
            {
                this.Response = response;
                this.semaphore.Set();
            }


        }
    }
}