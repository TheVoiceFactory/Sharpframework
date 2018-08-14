
namespace Sharpframework.Core
{
    public interface IValueManager<ValueType>
        : IValueProvider<ValueType>
    {
        new ValueType Value { get; set; }
    } // End of Interface IValueManager<...>
} // End of Namespace Sharpframework.Core
