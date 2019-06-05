using System;


namespace Test.DependencyInjection.DynamicProxy
{
    public class MethodsEpilogAttribute : Attribute
    {
        public MethodsEpilogAttribute ( String epilogMethodName )
        { MethodName = epilogMethodName; }

        public String MethodName { get; }
    }
}
