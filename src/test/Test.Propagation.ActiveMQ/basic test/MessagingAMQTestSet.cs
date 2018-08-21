using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Sharpframework.Propagation;
using NLog;
using Sharpframework.Propagation.Activemq;
using Sharpframework.EntityModel;
using Sharpframework.EntityModel.Implementation;

namespace Test.MessagingAMQ
{

    public class MessagingAMQTestSet
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        

        [Fact]
        public void Test1()
        {
            var man = new TopicManagerAMQP
            {
                ClientID = "1",
                TopicName = "topic1",
                BrokerUri = "devhost:",
                ConsumerID = "C2",
                IgnoreMyself = false//to loopback messages

            };
        }

        public interface IMyPayload
            : IEntityContract
        {
            string Nome { get; set; }
            string Cognome { get; set; }
        }

        public class MyPayload : Entity, IMyPayload // Sharpframework.Domains.Shared.Framework.IEntity
        {
            public string Nome { get; set; }
            public string Cognome { get; set; }

            public Type ContractType => throw new NotImplementedException();

            protected override IEnumerable<string> MyMainIdFieldsNames
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            protected override IEnumerable<object> MyMainIdFieldsValues
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            protected override Type ImplGetSerializationContract<SerializationContextType>(SerializationContextType context)
            {
                throw new NotImplementedException();
            }
        }

        public class MyFirstMessage : TopicMessage<MyFirstMessage, MyPayload>
        {
            public MyFirstMessage()
            {
            }

            public MyFirstMessage(MyPayload newclass)
            {
                this.Payload = newclass;
            }
        }

        public class MyFirstRequest : Request<MyFirstRequest, MyPayload>, IRequest<MyFirstRequest, MyPayload>
        {
            public MyFirstRequest(MyPayload newclass)
            {
                this.Payload = newclass;
            }
        }

        public class MyFirstResponse : Response<MyFirstResponse, IMyPayload>, IRequest<MyFirstResponse, IMyPayload>
        {
            public MyFirstResponse(IMyPayload payload, IRequest req) : base(payload, req)
            {
            }

            public MyFirstResponse()
            {
            }
        }

        [Fact]
        public void TestNlog()
        {
            logger.Trace(DateTime.Now);
        }

        [Fact]
        public void TestMSGSer()
        {
            var m1 = new MyFirstMessage(new MyPayload());
            string p = m1.Serialize();
            var m2 = m1.Deserialize(p);
            var brok = new TopicManagerAMQP
            {
                BrokerUri = "amqp://admin:admin@devhost:5672",
                TopicName = "General",
                ConsumerID = "TestMSGSer",
                ClientID = "TestMSGSer",
                IgnoreMyself = false,
                Timeout = -1
            };

            brok.StartTopicDialogue();
            brok.GeneralMessageReceived += Brok_GeneralMessageReceived;

            PingMsg ping = new PingMsg();
            ping.TargetID = "*";
            brok.Send(ping);

            MyFirstRequest msg2 = new MyFirstRequest(new MyPayload());
            brok.Send(m1);
            brok.StopTopicDialogue();
        }

        [Fact]
      //  [Category("Mess.Pippo")]
      //  [Property("Priority", 1)]
        public void TestAsyncRequest()
        {

            ITopicManager brok = new TopicManagerAMQP
            {
                BrokerUri = "amqp://admin:admin@devhost:5672",
                TopicName = "General",
                ConsumerID = "AsyncTestone",
                IgnoreMyself = false,
                Timeout = -1
            };

            brok.StartTopicDialogue();
            brok.GeneralMessageReceived += Brok_GeneralMessageReceived;
            Request<MyFirstRequest, MyPayload> msg2 = new MyFirstRequest(new MyPayload());
            logger.Trace("Correlation: " + msg2.Correlation);


            PingMsg ping = new PingMsg();
            ping.TargetID = "*";

            brok.Send(ping);

            Task<IResponse> ret = brok.SendRequestAsync(msg2);

            System.Threading.Thread.Sleep(1000);

            var resFact = new ResponseFactory<MyFirstResponse, IMyPayload>();
            MyFirstResponse resp2 = resFact.RespondTo(msg2).WithPayload(((IMyPayload)new MyPayload())); //crea la risposta simulata non attendendo la risposta

            brok.Send(((ITopicMessageContract)resp2)); //invia la risposta
            //System.Threading.Thread.Sleep(1000);
            var r = ret.Result;//recupera il valore in sincronro

            Assert.Equal(msg2.Correlation, r.Correlation);
        }

        private void Brok_GeneralMessageReceived(object sender, ITopicMessageContract e)
        {
            dynamic d = e;
            logger.Trace("MESSAGGIO: " + e.Name);
            ProcessMessages(d);
            //if (e as MyFirstRequest != null) { ProcessMessages((MyFirstRequest)e); }
        }

        private string _pm;

        private void ProcessMessages(ITopicMessageContract req)
        {
            logger.Trace("ITopicMessageContract");
            _pm += "_ITopicMessageContract_";
            // MyFirstResponse resp = (MyFirstResponse)MyFirstResponse.RespondTo(req);
        }

        private void ProcessMessages(MyFirstRequest req)
        {
            _pm += "_MYFIRSTR_";
            logger.Trace(_pm);
            // MyFirstResponse resp = (MyFirstResponse)MyFirstResponse.RespondTo(req);
        }

        private void ProcessMessages(IRequest req)
        {
            _pm += "_IREQUEST_";
            logger.Trace(_pm);
            // MyFirstResponse resp = (MyFirstResponse)MyFirstResponse.RespondTo(req);
        }

        private void ProcessMessages(MyFirstResponse req)
        {
            _pm += "_CTPT_";
            logger.Trace(_pm);
            logger.Trace(req.Correlation);
        }

        [Fact]
        public void TestResponseFactory()
        {
            var r = new MyFirstRequest(new MyPayload());
            var rf = new ResponseFactory<MyFirstResponse, IMyPayload>();
            Response<MyFirstResponse, IMyPayload> response = rf.
                                                                RespondTo(r).
                                                                WithPayload(((IMyPayload)new MyPayload()));
        }
    }
}