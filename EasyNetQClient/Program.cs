using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EasyNetQDIClient
{

    class Program
    {
        static Task Main(string[] args)
        {
            Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            IHost host = CreateHostBuilder(args).Build();

            Console.WriteLine("");

            return host.RunAsync();
        }

        static void RunOptions(Options opts)
        {
            var props = opts.SendMessage;
            //foreach (var prop in props)
            Console.WriteLine("props= {0}", string.Join(",", props));

            //handle options
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
            Console.WriteLine("HandleParseError");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, configurationBuilder) =>
                {
                    IHostEnvironment env = hostContext.HostingEnvironment;
                    configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
                    configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
                .UseSerilog()
                .ConfigureServices((hostContext, services) => { });

    }
}
