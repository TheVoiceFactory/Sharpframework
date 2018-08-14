using System;
using System.IO;

using Xunit;

using Sharpframework.Core.GuidExtension;
using Sharpframework.Serialization.ValueDom;

using Test.EntityModel.Serialization;


namespace Test.Processes.EntityModel.Serialization
{
    public class SerializationAdv
    {
        [ Fact ]
        public void Serialization1 ()
        {
            MainObject                      mainObj         = new MainObject ();
            MainObjectRepoSerMgr            mainObjSerMgr   = MainObjectRepoSerMgr.Singleton;
            RepositorySerializationContext  serCtx          = new RepositorySerializationContext ();
            RepositorySerializationContext  serCtx2         = new RepositorySerializationContext ();
            String                          serialization   = null;
            TextWriter                      tw              = new StringWriter ();

            if ( typeof ( IMainObject ).GetTypeGuid ().ToString () == "e94473ee-f53f-4b7b-95d6-36d78d327e86" )
                System.Diagnostics.Debug.WriteLine ( "Guid verificato" );

            mainObj.DateTimeField       = DateTime.Now;
            mainObj.Field3              = new SubObject ();
            mainObj.Field1              = "String Field \", \"aaa\" : \"bbb\", Insidioso";
            mainObj.Field2              = "Mario";
            mainObj.Field3.SubObjField1 = "Sub 1";
            mainObj.Field3.SubObjField2 = "Sub 2";
            mainObj.Field4              = true;
            mainObj.Field8              = -1234567.890;

            serCtx.SerializeMetadata = true;
            mainObjSerMgr.Serialize ( tw, mainObj, serCtx );

            tw.Flush ();
            tw.Close ();

            serialization = tw.ToString ();

            using ( StringReader sr1 = new StringReader ( serialization ) )
            {
                IMainObject deserResult = mainObjSerMgr.Deserialize ( sr1, serCtx2 );

                IMainObjectSerializationContract ppp = deserResult as IMainObjectSerializationContract;

                ppp.Field1 = "Primo";
                ppp.Field2 = "Coccodrillo";
                ppp.Field4 = false;
                ppp.Field8 = 987.654;

                IValueDomAlignable<IValueUnit, IValueUnit> vDomAligner = deserResult as IValueDomAlignable<IValueUnit, IValueUnit>;

                if ( vDomAligner != null )
                {
                    vDomAligner.Align ();

                    if ( vDomAligner.GetContext ( ref serCtx2 ) )
                    {
                        serCtx2.SerializeData       = true;
                        serCtx2.SerializeMetadata   = true;

                        tw = new StringWriter ();
                        mainObjSerMgr.Serialize ( tw, vDomAligner.Data, serCtx2 );

                        serialization = tw.ToString ();
                    }
                }
            }

            MainObjectFactory mainFact = MainObjectFactory.Singleton;

            mainFact.InitDto.DateTimeField         = DateTime.Now;
            mainFact.InitDto.Field1                = "Test";
            mainFact.InitDto.Field2                = "Mario";
            mainFact.InitDto.Field4                = false;
            mainFact.InitDto.Field8                = 9876.54;
            mainFact.InitDto.Field3.SubObjField1   = "Init Sub 1";
            mainFact.InitDto.Field3.SubObjField2   = "Init Sub 2";

            IMainObject k = mainFact.GetByDto ();
        } // End of Serialization1 ()

        [ Fact ]
        public void Serialization2 ()
        {
            IDerivedObject                  derivedObj      = null;
            DerivedObjectRepoSerMgr         serMgr          = null;
            RepositorySerializationContext  serCtx          = null;
            RepositorySerializationContext  serCtx2         = null;
            String                          serialization   = null;
            IContainedObject                subObj          = null;
            TextWriter                      tw              = null;

            derivedObj  = DerivedObjectFactory.Singleton.GetNew ();
            serMgr      = DerivedObjectRepoSerMgr.Singleton;
            serCtx      = new RepositorySerializationContext ();
            serCtx2     = new RepositorySerializationContext ();
            subObj      = ContainedObjectFactory.Singleton.GetNew ();
            tw          = new StringWriter ();

            derivedObj.SetSubObj ( subObj );
            derivedObj.SetQuantity ( 234.56 );
            derivedObj.SetTimeStamp ( DateTime.Now );
            subObj.SetName ( "Pluto" );

            serCtx.SerializeMetadata = true;

            serMgr.Serialize ( tw, derivedObj, serCtx );

            serialization = tw.ToString ();

            using ( StringReader sr1 = new StringReader ( serialization ) )
            {
                IBaseObject deserResult = serMgr.Deserialize ( sr1, serCtx2 );
            }
        } // End of Serialization2 ()
    } // End of Class SerializationAdv
} // End of Namespace Test.Processes.EntityModel.Serialization
