using System.ComponentModel;

using Sharpframework.Core;


namespace Sharpframework.Serialization.ValueDom
{
    public class ValueItemPropertyDescriptor
        : WrappedPropertyDescriptor
    {
        /// <summary>Costruttore Predefinito.</summary>
        /// <param name="originalPD">Il PropertyDescriptor originario, di cui questa classe
        /// costituisce il Wrapper.</param>
        public ValueItemPropertyDescriptor ( PropertyDescriptor originalPD )
            : base ( originalPD ) { }
    } // End of Class ValueItemPropertyDescriptor
} // End of Namespace Sharpframework.Serialization.ValueDom
