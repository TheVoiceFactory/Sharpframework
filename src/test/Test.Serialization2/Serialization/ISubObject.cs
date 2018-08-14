using System;

using Sharpframework.EntityModel;


namespace Test.EntityModel.Serialization
{
    public interface ISubObject
        : IValueObject
    {
        String SubObjField1 { get; }
        String SubObjField2 { get; }
    } // End of Interface ISubObject2
} // End of Namespace Test.EntityModel.Serialization
