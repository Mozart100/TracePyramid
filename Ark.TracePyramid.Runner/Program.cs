using GenFu;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using System;
using Serilog.Events;
using System.Reflection;

namespace Ark.TracePyramid.Runner
{
    class Program
    {
        const string OutTemplate = " {Environment} | {Timestamp:yyyy-MM-dd HH:mm:ss} | [{Level}] | {Bookmark} | {Message}{NewLine}{Exception}";
        //const string OutTemplate = " {Environment} | {Timestamp:yyyy-MM-dd HH:mm:ss} | [{Level}] | {Bookmark} | {Message}{NewLine}{Exception}";


        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------


        static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .Enrich.With(new ApplicationDetailsEnricher())
                  .WriteTo.Console(outputTemplate: OutTemplate)
                  .Enrich.WithProperty("Environment", "Developer")
                  .Enrich.FromLogContext()
                  .CreateLogger();

            var list = A.ListOf<Person>(10);

            //Test
            foreach (var item in list)
            {
                logger.ToTraceInfo<ArkIntegrationTest>(x => x.Test).Mark(x => x.Information("line {@item}", item));
                //using (LogContext.PushProperty(SerilogBookmarkBase.LableName, "stam"))
                //{
                //    logger.Information("line {@item}", item);
                //}
            }
            logger.PrintSeperator();

            var cartLog = Log.ForContext<Program>();

            
            foreach (var item in list)
            {
                logger.ToTraceInfo<ArkIntegrationTest>(x => x.XXX)
                    .MarkOn<ArkIntegrationTest>(x => x.XXX)
                    .MarkOn("ssss")
                    .Mark(x => x.Information("line {@item}", item));
                //logger.Information("line {@item}", item);

            }

            logger.PrintSeperator();


            var traceinfo = logger.ToTraceInfo<ArkIntegrationTest>(x => x.Test).MarkOn<ArkIntegrationTest>(x => x.XXX);

            foreach (var item in list)
            {
                traceinfo.Mark(x => x.Information("line {@item}", item));
            }

            traceinfo = traceinfo.MarkOn<ArkIntegrationTest>(x => x.Test3);
            foreach (var item in list)
            {
                traceinfo.Mark(x => x.Information("line {@item}", item));

            }
            Console.ReadKey();
        }
    }

    public class ApplicationDetailsEnricher : ILogEventEnricher
    {

        public ApplicationDetailsEnricher()
        {

        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var applicationAssembly = Assembly.GetEntryAssembly();

            var name    = applicationAssembly.GetName().Name;
            var version = applicationAssembly.GetName().Version;



            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ApplicationName", name));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ApplicationVersion", version));
        }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        //public int Age { get; set; }

        //public string Password { get; set; }


    }

    public class ArkIntegrationTest : SerilogBookmarkBase
    {
        public string Test => "Test";
        public string XXX => "XXX";
        public string Test3 => "TEST3";


        public override string Topic => "My Test";
    }
}