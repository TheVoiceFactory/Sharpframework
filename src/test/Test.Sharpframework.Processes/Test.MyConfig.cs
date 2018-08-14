using System;
using Xunit;

using Sharpframework.Core.Config;
//using Cedemo.Infrastructure.Messaging.ActiveMQ;

namespace Test.Sharpframework.ProcessesTest
{
   
    public class MyConfig : ConfigObject<MyConfig>
    {
        public string  conf1 { get; set; }
        public string conf2 { get; set; }

        public override MyConfig GetDefault()
        {
            this.conf1 = "conf1";
            this.conf2 = "conf2";
            return this;
        }


        //[Test]
        //public void TestConfigurator()
        //{
        //    TrainingFacility.TrainingFacilityReference a = new TrainingFacility.TrainingFacilityReference(Guid.Empty, "");
        //    Agreement.AgreementReference b = new Agreement.AgreementReference(Guid.Empty, "");

        //    String k = a.EntityName;

        //    k = b.EntityName;

        //    MyConfig conf = Configurator<MyConfig>.Config;
        //    conf.conf1 = "conf3";
        //    conf.conf1 = "conf4";
        //    conf.Store();
        //    MyConfig conf2 = Configurator<MyConfig>.Config;
        //    Assert.Equal(conf.conf1, conf2.conf1);
        //}

    } 

    public class MyConfigImplicit : ConfigObject<MyConfigImplicit>
    {
        public string conf1 { get;  set; } = "conf1";
        public string conf2 { get; private set; } = "conf2";

        public void SetConf2(string s)
        {
            this.conf2 = s;
        }
        //public MyConfigImplicit() { } //NOTA: ecco perche' gli abstract con un costruttore devono essere diciarati new() nei generici , cosi' vengono allertati in compilazione quando override il costruttore
        //public MyConfigImplicit(string conf1, string conf2)
        //{
        //    this.conf1 = conf1;
        //    this.conf2 = conf2;
        //}
    }

   
    public class MyConfigImplicitTest
    {

        [Fact]
        public void TestConfiguratorExt()
        {
            // TrainingFacility.TrainingFacilityReference a = new TrainingFacility.TrainingFacilityReference(Guid.Empty, "");
            // Agreement.AgreementReference b = new Agreement.AgreementReference(Guid.Empty, "");
            string b = "";
            String k = "";// a.EntityName;

            k = b;//.EntityName
            Configurator<MyConfigImplicit>.Delete();
            MyConfigImplicit conf = Configurator<MyConfigImplicit>.Config;
           
            Assert.Equal("conf1", conf.conf1);
            conf.conf1 = "conf3";
            conf.SetConf2 ("conf4");
            conf.Store();
            MyConfigImplicit conf2 = Configurator<MyConfigImplicit>.Config;
            Assert.Equal("conf3", conf.conf1);
            Assert.Equal("conf4", conf.conf2);
        }

    }


}