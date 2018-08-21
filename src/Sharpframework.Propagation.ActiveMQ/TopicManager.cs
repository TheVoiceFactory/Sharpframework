using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Amqp;
using NLog;
using Amqp.Framing;
using System.Text;
using Sharpframework.Core;
//using Sharpframework.Implementations.Domains.Shared.Model;

namespace Sharpframework.Propagation.Activemq
{
    public interface ITopicConnection
    {
    }


    public class TopicManagerAMQP : ITopicManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
      
        protected Connection connection;
        protected Session session;
        protected ReceiverLink consumer;
        protected SenderLink producer;
       
        public string BrokerUri { get; set; }
        public string TopicName { get; set; }
        public string ClientID { get; set; }
        public string ConsumerID { get; set; }
        public string Selector { get; set; }
        public int Timeout { get; set; } = 3000;

        public bool IgnoreMyself { get; set; } = true;

        public event EventHandler<ITopicMessageContract> GeneralMessageReceived;

        public void OnGeneralMessageReceived(ITopicMessageContract message)
        {
            logger.Info("OnGeneralMessageReceived");
            GeneralMessageReceived?.Invoke(this, message);
        }
       
       
        void Consumer_Listener(Amqp.IReceiverLink receiver , Amqp.Message  receivedMsg)
        {
            logger.Info("Consumer_Listener");
            logger.Trace($"IMessage receive:{receivedMsg.ApplicationProperties } - {receivedMsg.Properties.MessageId }");
            Message tm = receivedMsg as Message;
            if (tm != null)
            {
                logger.Trace($"Payload:{tm } - {tm.Properties.MessageId }");
            }
            if (IgnoreMyself)
            {
                if (tm.ApplicationProperties["ConsumerID"].ToString() == this.ConsumerID)
                {
                    logger.Trace($"IMessage ignored:{receivedMsg  }");
                    return;
                }
            }
            //__________________
            dynamic message = receivedMsg as Message;
            ITopicMessageContract mess = null;
            try
            {
                mess = DeserializeMsg(message.Body);
            }
            catch (Exception e)
            {

                logger.Error(e, $"TopicMangaer Deserialize Failed:{receivedMsg }");
            }

            if (message != null) { OnGeneralMessageReceived(mess); }

            //___________________
        }

        protected virtual void SessionInit()
        {
#if (DEBUG)
            if (BrokerUri == "") { BrokerUri = "amqp://admin:admin@devhost:5672"; }
#endif
            StopTopicDialogue();
            connection = new Connection(new Amqp.Address(BrokerUri));
            session = new Session(connection);

        }
        protected virtual void TopicConsumerProducerInit()
        {
            string senderSubscriptionId = "cedemo.amqp.sender";
            string receiverSubscriptionId = "cedemo.amqp.receiver";
           
            if (producer == null)
            {
                producer = new SenderLink(session, senderSubscriptionId, TopicName);
            }

            if (consumer == null) {
                consumer = new ReceiverLink(session, receiverSubscriptionId, TopicName);
               
                consumer.Start(5, (Consumer_Listener));
            }

            //TODO : IMPLEMENT PERSITENT MODE
            //Producer.DeliveryMode = MsgDeliveryMode.Persistent;
            //Producer.RequestTimeout = new TimeSpan(1000000);
        }
        public void StartTopicDialogue()
        {
            if (connection == null || session == null) {
                logger.Info("Session init");
                SessionInit();
            }
            if (producer == null || consumer == null) {
                logger.Info("TopicConsumerProducerInit init");
                TopicConsumerProducerInit();
            }
        }
        public void StopTopicDialogue()
        {
            if (consumer != null) { consumer.Close(); consumer.TryDispose(); consumer = null; }
            if (producer != null) { producer.Close(); producer.TryDispose(); producer = null; }

            if (session != null) { session.Close(); session.TryDispose(); session = null; }//paranoia settings , intanto deve essere eseguito alla sola chiusura
            if (connection != null) { connection.Close(); connection.TryDispose(); connection = null; }
        }

