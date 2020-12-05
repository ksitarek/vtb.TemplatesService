using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RESTFulSense.Exceptions;
using vtb.TemplatesService.Api.Tests.IntegrationTests.ExpectedResults;
using vtb.TemplatesService.DataAccess.Seed;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public class TemplatesEndpointTests_Get : TemplatesEndpointTests
    {
        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.Get(Templates.Tenant1FirstInvoiceTemplate.TemplateId));
        }

        [Test]
        public void Will_Return_404_For_Other_Tenant_Entities()
        {
            Authorize(Guid.NewGuid(), Tenant2Id, Array.Empty<string>(), Array.Empty<string>());
            Assert.ThrowsAsync<HttpResponseNotFoundException>(async () =>
                await _client.Get(Templates.Tenant1FirstInvoiceTemplate.TemplateId));
        }

        [Test]
        public async Task Will_Return_TemplateDetails()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), Array.Empty<string>());

            var expectedDetails = ExpectedTemplateDetails.From(Templates.Tenant1FirstInvoiceTemplate);
            var receivedDetails = await _client.Get(expectedDetails.TemplateId);

            receivedDetails.Should().BeEquivalentTo(expectedDetails);
        }
    }
}