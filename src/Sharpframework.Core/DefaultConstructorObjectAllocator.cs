using System.Reflection;


namespace Sharpframework.Core
{
    /// <summary>This is class dedicated to the strongly typed allocation of objects by a 
    /// standard pattern and the use of the default constructor.</summary>
    /// <typeparam name="AllocatedType">Type Parameter that indicates the type of object to
    /// allocate.</typeparam>
    public class DefaultConstructorObjectAllocator<AllocatedType>
        : ObjectAllocator<AllocatedType>
    {
        private ConstructorInfo __ci;


        /// <summary>Custom Constructor</summary>
        /// <param name="ci">The ConstructorInfo to use to allocate new instances</param>
        public DefaultConstructorObjectAllocator ( ConstructorInfo ci )
        {
            __ci = ci;
        } // End of Custom Constructor


        /// <summary>Implementation, of the object allocation.</summary>
        /// <returns>A new instance of the required type.</returns>
        protected override AllocatedType ImplAllocate ()
            => (AllocatedType) (__ci == null ? null : __ci.Invoke ( null ));
    } // End of Class DefaultConstructorObjectAllocator<...>
} // End of Namespace Sharpframework.Core
