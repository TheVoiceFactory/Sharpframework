using System;

using Sharpframework.EntityModel;


namespace Test.EntityModel.Serialization
{
    public interface ISubObjectContract
        : IValueObjectContract
    {
        String SubObjField1 { get; }
        String SubObjField2 { get; }
    } // End of Interface ISubObjectContract
} // End of Namespace Test.EntityModel.Serialization
