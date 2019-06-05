using System;


namespace Sharpframework.Roslyn.DynamicProxy
{
    public enum InterceptorType
    {
        Prolog,
        Epilog
    }

    //public delegate void PrologInterceptorDelegate (
    //    MemberType          memberType,
    //    String              memberName,
    //    Object              instance,
    //    params Object []    parameters );
    public class PrologInterceptorAttribute : InterceptorAttribute
    {
        public PrologInterceptorAttribute ( Type interceptorClass, String interceptorMethodName )
            : base ( interceptorClass, interceptorMethodName, InterceptorType.Prolog ) { }

    }

    public class EpilogInterceptorAttribute : InterceptorAttribute
    {
        public EpilogInterceptorAttribute ( Type interceptorClass, String interceptorMethodName )
            : base ( interceptorClass, interceptorMethodName, InterceptorType.Epilog ) { }

    }

    public class InterceptorAttribute
        : Attribute
    {
        public readonly Type            InterceptorClass;
        public readonly String          InterceptorMethodName;
        public readonly InterceptorType InterceptorType;
        //public readonly PrologInterceptorDelegate InterceptorDelegate;


        public InterceptorAttribute (
            Type            interceptorClass,
            String          interceptorMethodName,
            InterceptorType interceptorType )
        {
            InterceptorClass        = interceptorClass;
            InterceptorMethodName   = interceptorMethodName;
            InterceptorType         = interceptorType;
            //__objType = t;

            //Func<Type[], Type> getType = Expression.GetActionType;
            //MethodInfo mi = t.GetMethod ( kkk );
            //var types = mi.GetParameters().Select(p => p.ParameterType);

            //InterceptorDelegate = Delegate.CreateDelegate ( typeof ( PrologInterceptorDelegate ), mi ) as PrologInterceptorDelegate;
        }
    }
}
