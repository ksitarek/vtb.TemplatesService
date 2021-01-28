using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Mongo2Go;
using Moq;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    internal class ApiFactory : WebApplicationFactory<Startup>
    {
        private MongoDbRunner _runner;

        internal static readonly DateTimeOffset UtcNow = DateTimeOffset.UtcNow;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            _runner = MongoDbRunner.Start();

            builder.UseEnvironment("Release");

            builder.ConfigureAppConfiguration((ctx, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>()
                {
                    ["MongoDb:ConnectionString"] = _runner.ConnectionString,
                    ["MongoDb:DatabaseName"] = Guid.NewGuid().ToString(),

                    ["Jwt:Secret"] = new string('#', 1000),
                    ["Jwt:JwtTokenLifespan"] = TimeSpan.FromMinutes(5).ToString(),
                    ["Jwt:RefreshTokenLifespan"] = TimeSpan.FromMinutes(7).ToString(),
                });
            });

            builder.ConfigureTestServices((services) =>
            {
                var systemClockMock = new Mock<ISystemClock>();
                systemClockMock.Setup(x => x.UtcNow).Returns(UtcNow);

                var sd = new ServiceDescriptor(typeof(ISystemClock), _ => systemClockMock.Object, ServiceLifetime.Transient);
                services.Replace(sd);
            });

            base.ConfigureWebHost(builder);
        }

        protected override void Dispose(bool disposing)
        {
            _runner?.Dispose();
            base.Dispose(disposing);
        }
    }
}