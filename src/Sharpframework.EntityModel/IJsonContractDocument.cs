using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sharpframework.EntityModel
{



    public interface IJsonContractDocument : IDocumentID, IHasJsonText
    {


    }
    public interface IJsonEntityContractDocument : IJsonContractDocument
    {
    }

    public interface IJsonHierarchyDocument: IJsonContractDocument
    {
    }

    public interface IHasJsonText
    { string Json { get; } }

  
}
