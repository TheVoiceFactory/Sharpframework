using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Sharpframework.Core
{
    public static class Extensions
    {
        private static MethodInfo [] __emptyMethodInfo;

        static Extensions ()
        {
            __emptyMethodInfo = null;
        }


        public static void CopyPropertyValues(object source, object destination)
        {
            var destProperties = destination.GetType().GetProperties().Where(prop => prop.CanWrite);
            ////var destProperties = destination.GetType().GetProperties(System.Reflection.BindingFlags.SetProperty);

            foreach (var sourceProperty in source.GetType().GetProperties())
            {
                foreach (var destProperty in destProperties)
                {
                    if (destProperty.Name == sourceProperty.Name &&
                destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                    {
                        destProperty.SetValue(destination, sourceProperty.GetValue(
                            source, new object[] { }), new object[] { });

                        break;
                    }
                }
            }
        }

        public static void CopyValuesFrom(this object source, object destination)
        {
            CopyPropertyValues(source, destination);
        }

        public static MethodInfo [] Empty ( this MethodInfo [] target )
        {
            if ( __emptyMethodInfo == null ) __emptyMethodInfo = new MethodInfo [0];

            return __emptyMethodInfo;
        }

        public static PropertyDescriptorCollection GetAllInterfaceProperties ( this Type interfaceType )
        {
            if ( interfaceType      == null ) return null;
            if ( !interfaceType.IsInterface ) return null;

            List<Type>                              childInterfaces = null;
            PropertyDescriptorCollection            locPdc          = null;
            Dictionary<String, PropertyDescriptor>  pdl             = null;
            Type []                                 tmp             = null;

            if ( (tmp = interfaceType.GetInterfaces ()) != null )
                childInterfaces = new List<Type> ( tmp );

            childInterfaces.Add ( interfaceType );
            pdl = new Dictionary<String, PropertyDescriptor> ();

            foreach ( Type curInterface in childInterfaces )
                if ( curInterface.IsAssignableFrom ( interfaceType ) )
                {
                    locPdc = TypeDescriptor.GetProperties ( curInterface );

                    foreach ( PropertyDescriptor pd in locPdc )
                        if ( !pdl.ContainsKey ( pd.Name ) ) pdl.Add ( pd.Name, pd );
                }

            return new PropertyDescriptorCollection ( pdl.Values.ToArray () );
        } // End of GetAllInterfaceProperties (...)


        public static ConstructorInfo GetConstructor ( this Type target, params Type [] arguments )
            => target == null ? null : target.GetConstructor ( arguments );
        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            if (type.GetTypeInfo().IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(
                        BindingFlags.FlattenHierarchy
                        | BindingFlags.Public
                        | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy
                | BindingFlags.Public | BindingFlags.Instance);
        }

        public static MethodInfo [] RemovePropertiesAccessors ( this MethodInfo [] target
            , IEnumerable<PropertyInfo> properties )
        {
            void RemovePropertiesAccessors ( IList<MethodInfo> methods, MethodInfo accessor )
            {
                if ( methods.Contains ( accessor ) ) methods.Remove ( accessor);
            }

            List<MethodInfo> methods        = new List<MethodInfo> ();
            List<MethodInfo> specialNames   = null;

            if ( target == null ) return target.Empty ();

            foreach ( MethodInfo mi in target )
                if ( mi.MemberType != MemberTypes.Method )
                    continue;
                else if ( mi.IsSpecialName )
                    if ( specialNames == null )
                        (specialNames = new List<MethodInfo> ()).Add ( mi );
                    else
                        specialNames.Add ( mi );
                else
                    methods.Add ( mi );

            if ( specialNames == null ) return target;

            if ( properties == null ) properties = target [0].DeclaringType.GetProperties ();

            foreach ( PropertyInfo pi in properties )
            {
                RemovePropertiesAccessors ( specialNames, pi.GetMethod );
                RemovePropertiesAccessors ( specialNames, pi.SetMethod );
            }

            if ( specialNames.Count > 0 ) methods.AddRange ( specialNames );

            return methods.ToArray ();
        }

        public static void TryDispose(ref Object target)
        { target.TryDispose(); target = null; }

        public static void TryDispose(this Object target)
        {
            IDisposable disp = target as IDisposable;

            if (disp != null) disp.Dispose();
        } // End of Extension TryDispose (...)
    } // End of Static Class Extensions
} // End of Namespace Sharpframework.Core