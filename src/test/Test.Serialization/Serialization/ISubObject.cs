using System;

using Sharpframework.EntityModel;


namespace Test.EntityModel.Serialization
{
    public interface ISubObject
        : IValueObject
        , ISubObjectContract
    {
        new String SubObjField1 { get; set; }
        new String SubObjField2 { get; set; }
    } // End of Interface ISubObject
} // End of Namespace Test.EntityModel.Serialization