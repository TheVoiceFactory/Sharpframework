using System;

using Sharpframework.EntityModel;


namespace Sharpframework.Propagation.Facts
{
    public interface IExecVerbFact<TargetType>
        : IFact
    {
        Boolean Exec ( TargetType target );
    }
}
