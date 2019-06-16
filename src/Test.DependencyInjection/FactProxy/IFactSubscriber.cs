﻿using System;


namespace Sharpframework.Propagation.Facts
{
    public interface IFactSubscriber
    {
        Boolean Subscribe ( IFactConsumer factConsumer );

        Boolean Subscribe<FactType> ( IFactConsumer<FactType> factConsumer )
            where FactType : IFact;

        Boolean UnSubscribe ( Object factConsumer );

        Boolean UnSubscribe<FactType> ( IFactConsumer<FactType> factConsumer )
            where FactType : IFact;
    }
}
