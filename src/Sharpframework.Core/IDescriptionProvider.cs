using System;


namespace Sharpframework.Core
{
   // TODO: descritpion multilanguage, il comportamento dovrebbe implementare la Descrtiption di defualt (implementation based) e la Description ( languagecode)
   // TODO: aggiungere una short descritpion multilanguage

    public interface IDescriptionProvider
    {

        String Description { get; }
    } // End of Interface IDescriptionProvider
} // End of Namespace Sharpframework.Core
