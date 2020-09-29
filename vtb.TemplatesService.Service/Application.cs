using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic;

namespace vtb.TemplatesService.Service
{
    internal class Application
    {
        private readonly ILogger<Application> _logger;
        private readonly IServiceProvider _serviceProvider;
        private BusConfiguration _busConfig;

        public Application(
            ILogger<Application> logger,
            IServiceProvider serviceProvider,
            IOptions<BusConfiguration> options)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _busConfig = options.Value;
        }

        internal async Task RunAsync(CancellationToken cancellationToken)
        {
            var busControl = default(IBusControl);

            if (_busConfig.InMemory)
            {
                _logger.LogInformation("Starting bus using InMemory.");
                busControl = Bus.Factory.CreateUsingInMemory(c =>
                {
                    SetUpEndpoints(c);
                });
            }
            else
            {
                _logger.LogInformation("Starting bus using RabbitMq.");
                busControl = Bus.Factory.CreateUsingRabbitMq(c =>
                {
                    c.Host(_busConfig.Host, h =>
                    {
                        h.Username(_busConfig.UserName);
                        h.Password(_busConfig.Password);
                    });

                    SetUpEndpoints(c);
                });
            }

            var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellationTokenSource.Token);

            try
            {
                await busControl.StartAsync(linkedTokenSource.Token);
            }
            catch (Exception e)
            {
                var message = e.Message;

                var isCausedByCancellationException = e.InnerException != null && e.InnerException is OperationCanceledException;
                if (isCausedByCancellationException && timeoutCancellationTokenSource.Token.IsCancellationRequested)
                {
                    message = "Connection Timed Out.";
                }

                _logger.LogError(e, message);
                throw;
            }

            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
            while (!cancellationToken.IsCancellationRequested);

            await busControl.StopAsync(CancellationToken.None);
            Console.WriteLine("Bye.");
        }

        private void SetUpEndpoints(IBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("template-kinds", ep =>
            {
                //ep.Consumer(() => _serviceProvider.GetService<GetTemplateKindsConsumer>());
            });
        }
    }
}