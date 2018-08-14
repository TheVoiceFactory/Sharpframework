using System.Reflection;


namespace Sharpframework.Core
{
    /// <summary>This is class dedicated to the strongly typed allocation of objects by a 
    /// standard pattern and the use of a (self instancing, readable) property (e.g. "Singleton").</summary>
    /// <typeparam name="AllocatedType">Type Parameter that indicates the type of object to
    /// allocate.</typeparam>
    public class PropInfoObjectAllocator<AllocatedType>
        : ObjectAllocator<AllocatedType>
    {
        private MethodInfo __mi;


        /// <summary>Custom Constructor</summary>
        /// <param name="ci">The PropertyInfo of the property that return new instance</param>
        public PropInfoObjectAllocator ( PropertyInfo pi )
        {
            if      ( pi            == null ) __mi = null;
            else if ( !pi.CanRead )           __mi = null;
            else if ( pi.GetMethod  == null ) __mi = null;
            else                              __mi = pi.GetMethod;
        } // End of Custom Constructor


        /// <summary>Implementation, of the object allocation.</summary>
        /// <returns>A new instance of the required type.</returns>
        protected override AllocatedType ImplAllocate ()
            => (AllocatedType) (__mi == null ? null : __mi.Invoke ( null, null ));
    } // End of Class PropInfoObjectAllocator<...>
} // End of Namespace Sharpframework.Core
