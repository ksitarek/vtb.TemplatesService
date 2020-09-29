using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RESTFulSense.Exceptions;
using vtb.TemplatesService.Api.Tests.IntegrationTests.ExpectedResults;
using vtb.TemplatesService.DataAccess.Seed;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public class TemplatesEndpointTests_GetDefaultTemplate : TemplatesEndpointTests
    {
        private string _templateKindKey = TemplateKinds.InvoiceTemplateKind.TemplateKindKey;

        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.GetDefaultTemplate(_templateKindKey));
        }

        [Test]
        public void Will_Return_NotFound_When_Not_Set()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, new string[0], new string[0]);
            Assert.ThrowsAsync<HttpResponseNotFoundException>(async () =>
                await _client.GetDefaultTemplate(TemplateKinds.EmailTemplateKind.TemplateKindKey));
        }

        [Test]
        public async Task Will_Return_Default_Template()
        {
            var expectedDetails1 = ExpectedTemplateDetails.From(Templates.Tenant1FirstInvoiceTemplate);
            var expectedDetails2 = ExpectedTemplateDetails.From(Templates.Tenant2FirstInvoiceTemplate);

            Authorize(Guid.NewGuid(), Tenant1Id, new string[0], new string[0]);
            var receivedDetails1 = await _client.GetDefaultTemplate(_templateKindKey);

            Authorize(Guid.NewGuid(), Tenant2Id, new string[0], new string[0]);
            var receivedDetails2 = await _client.GetDefaultTemplate(_templateKindKey);

            receivedDetails1.Should().BeEquivalentTo(expectedDetails1);
            receivedDetails2.Should().BeEquivalentTo(expectedDetails2);
        }
    }
}