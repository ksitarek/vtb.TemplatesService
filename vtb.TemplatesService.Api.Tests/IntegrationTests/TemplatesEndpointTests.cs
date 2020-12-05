using System;
using NUnit.Framework;
using vtb.TemplatesService.Api.Tests.IntegrationTests.Clients;
using vtb.TemplatesService.DataAccess.Seed;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public class TemplatesEndpointTests : BaseApiEndpointTests
    {
        internal TemplatesTestsClient _client;

        internal readonly Guid Tenant1Id = Templates.Tenant1Id;
        internal readonly Guid Tenant2Id = Templates.Tenant2Id;

        [SetUp]
        public void SetUp()
        {
            _client = new TemplatesTestsClient(_httpClient);
            _httpClient.DefaultRequestHeaders.Clear();
        }
    }
}