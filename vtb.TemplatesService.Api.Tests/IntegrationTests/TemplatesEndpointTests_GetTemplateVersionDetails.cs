using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RESTFulSense.Exceptions;
using vtb.Auth.Permissions;
using vtb.TemplatesService.Api.Tests.IntegrationTests.ExpectedResults;
using vtb.TemplatesService.DataAccess.Seed;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public class TemplatesEndpointTests_GetTemplateVersionDetails : TemplatesEndpointTests
    {
        private Template _template1 = Templates.Tenant1FirstInvoiceTemplate;
        private Template _template2 = Templates.Tenant2SecondInvoiceTemplate;
        private TemplateVersion _templateVersion1 = Templates.Tenant1FirstInvoiceTemplate.Versions[0];
        private TemplateVersion _templateVersion2 = Templates.Tenant2SecondInvoiceTemplate.Versions[1];

        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.GetTemplateVersionDetails(_template1.TemplateId, _templateVersion1.TemplateVersionId));
        }

        [Test]
        public void Will_Require_ManageTemplates_Permission()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, new string[0], new string[0]);
            Assert.ThrowsAsync<HttpResponseForbiddenException>(async () =>
                await _client.GetTemplateVersionDetails(_template1.TemplateId, _templateVersion1.TemplateVersionId));
        }

        [Test]
        public void Will_Return_NotFound_When_Cannot_Find_TemplateVersion()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, new string[0], new[] { Permissions.ManageTemplates });

            Assert.ThrowsAsync<HttpResponseNotFoundException>(async () =>
                await _client.GetTemplateVersionDetails(Guid.NewGuid(), _templateVersion1.TemplateVersionId));

            Assert.ThrowsAsync<HttpResponseNotFoundException>(async () =>
                await _client.GetTemplateVersionDetails(_template1.TemplateId, Guid.NewGuid()));

            Assert.ThrowsAsync<HttpResponseNotFoundException>(async () =>
                await _client.GetTemplateVersionDetails(_template2.TemplateId, _templateVersion2.TemplateVersionId));
        }

        [Test]
        public async Task Will_Return_TemplateVersionDetails()
        {
            var expectedDetails1 = ExpectedTemplateVersionDetails.From(_templateVersion1);
            var expectedDetails2 = ExpectedTemplateVersionDetails.From(_templateVersion2);

            Authorize(Guid.NewGuid(), Tenant1Id, new string[0], new[] { Permissions.ManageTemplates });
            var receivedDetails1 = await _client.GetTemplateVersionDetails(_template1.TemplateId, _templateVersion1.TemplateVersionId);

            Authorize(Guid.NewGuid(), Tenant2Id, new string[0], new[] { Permissions.ManageTemplates });
            var receivedDetails2 = await _client.GetTemplateVersionDetails(_template2.TemplateId, _templateVersion2.TemplateVersionId);

            receivedDetails1.Should().BeEquivalentTo(expectedDetails1);
            receivedDetails2.Should().BeEquivalentTo(expectedDetails2);
        }
    }
}