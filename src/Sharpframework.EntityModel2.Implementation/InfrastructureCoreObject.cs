using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sharpframework.Core;

namespace Sharpframework.EntityModel.Implementation
{
    public class InfrastructureCoreObject
        : CoreObject 
        , IDisposable
        , IEquatable<InfrastructureCoreObject>
    {
        private class _FieldValuesImpl
            : IFieldsValues
            , IDisposable
        {
            private Object __belongingSet;
            private InfrastructureCoreObject __srcObj;
            private Func<IEnumerable<String>> __NamesDlg;
            private Func<IEnumerable<Object>> __ValuesDlg;


            public _FieldValuesImpl(Func<IEnumerable<String>> namesDlg,
                                        Func<IEnumerable<Object>> valuesDlg,
                                        InfrastructureCoreObject srcObj)
                : this(namesDlg, valuesDlg, srcObj, null) { }

            public _FieldValuesImpl(Func<IEnumerable<String>> namesDlg,
                                        Func<IEnumerable<Object>> valuesDlg,
                                        InfrastructureCoreObject srcObj,
                                        Object belongingSet)
            {
                _Reset();

                __belongingSet = belongingSet == null ? BelongingSetAttribute.MainSet : belongingSet;
                __srcObj = srcObj;
                __NamesDlg = namesDlg;
                __ValuesDlg = valuesDlg;
            } // End of Custom Constructor


            private IEnumerable<String> _Names
            {
                get
                {
                    if (__NamesDlg != null)
                    {
                        IEnumerable<String> names = __NamesDlg();

                        if (names != null)
                            foreach (String name in names)
                                yield return name;
                    }

                    if (__srcObj == null) yield break;
                    object[] attrs = null;
                    //IEnumerable<Attribute> attrs = null;
                    Type srchAttrType = typeof(BelongingSetAttribute);

                    foreach (PropertyInfo p in __srcObj.GetType().GetPublicProperties())
                        if ((attrs = p.GetCustomAttributes(srchAttrType, true)) != null)
                            foreach (BelongingSetAttribute attr in attrs)
                                if (attr.BelongingSet == __belongingSet)
                                    yield return String.IsNullOrWhiteSpace(attr.Name)
                                                            ? p.Name : attr.Name;

                    yield break;
                }
            }

            private IEnumerable<Object> _Values
            {
                get
                {
                    if (__ValuesDlg != null)
                    {
                        IEnumerable<Object> values = __ValuesDlg();

                        if (values != null)
                            foreach (Object value in values)
                                yield return value;
                    }

                    if (__srcObj == null) yield break;

                    object[] attrs = null;
                   
                    //IEnumerable<Attribute> attrs = null;
                    Type srchAttrType = typeof(BelongingSetAttribute);

                    foreach (PropertyInfo p in __srcObj.GetType().GetPublicProperties())
                        if ((attrs = p.GetCustomAttributes(srchAttrType, true)) != null)
                            foreach (BelongingSetAttribute attr in attrs)
                                if (attr.BelongingSet == __belongingSet)
                                    yield return p.GetValue(__srcObj);

                    yield break;
                }
            }

            private static Boolean _AreEquals(Object a, Object b)
            {
                // String is also Enumerable, so we can do direct comparation in order to avoid
                // Enumerable comparation.
                if (a as String != null) return String.Equals(a, b);

                IEnumerable enumA = a as IEnumerable;
                IEnumerable enumB = b as IEnumerable;

                if (enumA == null) return Object.Equals(a, b);
                if (enumB == null) return false;

                IEnumerable<Object> objEnumA = enumA as IEnumerable<Object>;
                IEnumerable<Object> objEnumB = enumB as IEnumerable<Object>;

                if (objEnumA == null) objEnumA = enumA.OfType<Object>();
                if (objEnumB == null) objEnumB = enumA.OfType<Object>();

                return objEnumA.SequenceEqual(objEnumB);
            } // End of _AreEquals (...)


            private Int32 _IndexOfKey(String key)
            {
                if (String.IsNullOrWhiteSpace(key)) return -1;
                if (__ValuesDlg == null) return -1;

                Int32 idx = -1;

                foreach (String nameKey in _Names)
                    if (key.Equals(nameKey))
                        return ++idx;
                    else
                        ++idx;

                return -1;
            }

            private Object _ValueAt(Int32 index)
            {
                if (index < 0) return null;
                if (_Values == null) return null;

                IEnumerator<Object> valuesEnum = _Values.GetEnumerator();

                if (valuesEnum == null) return null;

                while (index-- >= 0) if (!valuesEnum.MoveNext()) return null;

                return valuesEnum.Current;
            }


            private void _Reset()
            {
                __belongingSet = null;
                __srcObj = null;
                __NamesDlg = null;
                __ValuesDlg = null;
            } // End of _Reset ()


            public void Dispose() { _Reset(); }

            Object IFieldsValues.this[String name]
            { get { return _ValueAt(_IndexOfKey(name)); } }

            IEnumerable<String> IFieldsValues.Names
            { get { return _Names; } }

            IEnumerable<Object> IFieldsValues.Values
            { get { return _Values; } }

            Boolean IFieldsValues.Equals(IFieldsValues other)
            {
                if (other == null) return false;
                if (other.Values == null) return false;
                if (__ValuesDlg == null) return false;

                IEnumerator<Object> otherValuesEnum = other.Values.GetEnumerator();
                IEnumerator<Object> thisValuesEnum = _Values.GetEnumerator();

                if (otherValuesEnum == null) return false;
                if (thisValuesEnum == null) return false;

                try
                {
                    while (true)
                    {
                        if (!otherValuesEnum.MoveNext())
                            return !thisValuesEnum.MoveNext();
                        else if (!thisValuesEnum.MoveNext())
                            return false;
                        else if (thisValuesEnum.Current is IFieldsValues)
                            continue;
                        else if (!_AreEquals(thisValuesEnum.Current, otherValuesEnum.Current))
                            return false;
                        //else if ( !Object.Equals ( otherValuesEnum.Current,
                        //                            thisValuesEnum.Current ) )
                    }
                }
                finally { otherValuesEnum.Dispose(); thisValuesEnum.Dispose(); }
            } // End of IFieldsValues.Equals (...)
        } // End of Class _FieldValuesImpl


        private IFieldsValues __fields;
        private Type __fieldsViewType;


        protected InfrastructureCoreObject() { _Reset(); }

        ~InfrastructureCoreObject() { ImplDispose(false); }

        [Newtonsoft.Json.JsonIgnore]
        public IFieldsValues Fields { get { return _Fields; } }

        public void Dispose() { try { ImplDispose(true); } finally { } }

        public override Boolean Equals(Object obj)
        {
            InfrastructureCoreObject other = obj as InfrastructureCoreObject;

            return other == null ? base.Equals(obj) : Equals(other);
        } // End of Equals (...)

        public Boolean Equals(InfrastructureCoreObject other)
        { return other != null && other.Fields.Equals(Fields); }

        public override Int32 GetHashCode()
        { return base.GetHashCode(); }


        protected virtual IEnumerable<String> ImplFieldsNames
        {
            get
            {
                foreach (PropertyInfo p in _FieldsViewType.GetPublicProperties())
                    yield return p.Name;

                yield break;
            }
        }

        protected static IFieldsValues MyFieldsValuesFactory(
            Func<IEnumerable<String>> namesDlg,
            Func<IEnumerable<Object>> valuesDlg,
            InfrastructureCoreObject srcObj)
        { return new _FieldValuesImpl(namesDlg, valuesDlg, srcObj); }

        protected static IFieldsValues MyFieldsValuesFactory(
            Func<IEnumerable<String>> namesDlg,
            Func<IEnumerable<Object>> valuesDlg,
            InfrastructureCoreObject srcObj,
            Object belongingSet)
        { return new _FieldValuesImpl(namesDlg, valuesDlg, srcObj, belongingSet); }


        protected virtual IEnumerable<Object> ImplFieldsValues
        {
            get
            {
                foreach (PropertyInfo p in _FieldsViewType.GetPublicProperties())
                    yield return p.GetValue(this);

                yield break;
            }
        }

        protected virtual Type ImplFieldsViewType { get { return GetType(); } }


        protected virtual void ImplDispose(Boolean disposing)
        {
            try
            {
                if (disposing)
                {
                    __fields.TryDispose();
                    //if ( __fields != null ) __fields.Dispose ();
                }

                _Reset();
            }
            finally {; }
        } // End of ImplDispose (...)


        private IFieldsValues _Fields
        {
            get
            {
                if (__fields == null)
                    __fields = MyFieldsValuesFactory(
                                    delegate { return ImplFieldsNames; },
                                    delegate { return ImplFieldsValues; },
                                    this);

                return __fields;
            }
        }

        private Type _FieldsViewType
        {
            get
            {
                if (__fieldsViewType == null) __fieldsViewType = ImplFieldsViewType;

                return __fieldsViewType;
            }
        }


        private void _Reset()
        {
            __fields = null;
            __fieldsViewType = null;
        } // End of _Reset ()
    } // End of Class CoreObject


} // End of Namespace Sharpframework.Infrastructure
