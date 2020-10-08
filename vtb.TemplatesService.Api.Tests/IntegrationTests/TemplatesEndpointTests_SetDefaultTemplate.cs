using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RESTFulSense.Exceptions;
using vtb.Auth.Permissions;
using vtb.TemplatesService.DataAccess.Seed;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public class TemplatesEndpointTests_SetDefaultTemplate : TemplatesEndpointTests
    {
        private string _templateKindKey = TemplateKinds.InvoiceTemplateKind.TemplateKindKey;
        private Template _template1 = Templates.Tenant1SecondInvoiceTemplate;

        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.SetDefaultTemplate(_templateKindKey, _template1.TemplateId));
        }

        [Test]
        public void Will_Require_ManageTemplates_Permission()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), Array.Empty<string>());
            Assert.ThrowsAsync<HttpResponseForbiddenException>(async () =>
                await _client.SetDefaultTemplate(_templateKindKey, _template1.TemplateId));
        }

        [Test]
        public async Task Will_Set_Default_Template()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });
            var currentDefaultTemplate = await _client.GetDefaultTemplate(_templateKindKey);

            await _client.SetDefaultTemplate(_templateKindKey, _template1.TemplateId);
            var newDefaultTemplate = await _client.GetDefaultTemplate(_templateKindKey);

            newDefaultTemplate.Should().NotBeEquivalentTo(currentDefaultTemplate);
            newDefaultTemplate.TemplateId.Should().Be(_template1.TemplateId);
        }

        [Test]
        public void Will_Not_Set_Other_Tenants_Default_Template()
        {
            Authorize(Guid.NewGuid(), Tenant2Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });

            Assert.ThrowsAsync<HttpResponseBadRequestException>(async ()
                => await _client.SetDefaultTemplate(_templateKindKey, _template1.TemplateId));
        }
    }
}