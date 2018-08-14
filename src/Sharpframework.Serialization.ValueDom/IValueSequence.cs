
namespace Sharpframework.Serialization.ValueDom
{
    using System.Collections;

    using Sharpframework.Serialization;


    public interface IValueSequence
        : IValueItemBase
        , ISymbolTableValueSequence
    {
        new IEnumerable Value { get; }
    } // End of Interface IValueSequence
} // End of Namespace Sharpframework.Serialization.ValueDom
