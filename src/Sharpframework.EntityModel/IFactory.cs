using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sharpframework.EntityModel
{

    //TODO split interface : isolate json behaviour (IFactory generic)
    public interface IFactory<TBuildedType>
        : IObjectAllocator<TBuildedType>
        , IDtoAllocator<TBuildedType>
    {
        Type GetBuildedType();
        TBuildedType GetFromJsonString(IJsonContractDocument json);
        IJsonContractDocument ExportToJson(TBuildedType instance, string Version = "0");

    }

    public interface IObjectAllocator<ObjectType>
    {
        ObjectType GetNew();
    }
    public interface IDtoAllocator<AllocatedType>
    {
        AllocatedType GetByDto();
    } // End of IDtoAllocator<...>
}