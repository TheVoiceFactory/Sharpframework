using System;
using System.Collections.Generic;
using System.Text;

namespace Sharpframework.EntityModel
{
    public interface IEntityDocument:IEntity,IDocument, IStatusProvider 
    {
        IStatusDefinition InitialStatus { get; }
    }


    public interface IDocument : IDocumentID, IHasEditDate
    {
        IStatusDefinition MoveToStatus<newStatus>() where newStatus : IStatusDefinition, new();
    }
    public interface IDocumentID : IUid { }// IGuidProvider, IVersionProvider { }



    public interface IAggregate<IDType> : IEntity
    { }
    public interface IHasEditDate
    {
        DateTime CreationDate { get; }
        DateTime EditDate { get; }
        IList<string> EditTags { get; }
    }
    public interface IAggregateDocument<IDType> : IAggregate<IDType>, IDocument
    { }
} // End of Namespace Sharpframework.EntityModel
