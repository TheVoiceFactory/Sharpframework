namespace Sharpframework.Propagation
{
    public interface ISystemCommand : ITopicMessageContract { }

    public interface ISystemCommand<ClassType, PayloadType> : ICommand
    {
    }
}