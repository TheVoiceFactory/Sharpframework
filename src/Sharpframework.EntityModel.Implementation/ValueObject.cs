using System;

using Sharpframework.Core;


namespace Sharpframework.EntityModel.Implementation
{
    public abstract class ValueObjectBase<SerializationContractType>
        : InfrastructureCoreObject
        , IValueObject
    where SerializationContractType : IValueObjectContract
    {
        public ValueObjectBase () { _Reset (); }


        public Type ContractType { get { return ImplContractType; } }// set; }


        protected abstract Type ImplContractType { get; }

        protected override Type ImplFieldsViewType
        { get { return GetType ().GetInterface ( typeof ( SerializationContractType ).Name ); } }


        private void _Reset()
        {
            //__fields = null;
        } // End of _Reset ()
    } // End of Class ValueObjectBase<...>


    public abstract class ValueObject<SerializationContractType>
        : ValueObjectBase<SerializationContractType>
    where SerializationContractType : IValueObjectContract
    {
        protected override Type ImplContractType => typeof ( SerializationContractType );
    } // End of Class ValueObject<...>


    public abstract class ValueObject
        : ValueObjectBase<IValueObjectContract>
    {
    } // End of Class ValueObject

    //public abstract class ValueObject
    //  : ValueObjectUntyped

    //{
    //}

    //public class ValueObject<SerializationContractType>
    //   : ValueObjectUntyped

    //    where SerializationContractType : IValueObjectContract
    //{


    //    protected override Type ImplContractType
    //    { get { return typeof(SerializationContractType); } }


    //}

    //public abstract class ValueObjectUntyped
    //    : CoreObject
    //    , IValueObject

    //{
    //    public Type ContractType { get { return ImplContractType; } }// set; }

    //    protected abstract Type ImplContractType { get; }

    //    protected override Type ImplFieldsViewType
    //    { get { return GetType().GetTypeInfo().GetInterface(ImplContractType.Name); } }
    //}

    //public abstract class ValueObject
    //  : ValueObjectUntyped

    //{
    //}

    //public abstract class ValueObject_old
    //   : CoreObject
    //   , IValueObject

    //{
    //    public ValueObject_old()
    //    {
    //        _Reset();
    //    }

    //    public Type ContractType { get { return ImplContractType; } }// set; }

    //    protected abstract Type ImplContractType { get; }

    //    protected override Type ImplFieldsViewType
    //    { get { return GetType().GetTypeInfo().GetInterface((ContractType).Name); } }

    //    private void _Reset()
    //    {
    //        //__fields = null;
    //    } // End of _Reset ()
    //} // End of Class ValueObject
} // End of Namespace Sharpframework.Domains.Shared.Model
