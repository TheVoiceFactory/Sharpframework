using System;
using System.Linq;
using System.Collections.Generic;

using Sharpframework.Core;


namespace Sharpframework.EntityModel
{
    public interface IStatusDefinition
        : INameProvider
    {
        String StatusName { get; }
        Boolean IsFinal { get; }

        //IEnumerable<IStatusDefinition> From { get; } //forward only
        IEnumerable<IStatusDefinition> To { get; }

        Boolean IsNext(IStatusDefinition next);

    } // End of Interface IStatusDefinition

    public interface IStatusProvider 
    {
        IStatusDefinition Status { get; }
    } // End of Interface IStatusProvider 

    public abstract class StatusDefinitionBase
        : IStatusDefinition 
    {
        public virtual String StatusName { get => Name; }
        public virtual Boolean IsFinal => (int)To.Count() == 0;

        // public abstract IEnumerable<IStatusDefinition> From { get; }
        public abstract IEnumerable<IStatusDefinition> To { get; }

        public Boolean IsNext(IStatusDefinition next)
        {
            return To.Contains(next);
        }

        public virtual String Name => this.GetType().Name;
    } // End of Class StatusDefinitionBase
} // End of Namespace Sharpframework.EntityModel