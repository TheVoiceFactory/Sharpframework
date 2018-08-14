
namespace Sharpframework.Core
{
    /// <summary>This is a base class dedicated to the strongly typed allocation of objects by a 
    /// standard pattern.</summary>
    /// <typeparam name="AllocatedType">Type Parameter that indicates the type of object to
    /// allocate.</typeparam>
    /// <remarks>This is an abstract class that defines the standard pattern to use to generate
    /// a new object of type defined by the related class parameter type, so it defines only a
    /// function (named "Allocate"), that simply routes the call to another abstrat protected
    /// function to be implemented by the derived type.</remarks>
    public abstract class ObjectAllocator<AllocatedType>
    {
        /// <summary>Default Constructor</summary>
        protected ObjectAllocator () { }


        /// <summary>Function that allocate a new instance of the type indicated by the related
        /// class parameter type.</summary>
        /// <returns>A new instance of the required type.</returns>
        public AllocatedType Allocate () => ImplAllocate ();


        /// <summary>Implementation (delegated to the inheritor), of the object allocation.</summary>
        /// <returns>A new instance of the required type.</returns>
        protected abstract AllocatedType ImplAllocate ();
    } // End of Class ObjectAllocator<...>
} // End of Namespace Sharpframework.Core
