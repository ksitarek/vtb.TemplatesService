using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace vtb.TemplatesService.Api
{
    public static class Program
    {
        private const string DEV_DOCKER = "dev-docker";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    if (hostingContext.HostingEnvironment.EnvironmentName == DEV_DOCKER)
                    {
                        configuration.AddJsonFile(
                            path: "appsettings.Dev-Docker.json",
                            optional: false,
                            reloadOnChange: true);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog((hostingContext, loggerConfig) =>
                {
                    loggerConfig
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .WriteTo.Console()
                        .WriteTo.ApplicationInsights(TelemetryConverter.Events);
                });
    }
}