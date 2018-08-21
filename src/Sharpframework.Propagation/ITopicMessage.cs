using System.Collections.Generic;

namespace Sharpframework.Propagation
{
    // public delegate void MessageArrivedH<CT,PT>(ISkilMessage<CT, PT> message);
    public interface ITopicMessageContract
    {
        object PayloadObject { get; }
        string Name { get; }
        string FullName { get; }
        string Family { get; }
        string Origin { get; set; }
        IReadOnlyDictionary<string, string> HeaderProperties { get; }
        IDictionary<string, string> UserHeaderProperties { get; }
        string Serialize();
    }

    public interface ITopicMessage<ClassType, PayloadType>: ITopicMessageContract
    {
        PayloadType Payload { get; }
       //  ClassType Deserialize(string str);
    }

}