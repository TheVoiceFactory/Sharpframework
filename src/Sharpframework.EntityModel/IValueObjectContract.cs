using System;

namespace Sharpframework.EntityModel
{
    public interface IValueObjectContract
        : IContractBase
    {
        Type ContractType { get; }
    } // End of Interface IValueObjectContract
} // End of Namespace Sharpframework.EntityModel
