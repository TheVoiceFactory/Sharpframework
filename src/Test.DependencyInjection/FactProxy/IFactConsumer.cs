using System;
using System.Collections.Generic;
using System.Text;

namespace Sharpframework.Propagation.Facts
{
    public interface IFactConsumer
    {
        Boolean Consume ( IFact fact );
    }
    public interface IFactConsumer<FactType>
        where FactType : IFact
    {
        Boolean Consume ( FactType fact );
    }
}
