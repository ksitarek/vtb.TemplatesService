using System;
using System.Threading.Tasks;
using NUnit.Framework;
using RESTFulSense.Exceptions;
using vtb.Auth.Permissions;
using vtb.TemplatesService.DataAccess.Seed;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public class TemplateKindsEndpointTests_RemoveTemplateKind : TemplateKindsEndpointTests
    {
        private string _templateKindKey = Guid.NewGuid().ToString();

        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.RemoveTemplateKind(_templateKindKey));
        }

        [Test]
        public void Will_Require_SystemManage_Permission()
        {
            Authorize(Guid.NewGuid(), Guid.NewGuid(), Array.Empty<string>(), Array.Empty<string>());

            Assert.ThrowsAsync<HttpResponseForbiddenException>(async () =>
                await _client.RemoveTemplateKind(_templateKindKey));
        }

        [Test]
        public async Task Will_Remove_TemplateKindAsync()
        {
            Authorize(Guid.NewGuid(), Guid.NewGuid(), Array.Empty<string>(), new[] { Permissions.SystemManage });

            var expectedTemplateKind = new TemplateKind() { TemplateKindKey = _templateKindKey };
            await _client.CreateTemplateKind(expectedTemplateKind.TemplateKindKey);

            await _client.RemoveTemplateKind(_templateKindKey);

            Assert.ThrowsAsync<HttpResponseNotFoundException>(async () => await _client.GetTemplateKind(_templateKindKey));
        }

        [Test]
        public void Will_Not_Remove_TemplateKind_That_Is_Being_Used()
        {
            Authorize(Guid.NewGuid(), Guid.NewGuid(), Array.Empty<string>(), new[] { Permissions.SystemManage });

            Assert.ThrowsAsync<HttpResponseConflictException>(async () => await _client.RemoveTemplateKind(TemplateKinds.InvoiceTemplateKind.TemplateKindKey));
        }
    }
}