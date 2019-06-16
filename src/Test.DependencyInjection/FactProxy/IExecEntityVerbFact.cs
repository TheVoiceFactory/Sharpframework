using Sharpframework.EntityModel;


namespace Sharpframework.Propagation.Facts
{
    public interface IExecEntityVerbFact<TargetType>
        : IExecVerbFact<TargetType>
        , IUid
    {
    }
}
