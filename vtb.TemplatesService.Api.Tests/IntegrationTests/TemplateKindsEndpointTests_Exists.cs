using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RESTFulSense.Exceptions;
using vtb.TemplatesService.DataAccess.Seed;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public class TemplateKindsEndpointTests_Exists : TemplateKindsEndpointTests
    {
        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.Exists(TemplateKinds.InvoiceTemplateKind.TemplateKindKey));
        }

        [Test]
        public async Task Will_Return_True_When_Exists()
        {
            Authorize(Guid.NewGuid(), Guid.NewGuid(), new string[0], new string[0]);

            var result = await _client.Exists(TemplateKinds.InvoiceTemplateKind.TemplateKindKey);
            result.Should().BeTrue();
        }

        [Test]
        public async Task Will_Return_False_When_Does_Not_Exists()
        {
            Authorize(Guid.NewGuid(), Guid.NewGuid(), new string[0], new string[0]);

            var result = await _client.Exists(Guid.NewGuid().ToString());
            result.Should().BeFalse();
        }
    }
}