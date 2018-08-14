using System;
using System.Collections.Generic;

using Sharpframework.Core;
using Sharpframework.Serialization.ValueDom;


namespace Sharpframework.EntityModel.Implementation
{
    //using Sharpframework.EntityModel;
    

    public abstract class CompositedIdObject
        : ValueObject
        , ICompositedId
        , ICompositedIdSerializationContract
    {
        private IFieldsValues __mainId;


        protected CompositedIdObject () { _Reset(); }

        protected CompositedIdObject (  IValueUnit                                  valueUnit,
                                        IHierarchicalMetadataSerializationContext   context )
            : base ( valueUnit, context ) { _Reset(); }


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


        protected override void ImplDispose ( Boolean disposing )
        {
            try
            {
                if ( disposing )
                {
                    __mainId.TryDispose ();
                    //if ( __mainId != null ) __mainId.Dispose ();
                }

                _Reset ();
            }
            finally { base.ImplDispose ( disposing ); }
        } // End of ImplDispose (...)


        private void _Reset ()
        {
            __mainId = null;
        } // End of _Reset ()

        //[ Newtonsoft.Json.JsonIgnore ]
        IFieldsValues ICompositedId.IdFields { get { return MyMainId; } }
    } // End of Class CompositedIdObject
} // End of Namespace Sharpframework.EntityModel.Implementation
