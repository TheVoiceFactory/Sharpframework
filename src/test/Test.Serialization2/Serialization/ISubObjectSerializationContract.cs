using System;

using Sharpframework.EntityModel;


namespace Test.EntityModel.Serialization
{
    public interface ISubObjectSerializationContract
        : IValueObjectSerializationContract
    {
        String SubObjField1 { get; set; }
        String SubObjField2 { get; set; }
    } // End of Interface ISubObjectSerializationContract
} // End of Namespace Test.EntityModel.Serialization
