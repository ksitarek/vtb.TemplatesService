using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RESTFulSense.Exceptions;
using vtb.Auth.Permissions;
using vtb.TemplatesService.Api.Tests.IntegrationTests.ExpectedResults;
using vtb.TemplatesService.DataAccess.Seed;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public class TemplatesEndpointTests_UpdateTemplateVersion : TemplatesEndpointTests
    {
        private Guid _templateId = Templates.Tenant1FirstInvoiceTemplate.TemplateId;
        private Guid _templateVersionId = Templates.Tenant1FirstInvoiceTemplate.Versions[0].TemplateVersionId;

        private readonly ExpectedUpdateTemplateVersionBody _request = new ExpectedUpdateTemplateVersionBody()
        {
            Content = Guid.NewGuid().ToString()
        };

        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.UpdateTemplateVersion(_templateId, _templateVersionId, _request));
        }

        [Test]
        public void Will_Require_ManageTemplates_Permission()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), Array.Empty<string>());
            Assert.ThrowsAsync<HttpResponseForbiddenException>(async () =>
                await _client.UpdateTemplateVersion(_templateId, _templateVersionId, _request));
        }

        [Test]
        public async Task Will_Update_TemplateVersion()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });
            await _client.UpdateTemplateVersion(_templateId, _templateVersionId, _request);

            var template = await _client.Get(_templateId);
            template.CurrentVersionContent.Should().Be(_request.Content);
            template.CurrentVersionUpdatedAt.Should().Be(ApiFactory.UtcNow);
        }
    }
}