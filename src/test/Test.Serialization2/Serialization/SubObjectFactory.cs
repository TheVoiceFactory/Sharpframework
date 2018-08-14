using System;

using Sharpframework.Serialization;
using Sharpframework.Serialization.ValueDom;
using Sharpframework.EntityModel.Implementation;
using Sharpframework.EntityModel.Implementation.Serialization;


namespace Test.EntityModel.Serialization
{
    public abstract class SubObjectFactory<InterfaceType, InitDtoType, FactoryType>
        : ValueObjectFactoryBase<InterfaceType, InitDtoType, FactoryType>
    where InterfaceType : ISubObject
    where InitDtoType   : SubObjectFactory<InterfaceType, InitDtoType, FactoryType>.AbsSubObjectInitDto
                        , new ()
    where FactoryType   : SubObjectFactory<InterfaceType, InitDtoType, FactoryType>
                        , new ()
    {
        public abstract class AbsSubObjectInitDto
            : AbsValueObjectInitDto//InitDto
        {
            private SubObject __core;

            private SubObject _Core
            {
                get
                {
                    if ( __core == null )
                    {
                        __core = ImplCore as SubObject;
                        // Init Default Values...
                    }

                    return __core;
                }
            }

            protected override void ImplClear ()
            {
                __core = null;

                base.ImplClear ();
            }


            public String SubObjField1 { get => _Core.SubObjField1; set => _Core.SubObjField1 = value; }
            public String SubObjField2 { get => _Core.SubObjField2; set => _Core.SubObjField2 = value; }


            //protected override Type ImplGetSerializationContract<SerializationContextType> ( SerializationContextType context )
            //    => throw new NotImplementedException ();
        } // End of AbsSubObjectInitDto

        protected class SubObject
            : Entity
            , ISubObject
            , ISubObjectSerializationContract
            , ISerializationManagerProvider<IValueUnit>
        {
            public String SubObjField1 { get; set; }
            public String SubObjField2 { get; set; }

            public SubObject () : base () { }

            public SubObject (  IValueUnit                                  valueUnit,
                                IHierarchicalMetadataSerializationContext   context )
                : base ( valueUnit, context ) { }


            public ISerializable<IValueUnit>
                GetSerializationManager<SerializationContextType> ( SerializationContextType context )
                    where SerializationContextType : ISerializationContext
                        => ImplGetSerializationManager ( context );

            protected virtual ISerializable<IValueUnit>
                ImplGetSerializationManager<SerializationContextType> ( SerializationContextType context )
            where SerializationContextType : ISerializationContext
            {
                return SubObjectFactory.Singleton;
            }

            protected override Type ImplGetSerializationContract<SerializationContextType> (
                SerializationContextType context )
            {
                return typeof ( ISubObjectSerializationContract );
            }
        } // End of Class SubObject
    } // End of Class SubObjectFactory<...>


    public sealed class SubObjectFactory
        : SubObjectFactory<ISubObject, SubObjectFactory.SubObjectInitDto, SubObjectFactory>
    {
        public class SubObjectInitDto 
            : AbsSubObjectInitDto
        {
            private SubObject __core;

            protected override void ImplClear ()
            {
                __core = null;

                base.ImplClear ();
            }

            protected override ISubObject ImplCore
            { get { if ( __core == null ) __core = new SubObject (); return __core; } }
        }


        protected override ISubObject ImplDeserialize<SerializationContextType> (
            IValueUnit                  serialization,
            SerializationContextType    context )
        {
            return new SubObject ( serialization, context as IHierarchicalMetadataSerializationContext );
        }

        protected override ISubObject ImplGetNew () => new SubObject ();
    } // End of Class SubObjectFactory


    public sealed class SubObjectRepoSerMgr
        : JsonValueObjectValueUnitRepositorySerializationManager<
                            ISubObject,
                            SubObjectFactory,
                            SubObjectRepoSerMgr>
    {
    } // End of Class SubObjectRepoSerMgr
} // End of Namespace Test.EntityModel.Serialization
