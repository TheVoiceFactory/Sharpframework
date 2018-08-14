//using Sharpframework.Infrastructure;
using Sharpframework.Core;

namespace Sharpframework.EntityModel
{
    public interface ICompositedId
        : ICompositedIdContract
    {
        IFieldsValues IdFields { get; }
    } // End of Interface ICompositedId
} // End of Namespace Sharpframework.EntityModel
