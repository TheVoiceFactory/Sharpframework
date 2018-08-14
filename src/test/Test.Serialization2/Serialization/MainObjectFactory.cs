using System;
using System.Collections.Generic;
using System.Text;

namespace Test.EntityModel.Serialization
{
    using System;
    using Sharpframework.EntityModel.Implementation;
    using Sharpframework.EntityModel.Implementation.Serialization;
    using Sharpframework.Serialization;
    using Sharpframework.Serialization.ValueDom;
    using Sharpframework.EntityModel;


    public abstract class MainObjectFactory<InterfaceType, InitDtoType, FactoryType>
        : EntityFactory<InterfaceType, InitDtoType, FactoryType>
    where InterfaceType : IMainObject
    where InitDtoType   : MainObjectFactory<InterfaceType, InitDtoType, FactoryType>.AbsMainObjectInitDto
                        , new ()
    where FactoryType   : MainObjectFactory<InterfaceType, InitDtoType, FactoryType>
                        , new ()
    {
        public abstract class AbsMainObjectInitDto
            : AbsEntityObjectInitDto//InitDto
        {
            private MainObject __core;

            private MainObject _Core
            {
                get
                {
                    if ( __core == null )
                    {
                        __core = ImplCore as MainObject;
                        // Init Default Values...
                    }

                    return __core;
                }
            }

            protected override void ImplClear ()
            {
                __core = null;

                base.ImplClear ();
            } // End of ImplClear ()


            public String Field1 { get => _Core.Field1; set => _Core.Field1 = value; }

            public NameType Field2 { get => _Core.Field2; set => _Core.Field2 = value; }

            public SubObjectFactory.SubObjectInitDto Field3
            {
                get
                {
                    _Core.Field3 = SubObjectFactory.Singleton.InitDto.CoreObject;

                    return SubObjectFactory.Singleton.InitDto;
                }
            }

            public Boolean Field4 { get => _Core.Field4; set => _Core.Field4 = value; }

            public Double Field8 { get => _Core.Field8; set => _Core.Field8 = value; }

            public DateTime DateTimeField { get => _Core.DateTimeField; set => _Core.DateTimeField = value; }
        } // End of MainObjectInitDto

        protected class MainObject
            : Entity
            , IMainObject
            , IMainObjectSerializationContract
            , ISerializationManagerProvider<IValueUnit>
        {
            public String Field1 { get; set; }

            public NameType Field2 { get; set; }

            public ISubObject Field3 { get; set; }

            public Boolean Field4 { get; set; }

            //ISubObject2 IMainObject2.Field3 { get => Field3; }
            //ISubObject2 IMainObjectSerializationContract.Field3
            //{ get => Field3; set => Field3 = (SubObject2) value; }

            //public IList<ISubObject> Field5 { get; set; }

            //public IList<Object> Field6 { get; set; }

            //public IList Field6Bis { get; set; }

            //public IDictionary<String, ISubObject> Field7 { get; set; }

            public Double Field8 { get; set; }

            public DateTime DateTimeField { get; set; }

            //public Boolean [] BooleanArray { get; set; }

            //public DateTime [] DateTimeArray { get; set; }

            //public Int32 [] Int32Array { get; set; }

            //public Object [] MixedArray { get; set; }

            //public ISubObject [] SubObjectArray { get; set; }


            public MainObject () : base () { }

            public MainObject (    IValueUnit                                  valueUnit,
                                    IHierarchicalMetadataSerializationContext   context )
                : base ( valueUnit, context ) { }


            //protected override ISerializable<IValueUnit>
            //    ImplGetSerializationManager<SerializationContextType> ( SerializationContextType context )
            //{
            //    return MainObjectFactory2.Singleton;
            //}

            ISerializable<IValueUnit> ISerializationManagerProvider<IValueUnit>.GetSerializationManager<SerializationContextType> ( SerializationContextType context )
            {
                return MainObjectFactory.Singleton;
            }

            protected override Type ImplGetSerializationContract<SerializationContextType> (
                SerializationContextType context )
            {
                return typeof ( IMainObjectSerializationContract );
            }

            Type IValueObjectSerializationContract.GetSerializationContract<SerializationContextType> ( SerializationContextType context )
            {
                throw new NotImplementedException ();
            }
        } // End of Class MainObject
    } // End of Class MainObjectFactory<...>


    public sealed class MainObjectFactory
        : MainObjectFactory<IMainObject, MainObjectFactory.MainObjectInitDto, MainObjectFactory>
    {
        public class MainObjectInitDto 
            : AbsMainObjectInitDto
        {
            private MainObject __core;

            protected override void ImplClear ()
            {
                __core = null;

                base.ImplClear ();
            }

            protected override IMainObject ImplCore
            { get { if ( __core == null ) __core = new MainObject (); return __core; } }
        }


        protected override IMainObject ImplDeserialize<SerializationContextType> (
            IValueUnit                  serialization,
            SerializationContextType    context )
        {
            return new MainObject ( serialization, context as IHierarchicalMetadataSerializationContext );
        }

        protected override IMainObject ImplGetNew () => new MainObject ();
    } // End of Class MainObjectFactory


    public sealed class MainObjectRepoSerMgr
        : JsonValueObjectValueUnitRepositorySerializationManager<
                            IMainObject,
                            MainObjectFactory,
                            MainObjectRepoSerMgr>
    {
    } // End of Class MainObjectRepoSerMgr
} // End of Namespace Test.EntityModel.Serialization
