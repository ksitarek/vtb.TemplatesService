using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using RESTFulSense.Exceptions;
using vtb.Auth.Permissions;
using vtb.TemplatesService.DataAccess.Seed;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public class TemplatesEndpointTests_SetCurrentVersion : TemplatesEndpointTests
    {
        private Template _template1 = Templates.Tenant1SecondInvoiceTemplate;

        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.SetCurrentVersion(_template1.TemplateId, _template1.Versions[0].TemplateVersionId));
        }

        [Test]
        public void Will_Require_ManageTemplates_Permission()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), Array.Empty<string>());
            Assert.ThrowsAsync<HttpResponseForbiddenException>(async () =>
                await _client.SetCurrentVersion(_template1.TemplateId, _template1.Versions[0].TemplateVersionId));
        }

        [Test]
        public async Task Will_Set_Current_Version()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });

            var templateVersions = await _client.GetTemplateVersions(_template1.TemplateId, 1, int.MaxValue);

            await _client.SetCurrentVersion(_template1.TemplateId, _template1.Versions[0].TemplateVersionId);

            var templateWithCurrentVersion = await _client.Get(_template1.TemplateId);
            var expectedVersion = _template1.Versions.First(x => x.TemplateVersionId == _template1.Versions[0].TemplateVersionId);

            templateWithCurrentVersion.CurrentVersionId = expectedVersion.TemplateVersionId;
            templateWithCurrentVersion.CurrentVersion = expectedVersion.Version;
            templateWithCurrentVersion.CurrentVersionContent = expectedVersion.Content;
            templateWithCurrentVersion.CurrentVersionCreatedAt = expectedVersion.CreatedAt;
        }
    }
}