        public TopicManagerAMQP() { }

        public static ITopicMessageContract DeserializeMsg(string str)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ObjectCreationHandling = ObjectCreationHandling.Replace //CARLOADD
            };

            var res = JsonConvert.DeserializeObject(str, settings);
            return (ITopicMessageContract)res;
        }
        private Message PrepareAMQMessage(ITopicMessageContract lMess)
        {
            var messAMQ = new Message(lMess.Serialize())
            {

                // Add a message id
                Properties = new Properties() { MessageId = Guid.NewGuid().ToString() }
            };
            lMess.Origin = this.ConsumerID;

            // Add some message properties
            messAMQ.ApplicationProperties = new ApplicationProperties();
            messAMQ.ApplicationProperties["Name"] = lMess.Name;
            messAMQ.ApplicationProperties["ClientID"] = this.ClientID;
            messAMQ.ApplicationProperties["ConsumerID"] = this.ConsumerID;
            foreach (KeyValuePair<string, string> s in lMess.HeaderProperties)
            {
                messAMQ.ApplicationProperties[s.Key] = s.Value;
            }
            foreach (KeyValuePair<string, string> s in lMess.UserHeaderProperties)
            {
                messAMQ.ApplicationProperties[s.Key] = s.Value;

            }

            return messAMQ;
        }
        //todo aggiungere un metodo base per preparazione
        private Message PrepareAMQMessage(IRequest lMess)
        {
            //Fill data wi
            var message = new Message(lMess.Serialize())
            {

                // Add a message id
                Properties = new Properties() { MessageId = Guid.NewGuid().ToString() }
            };
            lMess.Origin = this.ConsumerID;

            message.ApplicationProperties = new ApplicationProperties();
            message.ApplicationProperties["Name"] = lMess.Name;
            message.ApplicationProperties["ClientID"] = this.ClientID;
            message.ApplicationProperties["ConsumerID"] = this.ConsumerID;
            message.ApplicationProperties["ClientID"] = this.ConsumerID;

            if (lMess.Correlation == null | lMess.Correlation == "")
            {
                //correlation (paranoia setting)
                var correlationID = Guid.NewGuid().ToString();
                message.ApplicationProperties["Correlation"] = correlationID;
                message.Properties.SetCorrelationId(correlationID);
            }
            else
            {
                message.ApplicationProperties["Correlation"] = lMess.Correlation;
                message.Properties.SetCorrelationId(lMess.Correlation);
            }

            
            message.Properties.ReplyTo = TopicName;
            message.Properties.To = TopicName;

            return message;
        }

        private void MySend(Message message)
        {
            try
            {
                logger.Trace("Send message" + message.ToString());
                this.producer.Send(message);
            }
            catch (Exception e)
            {
                logger.Error(e, "During Send Message AMQ");
                // throw;
            }

        }
        public void Send(ITopicMessageContract message)
        {
            Message mess = PrepareAMQMessage(message);
            MySend(mess);
        }
        public void Send(IRequest message)
        {
            Message mess = PrepareAMQMessage(message);
            MySend(mess);
        }
        public object SendRequest(IRequest request)
        {
            logger.Trace("Send Request");
            Task<IResponse> output = this.SendRequestAsync(request);
            object o = output.Result;
            return o;
        }
        public Task<IResponse> SendRequestAsync(IRequest request)
        {
            RequestProcessor rp = new RequestProcessor(this, request, Timeout);
            Task<IResponse> output = new Task<IResponse>(rp.ProcessRequest);
            this.producer.Send(PrepareAMQMessage(request));
            output.Start();
            return output;
        }

        public void SendRequest<ResponseType>(ref ResponseType response, IRequest request)
             where ResponseType : class, IResponse
        {
            response = SendRequest(request) as ResponseType;
        }

        public ResponseType Send<ResponseType>(IRequest request) where ResponseType : class, IResponse
        {
            return SendRequest(request) as ResponseType;
        }
    }
}
