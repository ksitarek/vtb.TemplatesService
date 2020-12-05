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
    public class TemplatesEndpointTests_AddNewTemplateVersion : TemplatesEndpointTests
    {
        private Guid _templateId = Templates.Tenant1SecondInvoiceTemplate.TemplateId;

        private ExpectedCreateTemplateVersion _request = new ExpectedCreateTemplateVersion()
        {
            Content = Guid.NewGuid().ToString(),
            IsActive = true
        };

        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.AddNewTemplateVersion(_templateId, _request));
        }

        [Test]
        public void Will_Require_ManageTemplates_Permission()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), Array.Empty<string>());
            Assert.ThrowsAsync<HttpResponseForbiddenException>(async () =>
                await _client.AddNewTemplateVersion(_templateId, _request));
        }

        [Test]
        public async Task Will_Add_New_Active_TemplateVersion()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });
            await _client.AddNewTemplateVersion(Templates.Tenant1FirstInvoiceTemplate.TemplateId, _request);

            var template = await _client.Get(Templates.Tenant1FirstInvoiceTemplate.TemplateId);
            template.CurrentVersionContent.Should().Be(_request.Content);
        }

        [Test]
        public async Task Will_Add_New_TemplateVersion()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });
            var uri = await _client.AddNewTemplateVersion(Templates.Tenant1FirstInvoiceTemplate.TemplateId, _request);
            var templateVersionId = Guid.Parse(uri.Segments[5]);

            var templateVersion = await _client.GetTemplateVersionDetails(Templates.Tenant1FirstInvoiceTemplate.TemplateId, templateVersionId);
            templateVersion.Content.Should().Be(_request.Content);
        }

        [Test]
        public void Will_Not_Add_TemplateVersion_To_Template_Owned_By_Another_Tenant()
        {
            Authorize(Guid.NewGuid(), Tenant2Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });
            Assert.ThrowsAsync<HttpResponseNotFoundException>(async ()
                => await _client.AddNewTemplateVersion(Templates.Tenant1FirstInvoiceTemplate.TemplateId, _request));
        }
    }
}