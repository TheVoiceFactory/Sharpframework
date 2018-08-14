using System;

using Sharpframework.Core;


namespace Sharpframework.EntityModel
{
    public interface IValueObject
        : IDisposable
        , IValueObjectSerializationContract
    {
        IFieldsValues Fields { get; }
    } // End of Interface IValueObject
} // End of Namespace Sharpframework.EntityModel

