using System;
using System.Collections.Generic;
using System.Text;

namespace Sharpframework.Propagation.Facts
{
    public interface IFactConsumer
    {
        void Consume ( IFact fact );
    }
    public interface IFactConsumer<FactType>
        where FactType : IFact
    {
        void Consume ( FactType fact );
    }
}
