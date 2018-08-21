using System;
using System.Threading.Tasks;

namespace Sharpframework.Propagation
{
    public interface ITopicManagerContract
    {
        string BrokerUri { get;  set; }
        string ClientID { get;  set; }
        string ConsumerID { get;  set; }
        string Selector { get;  set; }
        string TopicName { get;  set; }
        bool IgnoreMyself { get; set; }
    }

    public interface ITopicManager:ITopicManagerContract
    {

        int Timeout { get; set; }

        event EventHandler<ITopicMessageContract> GeneralMessageReceived;

        void OnGeneralMessageReceived(ITopicMessageContract message);
        void Send(ITopicMessageContract message);
        void Send(IRequest message);
        object SendRequest(IRequest request);
        void SendRequest<ResponseType>(ref ResponseType response, IRequest request) where ResponseType : class,IResponse ;
        ResponseType Send<ResponseType>( IRequest request) where ResponseType : class, IResponse;
        Task<IResponse> SendRequestAsync(IRequest request);
        void StartTopicDialogue();
        void StopTopicDialogue();
    }
}