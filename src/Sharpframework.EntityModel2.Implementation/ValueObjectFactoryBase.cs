using System;
using System.Collections.Generic;
using System.Text;
using Sharpframework.Core;
using Sharpframework.Serialization;
using Sharpframework.Serialization.ValueDom;


namespace Sharpframework.EntityModel.Implementation
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    //using Sharpframework.EntityModel;
    //using Serialization;
   


    public abstract class ValueObjectFactoryBase<InterfaceType, FactoryType>
        : ValueObjectFactoryBase<
                InterfaceType,
                ValueObjectFactoryBase<InterfaceType, FactoryType>.DummyInitDto,
                FactoryType>
        , IFactory<InterfaceType>
        , ISerializable<IValueUnit, InterfaceType>
    where InterfaceType : IValueObject
    where FactoryType : ValueObjectFactoryBase<InterfaceType, FactoryType>
                        , new()
    {
        public class DummyInitDto
            : AbsValueObjectInitDto
        {
            protected override InterfaceType ImplCore => default(InterfaceType);
        }

        private new DummyInitDto InitDto { get => null; }

        private new InterfaceType GetByDto() => default(InterfaceType);
        private new InterfaceType ImplInitDto { get => default(InterfaceType); }
    } // End of ValueObjectFactoryBase<...>


    public abstract class ValueObjectFactoryBase<InterfaceType, InitDtoType, FactoryType>
        : SingletonObject<FactoryType>
        , IFactory<InterfaceType>
        , ISerializable<IValueUnit, InterfaceType>
    where InterfaceType : IValueObject
    where InitDtoType : ValueObjectFactoryBase<InterfaceType, InitDtoType, FactoryType>.AbsValueObjectInitDto
                        , new()
    where FactoryType : ValueObjectFactoryBase<InterfaceType, InitDtoType, FactoryType>
                        , new()
    {
        public abstract class AbsValueObjectInitDto
        {
            private ValueObject __core;

            private ValueObject _Core
            {
                get
                {
                    if (__core == null)
                    {
                        __core = ImplCore as ValueObject;
                        // Init Default Values...
                    }

                    return __core;
                }
            }

            public InterfaceType CoreObject { get => ImplCore; }

            public void Clear() => ImplClear();

            protected virtual void ImplClear() { __core = null; }

            protected abstract InterfaceType ImplCore { get; }
        } // End of Class AbsValueObjectInitDto


        private InitDtoType __initDto;


        public InitDtoType InitDto { get => ImplInitDto; }


        protected virtual InitDtoType ImplInitDto
        {
            get
            {
                if (__initDto == null) __initDto = new InitDtoType();

                return __initDto;
            }
        }

        public InterfaceType Deserialize<SerializationContextType>(
            IValueUnit serialization,
            SerializationContextType context)
        where SerializationContextType : ISerializationContext
            => ImplDeserialize(serialization, context);

        public InterfaceType GetByDto() => ImplGetByDto();
        public InterfaceType GetNew() => ImplGetNew();

        public IValueUnit Serialize<SerializationContextType>(InterfaceType instance, SerializationContextType context)
            where SerializationContextType : ISerializationContext
        {
            return instance == null ? null : ImplDispatchSerialize(instance, context);
        }

        public IValueUnit Serialize<SerializationContextType>(object instance, SerializationContextType context) where SerializationContextType : ISerializationContext
        {
            throw new System.NotImplementedException();
        }

        object ISerializable<IValueUnit>.Deserialize<SerializationContextType>(IValueUnit serialization, SerializationContextType context)
        {
            throw new System.NotImplementedException();
        }



        protected virtual InterfaceType ImplGetByDto() => InitDto.CoreObject;  //default ( InterfaceType );
        protected abstract InterfaceType ImplGetNew();

        protected abstract InterfaceType ImplDeserialize<SerializationContextType>(
            IValueUnit serialization,
            SerializationContextType context)
        where SerializationContextType : ISerializationContext;

        protected virtual IValueUnit ImplDispatchSerialize(InterfaceType instance, dynamic context)
        {
            return ImplSerialize(instance, context);
        } // End of ImplDispatchSerialize (...)

        protected Boolean ImplIsBuiltinType(Type type)
        {
            //if ( type.IsPrimitive ) return true;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.DBNull:
                case TypeCode.Empty:
                case TypeCode.String:
                    return false;

                case TypeCode.Object:
                    break;

                default:
                    return true;
            }

            Type primValType = type.GetInterface("IHasPrimitiveValue`1");

            if (primValType == null) return false;
            if (!primValType.GetGenericArguments()[0].IsPrimitive) return false;

            return true;
        } // End of ImplIsBuiltinType (...)

        protected Boolean ImplIsString(Type type)
        {
            if (type == typeof(String)) return true;

            Type primValType = type.GetInterface("IHasPrimitiveValue`1");

            if (primValType == null) return false;
            if (primValType.GetGenericArguments()[0] != typeof(String)) return false;

            return true;
        } // End of ImplIsString (...)

        protected virtual IValueUnit ImplSerialize(InterfaceType instance, IHierarchicalMetadataSerializationContext context)
        {
            if (instance == null) return null;

            IValueUnit curCtxLvl = context.Hierarchy.Peek() as IValueUnit;
            IEnumerable enumerable = null;
            //ArrayList                                   enumValues  = null;
            PropertyDescriptorCollection pdc = null;
            Type propType = null;
            Object propValue = null;
            ValueUnit retVal = new ValueUnit();
            IValueUnit subUnit = null;

            //pdc = TypeDescriptor.GetProperties ( instance.GetSerializationContract ( context ) );
            pdc = instance.GetSerializationContract(context).GetAllInterfaceProperties();

            foreach (PropertyDescriptor pd in pdc)
            {
                if ((propValue = pd.GetValue(instance)) == null) continue;
                if ((propType = propValue.GetType()) == null) continue;

                //propStrValue = propValue.ToString (); // Default serializer...

                //valueItem = new ValueItem ( pd.Name, propValue );

                //if ( String.IsNullOrWhiteSpace ( propStrValue ) ) continue;
                if (ImplIsBuiltinType(propType))
                    retVal.Add(new ValueItem(pd.Name, propValue));
                else if (ImplIsString(propType))
                    retVal.Add(new ValueItem(pd.Name, propValue));
                else if (curCtxLvl == null)
                    return null;
                else if ((enumerable = propValue as IEnumerable) != null)
                {
                    ISerializable<IValueSequence> ser = null;

                    Boolean test = propValue.GetSerializationManager(ref ser, context);

                    ArrayList ctxts = new ArrayList();
                    ValueSequence ctxtSeq = new ValueSequence(ctxts);
                    IValueUnit ctxtVu = new ValueUnit();

                    ctxtVu.Add("Items", ctxtSeq);
                    curCtxLvl.Add(pd.Name, ctxtVu);
                    context.Hierarchy.Push(ctxtVu);
                    context.Hierarchy.Push(ctxtSeq);

                    try
                    {
                        retVal.Add(new ValueItem(pd.Name, ser.Serialize(propValue, context)));
                    }
                    finally { context.Hierarchy.Pop(); context.Hierarchy.Pop(); }
                }
                else if (ImplSerializeObject(
                        out subUnit,
                            delegate (IValueUnit vu) { curCtxLvl.Add(pd.Name, vu); },
                            propValue as IValueObjectSerializationContract,
                            context))
                    retVal.Add(new ValueItem(pd.Name, subUnit));
            } // End of foreach (...)

            return retVal;
        } // End of ImplSerialize (...)

        protected virtual IValueUnit ImplSerialize(InterfaceType instance, Object context)
        {
            return null;
        } // End of ImplSerialize (...)

        protected virtual Boolean ImplSerializeEnumerable(
            out ArrayList values,
                IEnumerable objValue,
                IHierarchicalMetadataSerializationContext context)
        {
            values = new ArrayList();

            if (objValue == null) return false;
            if (context == null) return false;

            IValueSequence valueSeq = context.Hierarchy.Peek() as IValueSequence;

            if (valueSeq == null) return false;

            IList ctxts = ((IValueItemBase)valueSeq).Value as IList;

            if (ctxts == null) return false;

            IValueUnit subUnit = null;

            try
            {
                foreach (Object item in objValue)
                    if (ImplIsBuiltinType(item.GetType()))
                    {
                        ctxts.Add(null); values.Add(item);
                    }
                    else if (ImplIsString(item.GetType()))
                    {
                        ctxts.Add(null); values.Add(item);
                    }
                    //else if ( ImplSerializeEnumerable ( retVal, pd.Name, item as IEnumerable, context ) )
                    //    continue;
                    else if (ImplSerializeObject(
                            out subUnit,
                                delegate (IValueUnit vu) { ctxts.Add(vu); },
                                item as IValueObjectSerializationContract,
                                context))
                        values.Add(subUnit);
            }
            finally { context.Hierarchy.Pop(); }

            return true;
        } // End of ImplSerializeEnumerable (...)

        protected virtual Boolean ImplSerializeEnumerable(
            out ArrayList values,
                ValueUnit parentUnit,
                String objKey,
                IEnumerable objValue,
                IHierarchicalMetadataSerializationContext context)
        {
            values = new ArrayList();

            if (parentUnit == null) return false;
            if (String.IsNullOrWhiteSpace(objKey)) return false;
            if (objValue == null) return false;
            if (context == null) return false;

            ArrayList ctxts = new ArrayList();
            ValueUnit ctxtVu = new ValueUnit();
            IValueUnit subUnit = null;

            IValueUnit curCtxLvl = context.Hierarchy.Peek() as IValueUnit;

            if (curCtxLvl == null) return false;

            ctxtVu.Add(String.Empty, ctxts);
            curCtxLvl.Add(objKey, ctxtVu);
            context.Hierarchy.Push(ctxtVu);

            try
            {
                foreach (Object item in objValue)
                    if (ImplIsBuiltinType(item.GetType()))
                    {
                        ctxts.Add(null); values.Add(item);
                    }
                    else if (ImplIsString(item.GetType()))
                    {
                        ctxts.Add(null); values.Add(item);
                    }
                    //else if ( ImplSerializeEnumerable ( retVal, pd.Name, item as IEnumerable, context ) )
                    //    continue;
                    else if (ImplSerializeObject(
                            out subUnit,
                                delegate (IValueUnit vu) { ctxts.Add(vu); },
                                item as IValueObjectSerializationContract,
                                context))
                        values.Add(subUnit);
            }
            finally { context.Hierarchy.Pop(); }

            return false;
        } // End of ImplSerializeEnumerable (...)

        protected virtual Boolean ImplSerializeObject(
            out IValueUnit retVal,
                Action<IValueUnit> addCtxtDlg,
                IValueObjectSerializationContract objValue,
                IHierarchicalMetadataSerializationContext context)
        {
            retVal = null;

            if (addCtxtDlg == null) return false;
            if (objValue == null) return false;
            if (context == null) return false;

            ISerializable<IValueUnit> serMgr = null;
            IValueUnit valueUnit = null;

            //ISerializationManagerProvider<IValueUnit>   serMgrPrvdr = null;
            //serMgrPrvdr = objValue as ISerializationManagerProvider<IValueUnit>;

            //if ( serMgrPrvdr                                                == null ) return false;
            //if ( (serMgr = serMgrPrvdr.GetSerializationManager ( context )) == null ) return false;

            if (!objValue.GetSerializationManager(ref serMgr, context))
                return false;

            addCtxtDlg(valueUnit = new ValueUnit());
            context.Hierarchy.Push(valueUnit);

            try { retVal = serMgr.Serialize(objValue, context); }
            finally { context.Hierarchy.Pop(); }

            return true;
        } // End of ImplSerializeObject (...)

        Type IFactory<InterfaceType>.GetBuildedType()
        {
            throw new NotImplementedException();
        }

        InterfaceType IFactory<InterfaceType>.GetFromJsonString(IJsonContractDocument json)
        {
            throw new NotImplementedException();
        }

        IJsonContractDocument IFactory<InterfaceType>.ExportToJson(InterfaceType instance, string Version)
        {
            throw new NotImplementedException();
        }

        InterfaceType IObjectAllocator<InterfaceType>.GetNew()
        {
            throw new NotImplementedException();
        }
    } // End of Class ValueObjectFactoryBase<...>
} // End of Namespace Sharpframework.EntityModel.Implementation