using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Sharpframework.Core
{
    public static class Extensions
    {
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

        public static void TryDispose(ref Object target)
        { target.TryDispose(); target = null; }

        public static void TryDispose(this Object target)
        {
            IDisposable disp = target as IDisposable;

            if (disp != null) disp.Dispose();
        } // End of Extension TryDispose (...)
    } // End of Static Class Extensions
} // End of Namespace Sharpframework.Core