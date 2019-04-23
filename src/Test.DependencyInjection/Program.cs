using System;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;


namespace Test.DependencyInjection
{
    class Program
    {
        public interface IMyService
        {
            String StringDecorator ( String target );
        }

        public interface IMyHostedService
            : IHostedService
        {
            String StringService ( String data );
        }
        private class MyService : IMyService
        {
            public String StringDecorator ( String target )
            {
                return "$$$" + target + "@@@";
            }
        }

        private class MyHostedService
            : BackgroundService, IMyHostedService
        {
            private IMyService          __mySer;
            private IServiceProvider    __sp;

            public MyHostedService  ( IServiceProvider sp )
            {
                __sp = sp;

                if (sp == null) return;

                __mySer = sp.GetService<IMyService> ();
            }
            public override Task StartAsync ( CancellationToken cancellationToken )
            {
                return base.StartAsync ( cancellationToken );
            }

            public override Task StopAsync ( CancellationToken cancellationToken )
            {
                return base.StopAsync ( cancellationToken );
            }
            protected async override Task ExecuteAsync ( CancellationToken stoppingToken )
            {
                async Task<String> ConsoleReadLine ( CancellationToken stoppingToken )
                    => await Task<String>.Run ( () => Console.ReadLine (), stoppingToken );

                String lineStr = String.Empty;

                while ( (lineStr = await ConsoleReadLine ( stoppingToken )) != null )
                {
                    Console.WriteLine ( "Echo: {0}", lineStr );
                    Console.WriteLine ( "Local Computation Result: {0}", StringService ( lineStr ) );
                    Console.WriteLine ( "StringDecorator Result: {0}", __mySer.StringDecorator ( lineStr ) );
                }
            }

            public String StringService ( String data )
            {
                return "!!!" + data + "%%%";
            }
        }

        static async Task Main ( string [] args )
        {
            IHostBuilder hb = new HostBuilder ()
                .ConfigureServices((hostContext, services) =>
                {
                    //services.AddHostedService<MyHostedService> ();
                    services.AddSingleton<IHostedService, MyHostedService> ();
                    services.AddTransient<IMyService, MyService>();
                });

            Console.WriteLine("Hello World!");

            //Task tsk = hb.RunConsoleAsync ();
            await hb.RunConsoleAsync ();
        }
    }
}
