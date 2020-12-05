using FluentAssertions;
using NUnit.Framework;
using RESTFulSense.Exceptions;
using System;
using System.Threading.Tasks;
using vtb.TemplatesService.DataAccess.Seed;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public class TemplateKindsEndpointTests_Get : TemplateKindsEndpointTests
    {
        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.GetTemplateKind(TemplateKinds.EmailTemplateKind.TemplateKindKey));
        }

        [Test]
        public async Task Will_Return_TemplateKind_By_Key()
        {
            Authorize(Guid.NewGuid(), Guid.NewGuid(), Array.Empty<string>(), Array.Empty<string>());

            var expectedTemplateKind = TemplateKinds.EmailTemplateKind;
            var receivedTemplateKind = await _client.GetTemplateKind(TemplateKinds.EmailTemplateKind.TemplateKindKey);

            receivedTemplateKind.Should().BeEquivalentTo(expectedTemplateKind);
        }
    }
}