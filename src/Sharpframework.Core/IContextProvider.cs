
namespace Sharpframework.Core
{
    public interface IContextProvider<ContextType>
        where ContextType : IContext
    {
        ContextType Context { get; }
    } // End of Interface IContextProvider<...>
} // End of Namespace Sharpframework.Core