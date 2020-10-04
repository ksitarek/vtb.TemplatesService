using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RESTFulSense.Exceptions;
using vtb.TemplatesService.Api.Tests.IntegrationTests.ExpectedResults;
using vtb.TemplatesService.DataAccess.Seed;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public class TemplatesEndpointTests_GetTemplates : TemplatesEndpointTests
    {
        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.GetTemplates(1, 1));
        }

        [Test]
        public async Task Will_Return_Empty_Page_When_No_Records()
        {
            Authorize(Guid.NewGuid(), Guid.NewGuid(), Array.Empty<string>(), Array.Empty<string>());
            var expectedEmptyPage = new ExpectedListPage<ExpectedTemplateListItem>(0, new List<ExpectedTemplateListItem>());

            var receivedPage = await _client.GetTemplates(1, 1);
            receivedPage.Should().BeEquivalentTo(expectedEmptyPage);
        }

        [Test]
        public async Task Will_Paginate_Templates()
        {
            Authorize(Guid.NewGuid(), Tenant1Id, Array.Empty<string>(), Array.Empty<string>());
            await TestPagination((page, pageSize) => _client.GetTemplates(page, pageSize),
                new List<ExpectedTemplateListItem>()
                {
                    ExpectedTemplateListItem.From(Templates.Tenant1FirstInvoiceTemplate),
                    ExpectedTemplateListItem.From(Templates.Tenant1SecondInvoiceTemplate)
                });

            Authorize(Guid.NewGuid(), Tenant2Id, Array.Empty<string>(), Array.Empty<string>());
            await TestPagination((page, pageSize) => _client.GetTemplates(page, pageSize),
                new List<ExpectedTemplateListItem>()
                {
                    ExpectedTemplateListItem.From(Templates.Tenant2FirstInvoiceTemplate),
                    ExpectedTemplateListItem.From(Templates.Tenant2SecondInvoiceTemplate)
                });
        }
    }
}