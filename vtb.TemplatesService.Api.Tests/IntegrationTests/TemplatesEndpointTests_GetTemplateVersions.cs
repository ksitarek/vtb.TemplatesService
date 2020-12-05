using NUnit.Framework;
using RESTFulSense.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vtb.Auth.Permissions;
using vtb.TemplatesService.Api.Tests.IntegrationTests.ExpectedResults;
using vtb.TemplatesService.DataAccess.Seed;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public class TemplatesEndpointTests_GetTemplateVersions : TemplatesEndpointTests
    {
        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.GetTemplateVersions(Templates.Tenant1FirstInvoiceTemplate.TemplateId, 1, 1));
        }

        [Test]
        public void Will_Require_ManageTemplates_Permission()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), Array.Empty<string>());
            Assert.ThrowsAsync<HttpResponseForbiddenException>(async () =>
                await _client.GetTemplateVersions(Templates.Tenant1FirstInvoiceTemplate.TemplateId, 1, 1));
        }

        [Test]
        public void Will_Return_NotFound_When_No_Template()
        {
            Authorize(Guid.NewGuid(), Guid.NewGuid(), Array.Empty<string>(), new[] { Permissions.ManageTemplates });
            Assert.ThrowsAsync<HttpResponseNotFoundException>(async () =>
                await _client.GetTemplateVersions(Templates.Tenant1FirstInvoiceTemplate.TemplateId, 1, 1));
        }

        [Test]
        public async Task Will_Paginate_TemplateVersions()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), new[] { Permissions.ManageTemplates });

            var template = Templates.Tenant1FirstInvoiceTemplate;
            await TestPagination((page, pageSize) => _client.GetTemplateVersions(template.TemplateId, page, pageSize),
                new List<ExpectedTemplateVersionListItem>()
                {
                    ExpectedTemplateVersionListItem.From(template.Versions[1]),
                    ExpectedTemplateVersionListItem.From(template.Versions[0])
                });
        }
    }
}