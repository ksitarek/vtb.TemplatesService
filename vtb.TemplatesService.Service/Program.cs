using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using vtb.Auth.Tenant;
using vtb.Auth.Tenant.MessagingFilters;
using vtb.TemplatesService.BusinessLogic;
using vtb.TemplatesService.BusinessLogic.Managers;
using vtb.TemplatesService.BusinessLogic.RequestHandlers;
using vtb.TemplatesService.Contracts.Requests;
using vtb.TemplatesService.DataAccess.Repositories;
using vtb.Utils.Extensions;

namespace vtb.TemplatesService.Service
{
    internal class Program
    {
        private const string DEV_DOCKER = "dev-docker";

        private static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.AddJsonFile(
                        path: "appsettings.json",
                        optional: false,
                        reloadOnChange: true);

                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == DEV_DOCKER)
                    {
                        configuration.AddJsonFile(
                            path: "appsettings.Dev-Docker.json",
                            optional: false,
                            reloadOnChange: true);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddVtbPrerequisites();
                    services.AddTenantProvider();

                    // configuration
                    services.Configure<BusConfiguration>(hostContext.Configuration.GetSection("RabbitMq"));
                    services.Configure<MongoDbConfiguration>(hostContext.Configuration.GetSection("MongoDb"));

                    // managers
                    services.AddTransient<ITemplateKindManager, TemplateKindManager>();
                    services.AddTransient<ITemplateManager, TemplateManager>();

                    // repositories
                    services.AddTransient<ITemplateKindsRepository, TemplateKindsRepository>();
                    services.AddTransient<ITemplatesRepository, TemplatesRepository>();

                    ConfigureMongodb(services);
                    ConfigureMassTransit(services, hostContext);

                    services.AddHostedService<TemplatesHostedService>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });

            await builder.RunConsoleAsync();
        }

        private static void ConfigureMassTransit(IServiceCollection services, HostBuilderContext hostContext)
        {
            services.AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();

                cfg.AddConsumer<GetDefaultTemplateRequestHandler>();
                cfg.AddRequestClient<GetDefaultTemplateRequest>();

                cfg.UsingRabbitMq((busRegistrationContext, busFactoryConfigurator) =>
                {
                    var rmqConfig = new BusConfiguration();
                    hostContext.Configuration.GetSection("RabbitMq").Bind(rmqConfig);

                    busFactoryConfigurator.UseConsumeFilter(typeof(TenantConsumeFilter<>), busRegistrationContext);

                    busFactoryConfigurator.Host(rmqConfig.Host, rmqConfig.VirtualHost, h =>
                    {
                        h.Username(rmqConfig.UserName);
                        h.Password(rmqConfig.Password);
                    });

                    busFactoryConfigurator.ConfigureEndpoints(busRegistrationContext);
                });
            });
        }

        private static void ConfigureMongodb(IServiceCollection services)
        {
            services.AddSingleton((sp) =>
            {
                var mongodbOptions = sp.GetRequiredService<IOptions<MongoDbConfiguration>>();
                return new MongoClient(mongodbOptions.Value.ConnectionString);
            });

            services.AddTransient((sp) =>
            {
                var mongodbConfig = sp.GetRequiredService<IOptions<MongoDbConfiguration>>();
                var mongodbClient = sp.GetRequiredService<MongoClient>();
                var database = mongodbClient.GetDatabase(mongodbConfig.Value.DatabaseName);

                return database;
            });
        }
    }
}