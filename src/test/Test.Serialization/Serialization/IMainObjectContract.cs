using System;
using System.Collections;
using System.Collections.Generic;

using Sharpframework.EntityModel;


namespace Test.EntityModel.Serialization
{
    public interface IMainObjectContract
        : IEntityContract
    {
        String Field1 { get; }

        NameType Field2 { get; }

        ISubObject Field3 { get; }

        Boolean Field4 { get; }

        IList<ISubObject> Field5 { get; }

        IList<Object> Field6 { get; }

        IList Field6Bis { get; }

        IDictionary<String, ISubObject> Field7 { get; }

        Double Field8 { get; }

        DateTime DateTimeField { get; }

        Boolean [] BooleanArray { get; }

        DateTime [] DateTimeArray { get; }

        Int32 [] Int32Array { get; }

        Object [] MixedArray { get; }

        ISubObject [] SubObjectArray { get; }
    } // End of Interface IMainObjectContract
} // End of Namespace Test.EntityModel.Serialization
