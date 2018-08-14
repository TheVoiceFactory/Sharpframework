using System;

using Sharpframework.Core;


namespace Test.EntityModel
{
    public class NameType
        : PrimitiveType<String, NameType>
    {
        public static implicit operator NameType ( String value )
            => ToDerivedType ( value );
    } // End of Class NameType
} // End of Namespace Test.EntityModel
