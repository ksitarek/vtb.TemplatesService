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
    public class TemplateKindsEndpointTests_CreateTemplateKind : TemplateKindsEndpointTests
    {
        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.CreateTemplateKind(Guid.NewGuid().ToString()));
        }

        [Test]
        public void Will_Require_SystemManage_Permission()
        {
            Authorize(Guid.NewGuid(), Guid.NewGuid(), Array.Empty<string>(), Array.Empty<string>());

            Assert.ThrowsAsync<HttpResponseForbiddenException>(async () =>
                await _client.CreateTemplateKind(Guid.NewGuid().ToString()));
        }

        [Test]
        public async Task Will_Create_New_TemplateKind()
        {
            Authorize(Guid.NewGuid(), Guid.NewGuid(), Array.Empty<string>(), new[] { Permissions.SystemManage });

            var expectedTemplateKind = new TemplateKind() { TemplateKindKey = Guid.NewGuid().ToString() };
            await _client.CreateTemplateKind(expectedTemplateKind.TemplateKindKey);

            var receivedTemplateKind = await _client.GetTemplateKind(expectedTemplateKind.TemplateKindKey);
            receivedTemplateKind.Should().BeEquivalentTo(expectedTemplateKind);
        }

        [Test]
        public void Will_Not_Create_TemplateKind_With_Taken_Key()
        {
            Authorize(Guid.NewGuid(), Guid.NewGuid(), Array.Empty<string>(), new[] { Permissions.SystemManage });

            Assert.ThrowsAsync<HttpResponseConflictException>(async () =>
                await _client.CreateTemplateKind(TemplateKinds.EmailTemplateKind.TemplateKindKey));
        }
    }
}