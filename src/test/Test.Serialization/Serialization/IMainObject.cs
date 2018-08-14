using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Sharpframework.Core;
using Sharpframework.EntityModel;


namespace Test.EntityModel.Serialization
{
    public class NameType : PrimitiveType<string, NameType>
    {
        public static implicit operator NameType(string value)
        {
            return NameType.ToDerivedType(value);
        }
    }

    [ Guid ( "E94473EE-F53F-4B7B-95D6-36D78D327E86" ) ]
    public interface IMainObject
        : IEntity
        , IMainObjectContract
    {
        new String Field1 { get; set; }

        new NameType Field2 { get; set; }

        new ISubObject Field3 { get; set; }

        new Boolean Field4 { get; set; }

        new IList<ISubObject> Field5 { get; set; }

        new IList<Object> Field6 { get; set; }

        new IList Field6Bis { get; set; }

        new IDictionary<String, ISubObject> Field7 { get; set; }

        new Double Field8 { get; set; }

        new DateTime DateTimeField { get; set; }

        new Boolean [] BooleanArray { get; set; }

        new DateTime [] DateTimeArray { get; set; }

        new Int32 [] Int32Array { get; set; }

        new Object [] MixedArray { get; set; }

        new ISubObject [] SubObjectArray { get; set; }
    } // End of Interface IMainObject
} // End of Namespace Test.EntityModel.Serialization
