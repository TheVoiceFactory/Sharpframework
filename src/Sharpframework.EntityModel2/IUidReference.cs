using System;


namespace Sharpframework.EntityModel
{
    public interface IUidReference
        : IUid
        //, IReferenceTagsProvider
    {
        String EntityName { get; }
    } // End of Interface IUidReference
} // End of Namespace Sharpframework.Domains.Shared.Framework
