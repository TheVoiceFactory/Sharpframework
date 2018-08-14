
namespace Sharpframework.Core
{
    public interface IPrimitiveTypeValue
        : IValueGetter
        , IValueProvider
        , IValueSetter
    {
    } // End of Interface IPrimitiveTypeValue

    public interface IPrimitiveTypeValue<CoreType>
        : IPrimitiveTypeValue
        , IHasPrimitiveValue<CoreType>
        , IValueManager<CoreType>
    {
    } // End of Interface IPrimitiveTypeValue<...>
} // End of Namespace Sharpframework.Core
