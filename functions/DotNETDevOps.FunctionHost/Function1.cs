
using DotNetDevOps.Web;
using DotNETDevOps.Extensions.AzureFunctions;
using DotNETDevOps.FunctionHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

[assembly: WebJobsStartup(typeof(AspNetCoreWebHostStartUp<WebBuilder, Startup>))]


namespace DotNETDevOps.FunctionHost
{
     
    
    public class WebBuilder : IWebHostBuilderExtension<Startup>
    {
        private readonly IHostingEnvironment environment;

        public WebBuilder(IHostingEnvironment environment)
        {
            this.environment = environment;
        }
        public void ConfigureAppConfiguration(WebHostBuilderContext context, IConfigurationBuilder builder)
        {
            builder
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
                .AddUserSecrets("93CD8C24-88BA-4141-9E65-7E78FBDB6D95")
                .AddEnvironmentVariables();
        }

        private void Logging(ILoggingBuilder b)
        {
            //b.AddProvider(new SerilogLoggerProvider(
            //            new LoggerConfiguration()
            //               .MinimumLevel.Verbose()
            //               .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
            //               .Enrich.FromLogContext()
            //                .WriteTo.File($"apptrace.log", buffered: true, flushToDiskInterval: TimeSpan.FromSeconds(30), rollOnFileSizeLimit: true, fileSizeLimitBytes: 1024 * 1024 * 32, rollingInterval: RollingInterval.Hour)
            //               .CreateLogger()));
        }

        public void ConfigureWebHostBuilder(ExecutionContext executionContext, WebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(ConfigureAppConfiguration);
            builder.ConfigureLogging(Logging);

            if (environment.IsDevelopment())
            {
                builder.UseContentRoot(Path.Combine(Directory.GetCurrentDirectory(), "../../../../../src/DotNetDevOps.Web"));
            }
        }
    }

    public class ServerlessApi
    {


        [FunctionName("AspNetCoreHost")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, Route = "{*all}")]HttpRequest req,
            [AspNetCoreRunner(Startup = typeof(Startup))] IAspNetCoreRunner aspNetCoreRunner,
            ExecutionContext executionContext)
        {

            return aspNetCoreRunner.RunAsync(executionContext);
        }
    }
}
