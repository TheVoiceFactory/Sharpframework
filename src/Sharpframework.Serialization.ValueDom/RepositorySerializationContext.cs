
namespace Sharpframework.Serialization.ValueDom
{
    public sealed class RepositorySerializationContext
        : ValueDomMetadataSerializationContext<RepositorySerializationContext>
        , IRepositorySerializationContext
    {
        public RepositorySerializationContext ( IValueUnit metadata ) : base ( metadata ) { }
        public RepositorySerializationContext () : base ( new ValueUnit () ) { }
    } // End of Class RepositorySerializationContext
} // End of Namespace Sharpframework.Serialization.ValueDom
