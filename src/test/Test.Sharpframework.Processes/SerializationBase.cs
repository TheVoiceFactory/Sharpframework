using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

using Sharpframework.Core.Config;
using Sharpframework.Serialization.ValueDom;

using System.Linq;

//using //Sharpframework.Infrastructure;
//using Sharpframework.Domains.Framework;
//using Sharpframework.Serialization.Json;
//using Sharpframework.Implementations.Domains;
//using Test.Demo;
//using Test.Demo.TestPerson;
//using Test.Demo.DBSerialization;
//using Sharpframework.Core;

using Sharpframework.Core.GuidExtension;

using Test.EntityModel.Serialization;


namespace Test.Processes.EntityModel.Serialization
{
    public class SerializationBase
    {
        [ Fact ]
        public void Serialization1 ()
        {
            IMainObject                     mainObj         = MainObjectFactory.Singleton.GetNew ();
            MainObjectRepoSerMgr            mainObjSerMgr   = MainObjectRepoSerMgr.Singleton;
            RepositorySerializationContext  serCtx          = new RepositorySerializationContext ();
            RepositorySerializationContext  serCtx2         = new RepositorySerializationContext ();
            ISubObject                      subObject       = null;

            if ( typeof ( IMainObject ).GetTypeGuid ().ToString () == "e94473ee-f53f-4b7b-95d6-36d78d327e86" )
                System.Diagnostics.Debug.WriteLine ( "Guid verificato" );

            mainObj.DateTimeField       = DateTime.Now;
            mainObj.Field3              = SubObjectFactory.Singleton.GetNew ();
            mainObj.Field1              = "String Field \", \"aaa\" : \"bbb\", Insidioso";
            mainObj.Field2              = "Mario";
            mainObj.Field3.SubObjField1 = "Sub 1";
            mainObj.Field3.SubObjField2 = "Sub 2";
            mainObj.Field4              = true;
            mainObj.Field5              = new List<ISubObject> ();
            mainObj.Field6              = new List<Object> ();
            mainObj.Field6Bis           = (IList) mainObj.Field6;
            mainObj.Field7              = new Dictionary<String, ISubObject> ();
            mainObj.Field8              = -1234567.890;
            mainObj.BooleanArray        = new Boolean [] { true, false, true };
            mainObj.DateTimeArray       = new DateTime [] { DateTime.Now.AddDays ( -1 ),
                                                            DateTime.Now,
                                                            DateTime.Now.AddDays ( 1 ) };
            mainObj.Int32Array          = new Int32 [] { 123, 456, 789 };
            mainObj.MixedArray          = new Object [] { 123, true, DateTime.Now };
            mainObj.SubObjectArray      = new ISubObject [ 2 ];

            subObject                   = SubObjectFactory.Singleton.GetNew ();
            subObject.SubObjField1      = "Collection Item 1 Field 1";
            subObject.SubObjField2      = "Collection Item 1 Field 2";
            mainObj.Field5.Add ( subObject );
            mainObj.Field6.Add ( subObject );
            mainObj.SubObjectArray [ 0 ] = subObject;

            subObject                   = SubObjectFactory.Singleton.GetNew ();
            subObject.SubObjField1      = "Collection Item 2 Field 1";
            subObject.SubObjField2      = "Collection Item 2 Field 2";
            mainObj.Field5.Add ( subObject );
            mainObj.SubObjectArray [ 1 ] = subObject;

            mainObj.Field6.Add ( "Ciccio Pasticcio" );
            mainObj.Field6.Add ( subObject );

            mainObj.Field7.Add ( "Dictionary Test", subObject );

            //dynamic test = MainObjectFactory.Singleton.ExportToJson ( mainObj );

            TextWriter tw = new StringWriter ();

            serCtx.SerializeMetadata = true;
            mainObjSerMgr.Serialize ( tw, mainObj, serCtx );

            tw.Flush ();
            tw.Close ();

            String serialization = tw.ToString ();

            using ( StringReader sr1 = new StringReader ( serialization ) )
            {
                IMainObject deserResult = mainObjSerMgr.Deserialize ( sr1, serCtx2 );
            }
        } // End of Serialization1 ()
    } // End of Class SerializationBase
} // End of Namespace Test.Processes.EntityModel.Serialization
