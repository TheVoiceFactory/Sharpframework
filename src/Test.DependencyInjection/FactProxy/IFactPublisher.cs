
namespace Sharpframework.Propagation.Facts
{
    public interface IFactPublisher
    {
        void Publish<FactType> ( FactType fact )
            where FactType : IFact;
    }
}
