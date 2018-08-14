using System;

//using Sharpframework.Infrastructure;
using Sharpframework.Core;

namespace Sharpframework.EntityModel
{
    public interface IValueObject
        : IDisposable
        , IValueObjectContract
    {
        IFieldsValues Fields { get; }
    } // End of Interface IValueObject
} // End of Namespace Sharpframework.EntityModel
