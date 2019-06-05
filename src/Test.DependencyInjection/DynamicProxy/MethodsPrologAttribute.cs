using System;


namespace Test.DependencyInjection.DynamicProxy
{
    public class MethodsPrologAttribute : Attribute
    {
        public MethodsPrologAttribute ( String prologMethodName )
        { MethodName = prologMethodName; }

        public String MethodName { get; }
    }
}
