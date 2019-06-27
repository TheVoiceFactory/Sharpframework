using System;

using Sharpframework.EntityModel;
using Sharpframework.EntityModel.Implementation;
using Sharpframework.Propagation.Facts;


namespace Test.DependencyInjection.FactProxy
{
    public delegate TestFactClass TestFactClassAllocatorDlg ( IServiceProvider sp );

    public class SpItem
    {
        public Type ContractType { get; set; }
        public Type ImplType { get; set; }
        public SpAllocatorDelegate SpAllocatorDlg { get; set; }
    }

    public delegate Object SpAllocatorDelegate ( IServiceProvider sp );

    public static class SpAllocators
    {
        public static SpAllocatorDelegate GetSpAllocator<TargetType> ()
            where TargetType : class
        {
            if ( typeof ( TargetType ) == typeof ( TestFactClass ) ) return TestFactClass.SpAllocatorDlg;

            return null;
        }
    }

    public interface ITestFact
        : IEntity// IUid
    {
	    public String Name { get; }

	    public String Surname { get; }

        [ProvideExecVerbFact]
        public Boolean ChangeName ( String newName );

        [ProvideExecVerbFact]
        public Boolean ChangeSurname ( String newSurname );

    }

    public class TestFactClass
        : Entity
        , ITestFact
    {
        public static readonly SpAllocatorDelegate SpAllocatorDlg;

        public readonly IServiceProvider Sp;


        static TestFactClass ()
        {
            SpAllocatorDlg = new SpAllocatorDelegate ( ( sp ) => new TestFactClass ( sp ) );
        }


        public TestFactClass () : this ( null, null ) { }

        public TestFactClass ( IServiceProvider sp ) : this ( sp, null ) { }

        public TestFactClass ( IUid uid ) : this ( null, uid ) { }

        public TestFactClass ( IServiceProvider sp, IUid uid )
            : base ( uid == null ? new UId () : uid )
        {
            Sp = sp;
        }


	    public String Name { get; private set; }

	    public String Surname { get; private set; }

        //public String Version { get; private set; }

        //public Guid Guid { get; private set; }

        public Boolean ChangeName ( String newName )
	    { Name = newName; return true; }

	    public Boolean ChangeSurname ( String newSurname )
	    { Surname = newSurname; return true; }


        protected override Type ImplContractType
            => throw new NotImplementedException ();
    }
}
