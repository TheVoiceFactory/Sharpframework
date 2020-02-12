using System;

using Sharpframework.Roslyn.DynamicProxy;


namespace Test.DependencyInjection.DynamicProxy
{
    public static class PrologEpilog
    {
        public static void Epilog (
            MemberType          memberType,
            String              memberName,
            Object              instance,
            Object              retVal,
            params Object []    parameters )
        {
            if (memberName.Contains("Plutol"))
            {
                Console.Write(".");
            }
        }

        public static void Prolog (
            MemberType          memberType,
            String              memberName,
            Object              instance,
            params Object []    parameters )
        {
            if (memberName.Contains("Paperino"))
            {
                Console.Write("-");
            }
        }
    }

}
