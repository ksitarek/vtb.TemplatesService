using System;
using System.Threading.Tasks;
using NUnit.Framework;
using RESTFulSense.Exceptions;
using vtb.Auth.Permissions;
using vtb.TemplatesService.DataAccess.Seed;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public class TemplatesEndpointTests_RemoveTemplateVersion : TemplatesEndpointTests
    {
        private Template _template1 = Templates.Tenant1FirstInvoiceTemplate;
        private Template _template2 = Templates.Tenant2FirstInvoiceTemplate;
        private TemplateVersion _template1Version1Active = Templates.Tenant1FirstInvoiceTemplate.Versions[0];
        private TemplateVersion _template1Version2 = Templates.Tenant1FirstInvoiceTemplate.Versions[1];
        private TemplateVersion _template2Version1Active = Templates.Tenant2FirstInvoiceTemplate.Versions[0];
        private TemplateVersion _template2Version2 = Templates.Tenant2FirstInvoiceTemplate.Versions[1];

        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.RemoveTemplateVersion(_template1.TemplateId, _template1Version2.TemplateVersionId));
        }

        [Test]
        public void Will_Require_ManageTemplates_Permission()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), Array.Empty<string>());
            Assert.ThrowsAsync<HttpResponseForbiddenException>(async () =>
                await _client.RemoveTemplateVersion(_template1.TemplateId, _template1Version2.TemplateVersionId));
        }

        [Test]
        public async Task Will_Remove_Template_Version()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });
            await _client.RemoveTemplateVersion(_template1.TemplateId, _template1Version2.TemplateVersionId);

            Assert.ThrowsAsync<HttpResponseNotFoundException>(async ()
                => await _client.GetTemplateVersionDetails(_template1.TemplateId, _template1Version2.TemplateVersionId));

            Authorize(Guid.NewGuid(), Tenant2Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });
            await _client.RemoveTemplateVersion(_template2.TemplateId, _template2Version2.TemplateVersionId);

            Assert.ThrowsAsync<HttpResponseNotFoundException>(async ()
                => await _client.GetTemplateVersionDetails(_template2.TemplateId, _template2Version2.TemplateVersionId));
        }

        [Test]
        public void Will_Not_Remove_TemplateVersion_From_Template_Owned_By_Other_Tenant()
        {
            Authorize(Guid.NewGuid(), Tenant2Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });
            Assert.ThrowsAsync<HttpResponseNotFoundException>(async ()
                => await _client.RemoveTemplateVersion(_template1.TemplateId, _template1Version2.TemplateVersionId));
        }

        [Test]
        public async Task Will_Not_Remove_Active_TemplateVersion()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });
            Assert.ThrowsAsync<HttpResponseConflictException>(async ()
                => await _client.RemoveTemplateVersion(_template1.TemplateId, _template1Version1Active.TemplateVersionId));
            await _client.GetTemplateVersionDetails(_template1.TemplateId, _template1Version1Active.TemplateVersionId);

            Authorize(Guid.NewGuid(), Tenant2Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });
            Assert.ThrowsAsync<HttpResponseConflictException>(async ()
                => await _client.RemoveTemplateVersion(_template2.TemplateId, _template2Version1Active.TemplateVersionId));
            await _client.GetTemplateVersionDetails(_template2.TemplateId, _template2Version1Active.TemplateVersionId);
        }
    }
}