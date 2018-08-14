using System;

using Sharpframework.EntityModel;


namespace Test.EntityModel.Serialization
{
    public interface IMainObjectSerializationContract
        : IEntitySerializationContract 
    {
        String Field1 { get; set; }

        NameType Field2 { get; set; }

        ISubObject Field3 { get; set; }

        Boolean Field4 { get; set; }

        //IList<ISubObject> Field5 { get; set; }

        //IList<Object> Field6 { get; set; }

        //IList Field6Bis { get; set; }

        //IDictionary<String, ISubObject> Field7 { get; set; }

        Double Field8 { get; set; }

        DateTime DateTimeField { get; set; }

        //Boolean [] BooleanArray { get; set; }

        //DateTime [] DateTimeArray { get; set; }

        //Int32 [] Int32Array { get; set; }

        //Object [] MixedArray { get; set; }

        //ISubObject [] SubObjectArray { get; set; }
    } // End of Interface IMainObjectContract
} // End of Namespace Test.EntityModel.Serialization
