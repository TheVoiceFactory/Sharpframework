
namespace Sharpframework.Core
{
    public interface IOwnerProvider<OwnerType>
    {
        OwnerType Owner { get; }
    } // End of Interface IOwnerProvider<...>
} // End of Namespace Sharpframework.Core