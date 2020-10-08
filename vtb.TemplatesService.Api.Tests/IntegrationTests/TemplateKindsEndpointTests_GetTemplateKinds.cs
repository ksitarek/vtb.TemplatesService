using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NUnit.Framework;
using RESTFulSense.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vtb.TemplatesService.Api.Tests.IntegrationTests.ExpectedResults;
using vtb.TemplatesService.DataAccess.Seed;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public class TemplateKindsEndpointTests_GetTemplateKinds : TemplateKindsEndpointTests
    {
        [Test]
        public void Will_Require_Authentication()
        {
            Assert.ThrowsAsync<HttpResponseUnauthorizedException>(async () =>
                await _client.GetTemplateKinds(1, 1));
        }

        [Test]
        public async Task Will_Paginate_TemplateKinds()
        {
            Authorize(Guid.NewGuid(), Guid.NewGuid(), Array.Empty<string>(), Array.Empty<string>());
            await TestPagination((page, pageSize) => _client.GetTemplateKinds(page, pageSize),
                new List<ExpectedTemplateKindListItem>()
                {
                    new ExpectedTemplateKindListItem() {TemplateKindKey = TemplateKinds.EmailTemplateKind.TemplateKindKey},
                    new ExpectedTemplateKindListItem() {TemplateKindKey = TemplateKinds.InvoiceTemplateKind.TemplateKindKey}
                });
        }

        [Test]
        public async Task Will_Return_Empty_Page_When_No_Records()
        {
            var db = _factory.Services.GetService<IMongoDatabase>();
            var collection = db.GetCollection<TemplateKind>("TemplateKinds");
            await collection.DeleteManyAsync(Builders<TemplateKind>.Filter.Empty);

            Authorize(Guid.NewGuid(), Guid.NewGuid(), Array.Empty<string>(), Array.Empty<string>());

            var expectedEmptyPage =
                new ExpectedListPage<ExpectedTemplateKindListItem>(0, new List<ExpectedTemplateKindListItem>() { });

            var receivedEmptyPage = await _client.GetTemplateKinds(1, 1);
            receivedEmptyPage.Should().BeEquivalentTo(expectedEmptyPage);
        }
    }
}