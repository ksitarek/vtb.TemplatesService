using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic;
using vtb.TemplatesService.BusinessLogic.Managers;
using vtb.TemplatesService.BusinessLogic.RequestHandlers;
using vtb.TemplatesService.Contracts.Requests;
using vtb.TemplatesService.DataAccess.Repositories;

namespace vtb.TemplatesService.Service
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", true);
                })
                .ConfigureServices((hostContext, services) =>
                {
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
                cfg.AddRequestClient<IGetDefaultTemplateRequest>();

                cfg.UsingRabbitMq((x, y) =>
                {
                    var rmqConfig = hostContext.Configuration.GetSection("RabbitMq").Get<BusConfiguration>();

                    y.Host(rmqConfig.Host, rmqConfig.VirtualHost, h =>
                    {
                        h.Username(rmqConfig.UserName);
                        h.Password(rmqConfig.Password);
                    });

                    y.ConfigureEndpoints(x);
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