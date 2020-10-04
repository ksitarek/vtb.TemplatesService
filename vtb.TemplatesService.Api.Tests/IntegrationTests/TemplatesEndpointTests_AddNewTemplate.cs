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
    public class TemplatesEndpointTests_AddNewTemplate : TemplatesEndpointTests
    {
        private ExpectedCreateTemplate _request = new ExpectedCreateTemplate()
        {
            TemplateLabel = Guid.NewGuid().ToString(),
            TemplateKindKey = TemplateKinds.InvoiceTemplateKind.TemplateKindKey,
            Content = Guid.NewGuid().ToString()
        };

        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.AddNewTemplate(_request));
        }

        [Test]
        public void Will_Require_ManageTemplates_Permission()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), Array.Empty<string>());
            Assert.ThrowsAsync<HttpResponseForbiddenException>(async () =>
                await _client.AddNewTemplate(_request));
        }

        [Test]
        public async Task Will_Prevent_Conflicting_Labels()
        {
            var label = Guid.NewGuid().ToString();
            _request.TemplateLabel = label;

            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });

            await _client.AddNewTemplate(_request);

            Assert.ThrowsAsync<HttpResponseConflictException>(async () =>
                await _client.AddNewTemplate(_request));
        }

        [Test]
        public async Task Will_Allow_Same_Label_Between_Tenants()
        {
            var expectedTemplateDetails = new ExpectedTemplateDetails()
            {
                TemplateId = Guid.Empty,
                Label = _request.TemplateLabel,
                TemplateKindKey = _request.TemplateKindKey,
                CurrentVersionContent = _request.Content,
                CurrentVersionId = Guid.Empty,
                CurrentVersion = 1,
                CurrentVersionCreatedAt = ApiFactory.UtcNow
            };

            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });
            var uri1 = await _client.AddNewTemplate(_request);

            var createdTemplate1 = await _client.Get(Guid.Parse(uri1.Segments[3]));
            createdTemplate1.TemplateId.Should().Be(Guid.Parse(uri1.Segments[3]));
            createdTemplate1.CurrentVersionId.Should().NotBe(Guid.Empty);
            createdTemplate1.Should().BeEquivalentTo(
                expectedTemplateDetails,
                options => options.Excluding(o => o.TemplateId).Excluding(o => o.CurrentVersionId));

            Authorize(Guid.NewGuid(), Tenant2Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });
            var uri2 = await _client.AddNewTemplate(_request);

            var createdTemplate2 = await _client.Get(Guid.Parse(uri2.Segments[3]));
            createdTemplate2.TemplateId.Should().Be(Guid.Parse(uri2.Segments[3]));
            createdTemplate2.CurrentVersionId.Should().NotBe(Guid.Empty);
            createdTemplate2.Should().BeEquivalentTo(
                expectedTemplateDetails,
                options => options.Excluding(o => o.TemplateId).Excluding(o => o.CurrentVersionId));
        }
    }
}