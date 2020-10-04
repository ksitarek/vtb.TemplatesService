using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic;
using vtb.TemplatesService.DataAccess.Repositories;

namespace vtb.TemplatesService.Service
{
    internal static class Program
    {
        private static IConfigurationRoot Configuration;
        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<CliOptions>(args)
                    .WithParsedAsync(Run);

            Console.CancelKeyPress += (sender, args) => _cancellationTokenSource.Cancel();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // configuration
            services.Configure<BusConfiguration>(Configuration.GetSection("Bus"));
            services.Configure<MongoDbConfiguration>(Configuration.GetSection("MongoDb"));

            // consumers

            // repositories
            services.AddTransient<ITemplateKindsRepository, TemplateKindsRepository>();

            // mongo db
            services.AddSingleton((sp) =>
            {
                var mongoOptions = sp.GetService<IOptions<MongoDbConfiguration>>();
                return new MongoClient(mongoOptions.Value.ConnectionString);
            });

            services.AddTransient((sp) =>
            {
                var mongoOptions = sp.GetService<IOptions<MongoDbConfiguration>>();
                var mongoClient = sp.GetService<MongoClient>();
                var database = mongoClient.GetDatabase(mongoOptions.Value.DatabaseName);

                return database;
            });
        }

        private static async Task Run(CliOptions cliOptions)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<Application>();

            ConfigureConfiguration(serviceCollection, cliOptions.Configuration);
            ConfigureLogging(serviceCollection);
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            await serviceProvider.GetService<Application>().RunAsync(_cancellationTokenSource.Token);
        }

        private static void ConfigureLogging(IServiceCollection services)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();

                builder.AddConfiguration(Configuration.GetSection("Logging"));
            });

            services.AddSingleton(loggerFactory);
            services.AddLogging();
        }

        private static void ConfigureConfiguration(IServiceCollection services, string configurationOption)
        {
            var configurationBuilder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", false)
                            .AddUserSecrets(typeof(Program).Assembly);

            configurationBuilder.AddJsonFile($"appsettings.{configurationOption}.json", configurationOption == "Release");

            Configuration = configurationBuilder.Build();
            services.AddOptions();
        }

        private class CliOptions
        {
            [Option('c', "configuration", Default = "Release", HelpText = "Chooses which configuration will be loaded.")]
            public string Configuration { get; set; }
        }
    }
}