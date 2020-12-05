using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mongo2Go;
using Moq;
using System;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    internal class ApiFactory : WebApplicationFactory<Startup>
    {
        private MongoDbRunner _runner;

        internal static readonly DateTimeOffset UtcNow = DateTimeOffset.UtcNow;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            _runner = MongoDbRunner.Start();

            builder.UseEnvironment("Release");

            builder.UseSetting("MongoDb:ConnectionString", _runner.ConnectionString);
            builder.UseSetting("MongoDb:DatabaseName", Guid.NewGuid().ToString());

            builder.UseSetting("Jwt:Secret", new string('#', 1000));
            builder.UseSetting("Jwt:JwtTokenLifespan", TimeSpan.FromMinutes(5).ToString());
            builder.UseSetting("Jwt:RefreshTokenLifespan", TimeSpan.FromHours(7).ToString());

            builder.ConfigureTestServices((services) =>
            {
                var systemClockMock = new Mock<ISystemClock>();
                systemClockMock.Setup(x => x.UtcNow).Returns(UtcNow);

                var sd = new ServiceDescriptor(typeof(ISystemClock), _ => systemClockMock.Object, ServiceLifetime.Transient);
                services.Replace(sd);
            });
        }

        protected override void Dispose(bool disposing)
        {
            _runner?.Dispose();
            base.Dispose(disposing);
        }
    }
}