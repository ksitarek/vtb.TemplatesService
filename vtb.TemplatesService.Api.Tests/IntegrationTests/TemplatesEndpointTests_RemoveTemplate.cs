using System;
using System.Threading.Tasks;
using NUnit.Framework;
using RESTFulSense.Exceptions;
using vtb.Auth.Permissions;
using vtb.TemplatesService.DataAccess.Seed;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public class TemplatesEndpointTests_RemoveTemplate : TemplatesEndpointTests
    {
        private Template _template1 = Templates.Tenant1SecondInvoiceTemplate;
        private Template _template2 = Templates.Tenant2SecondInvoiceTemplate;

        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.RemoveTemplate(_template1.TemplateId));
        }

        [Test]
        public void Will_Require_ManageTemplates_Permission()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), Array.Empty<string>());
            Assert.ThrowsAsync<HttpResponseForbiddenException>(async () =>
                await _client.RemoveTemplate(_template1.TemplateId));
        }

        [Test]
        public async Task Will_Remove_Template()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });
            await _client.RemoveTemplate(_template1.TemplateId);
            Assert.ThrowsAsync<HttpResponseNotFoundException>(async () => await _client.Get(_template1.TemplateId));

            Authorize(Guid.NewGuid(), Tenant2Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });
            await _client.RemoveTemplate(_template2.TemplateId);
            Assert.ThrowsAsync<HttpResponseNotFoundException>(async () => await _client.Get(_template2.TemplateId));
        }

        [Test]
        public void Will_Not_Remove_Template_Owned_By_Other_Tenant()
        {
            Authorize(Guid.NewGuid(), Tenant2Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });
            Assert.ThrowsAsync<HttpResponseNotFoundException>(async () => await _client.RemoveTemplate(_template1.TemplateId));
        }
    }
}