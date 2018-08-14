using Sharpframework.Core;


namespace Sharpframework.EntityModel
{
    public interface ICompositedId
        : IValueObject
    {
        IFieldsValues IdFields { get; }
    } // End of Interface ICompositedId
} // End of Namespace Sharpframework.EntityModel
