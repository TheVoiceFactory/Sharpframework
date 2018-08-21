namespace Sharpframework.Propagation.Domains
{


    public abstract class ApplicationServiceBase :DomainServiceBase , IApplicationService
    { }


    public abstract class DomainBase : IDomain
    {
        public abstract string DomainID { get; }
        public string DomainName { get; }
    }

    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        public abstract void Commit();
        public abstract void Rollback();
        public abstract void Start();
    }

}