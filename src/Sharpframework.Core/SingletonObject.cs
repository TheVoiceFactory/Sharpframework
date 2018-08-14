
namespace Sharpframework.Core
{
    /// <summary>Utility base class that add a strongly typed self-instancing static property
    /// "Singleton" for a singleton instance.</summary>
    /// <typeparam name="SingletonType">Parameter Type that defines the singleton instance type.</typeparam>
    public class SingletonObject<SingletonType>
        where SingletonType : class, new ()
    {
        private static SingletonType __singleton;


        /// <summary>Static Constructor</summary>
        static SingletonObject () { __singleton = null; }


        /// <summary>At the first invokation this readonly property creates an instance of the type
        /// indicated by the related type parameter of the class, stores it internally then returns
        /// this value. All subsequent invokation of this property will simply returns the internal-
        /// ly stored value.</summary>
        /// <remarks>Because this property (as the internal stored member) is static, the allocation
        /// is made only once for each type that inherits from this base class.</remarks>
        public static SingletonType Singleton
        {
            get
            {
                if ( __singleton == null ) __singleton = new SingletonType ();

                return __singleton;
            }
        }
    } // End of Class SingletonObject<...>
} // End of Namespace Sharpframework.Core
