using System;
using System.Collections.Generic;
using System.Text;

namespace Sharpframework.EntityModel.Implementation
{
    using Sharpframework.EntityModel;


    public abstract class EntityFactory<InterfaceType, FactoryType>
        : EntityFactory<
                InterfaceType,
                EntityFactory<InterfaceType, FactoryType>.DummyInitDto,
                FactoryType>
        //, EntityModel.IFactory<InterfaceType>
    where InterfaceType : IEntity
    where FactoryType   : EntityFactory<InterfaceType, FactoryType>
                        , new ()
    {
        public class DummyInitDto
            : AbsEntityObjectInitDto
        {
            protected override InterfaceType ImplCore => default ( InterfaceType );
        }

        private new DummyInitDto InitDto { get => null; }

        private new InterfaceType GetByDto () => default ( InterfaceType );
        private new InterfaceType ImplInitDto { get => default ( InterfaceType ); }
    } // End of EntityFactoryBase<...>


    public abstract class EntityFactory<InterfaceType, InitDtoType, FactoryType>
        : ValueObjectFactory<InterfaceType, InitDtoType, FactoryType>
    where InterfaceType : IEntity
    where InitDtoType   : EntityFactory<InterfaceType, InitDtoType, FactoryType>.AbsEntityObjectInitDto
                        , new ()
    where FactoryType   : EntityFactory<InterfaceType, InitDtoType, FactoryType>
                        , new ()
    {
        public abstract class AbsEntityObjectInitDto
            : AbsValueObjectInitDto
        {
            private Entity __core;

            private Entity _Core
            {
                get
                {
                    if ( __core == null )
                    {
                        __core = ImplCore as Entity;
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
        } // End of AbsEntityObjectInitDto
    } // End of Class EntityFactory<...>
} // End of Namespace Sharpframework.EntityModel.Implementation