using System;
using System.Runtime.InteropServices;

using Sharpframework.EntityModel;


namespace Test.EntityModel.Serialization
{
    [ Guid ( "E94473EE-F53F-4B7B-95D6-36D78D327E87" ) ]
    public interface IMainObject
        : IEntity
    {
        String Field1 { get; }

        NameType Field2 { get; }

        ISubObject Field3 { get; }

        Boolean Field4 { get; }

        Double Field8 { get; }

        DateTime DateTimeField { get; }
    } // End of Interface IMainObject
} // End of Namespace Test.EntityModel.Serialization
