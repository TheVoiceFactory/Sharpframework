using System;
using System.Collections.Generic;

using Sharpframework.Core;


namespace Sharpframework.EntityModel.Implementation
{
    public abstract class CompositedIdObjectBase<SerializationContractType>
        : ValueObjectBase<SerializationContractType>
        , ICompositedId
    where SerializationContractType : ICompositedIdContract
    {
        protected CompositedIdObjectBase () { _Reset(); }

        private IFieldsValues __mainId;


        protected virtual IEnumerable<String> MyMainIdFieldsNames { get { return null; } }

        protected virtual IEnumerable<Object> MyMainIdFieldsValues { get { return null; } }

        protected IFieldsValues MyMainId
        {
            get
            {
                if (__mainId == null)
                    __mainId = MyFieldsValuesFactory(
                                    delegate { return MyMainIdFieldsNames; },
                                    delegate { return MyMainIdFieldsValues; },
                                    this );

                return __mainId;
            }
        }


        protected override void ImplDispose(Boolean disposing)
        {
            try
            {
                if (disposing)
                {
                    __mainId.TryDispose ();
                    //if (__mainId != null) __mainId.Dispose();
                }

                _Reset();
            }
            finally { base.ImplDispose(disposing); }
        } // End of ImplDispose (...)


        private void _Reset()
        {
            __mainId = null;
        } // End of _Reset ()

        [Newtonsoft.Json.JsonIgnore]
        IFieldsValues ICompositedId.IdFields { get { return MyMainId; } }
    } // End of Class CompositedIdObjectBase<...>


    public abstract class CompositedIdObject<SerializationContractType>
        : CompositedIdObjectBase<SerializationContractType>
    where SerializationContractType : ICompositedIdContract
    {
        protected override Type ImplContractType => typeof ( SerializationContractType );
    } // End of Class CompositedIdObject<...>


    public abstract class CompositedIdObject
        : CompositedIdObjectBase<ICompositedIdContract>
    {
    } // End of Class CompositedIdObject

    // public abstract class CompositedIdObject<SerializationContractType>
    //    : CompositedIdObjectUntyped

    //where SerializationContractType : ICompositedIdContract

    // {
    //     protected override Type ImplContractType { get { return typeof(SerializationContractType); } }
    // }

    // public abstract class CompositedIdObjectUntyped
    //     : ValueObjectUntyped
    //     , ICompositedId

    // {
    //     protected CompositedIdObjectUntyped()
    //     {
    //         _Reset();
    //     }

    //     private IFieldsValues __mainId;

    //     protected virtual IEnumerable<String> MyMainIdFieldsNames { get { return null; } }

    //     protected virtual IEnumerable<Object> MyMainIdFieldsValues { get { return null; } }

    //     protected IFieldsValues MyMainId
    //     {
    //         get
    //         {
    //             if (__mainId == null)
    //                 __mainId = MyFieldsValuesFactory(
    //                                 delegate { return MyMainIdFieldsNames; },
    //                                 delegate { return MyMainIdFieldsValues; },
    //                                 this);

    //             return __mainId;
    //         }
    //     }

    //     protected override void ImplDispose(Boolean disposing)
    //     {
    //         try
    //         {
    //             if (disposing)
    //             {
    //                 __mainId.TryDispose();
    //                 //if (__mainId != null) __mainId.Dispose();
    //             }

    //             _Reset();
    //         }
    //         finally { base.ImplDispose(disposing); }
    //     } // End of ImplDispose (...)

    //     private void _Reset()
    //     {
    //         __mainId = null;
    //     } // End of _Reset ()

    //     [Newtonsoft.Json.JsonIgnore]
    //     IFieldsValues ICompositedId.IdFields { get { return MyMainId; } }
    // } // End of Class CompositedIdObject

    // public abstract class CompositedIdObject
    //   : CompositedIdObjectUntyped
    //   , ICompositedId
    // { }
} // End of Namespace Sharpframework.EntityModel.Implementation
