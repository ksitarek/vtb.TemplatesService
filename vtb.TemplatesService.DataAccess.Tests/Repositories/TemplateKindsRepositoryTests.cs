using FluentAssertions;
using MongoDB.Driver;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.DataAccess.Repositories;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.DataAccess.Tests.Repositories
{
    public class TemplateKindsRepositoryTests : MongoDbTests
    {
        private IMongoCollection<TemplateKind> _collection;
        private TemplateKindsRepository _repository;

        private IReadOnlyList<TemplateKind> _templateKinds = new List<TemplateKind>()
            {
                new TemplateKind() {TemplateKindKey = "tk-1" },
                new TemplateKind() {TemplateKindKey = "tk-2" },
            }.AsReadOnly();

        [SetUp]
        public void SetUp()
        {
            _collection = _database.GetCollection<TemplateKind>("TemplateKinds");
            _collection.InsertMany(_templateKinds);

            _repository = new TemplateKindsRepository(_database);
        }

        [TearDown]
        public void TearDown()
        {
            _collection.DeleteMany(Builders<TemplateKind>.Filter.Empty);
        }

        [Test]
        public async Task Will_Insert_TemplateKind_To_Collection()
        {
            var templateKind = new TemplateKind() { TemplateKindKey = "tk-3" };
            await _repository.AddTemplateKind(templateKind, CancellationToken.None);

            var filter = Builders<TemplateKind>.Filter.Eq(x => x.TemplateKindKey, templateKind.TemplateKindKey);
            (await _collection.Find(filter).FirstAsync()).Should().BeEquivalentTo(templateKind);
        }

        [Test]
        public async Task Will_Return_TemplateKind()
        {
            var result = await _repository.GetTemplateKind("tk-1");
            result.Should().BeEquivalentTo(_templateKinds.ElementAt(0));
        }

        [Test]
        public async Task Will_Remove_TemplateKind()
        {
            await _repository.RemoveTemplateKind("tk-1");

            var filter = Builders<TemplateKind>.Filter.Eq(x => x.TemplateKindKey, "tk-1");
            (await _collection.Find(filter).FirstOrDefaultAsync()).Should().BeNull();
        }

        [Test]
        public async Task Will_Return_Page_Of_TemplateKinds()
        {
            var page = await _repository.GetTemplateKindsPage(1, 1);
            page.Entities.Count.Should().Be(1);
            page.TotalCount.Should().Be(2);
            page.Entities.ElementAt(0).Should().BeEquivalentTo(_templateKinds.ElementAt(0));

            page = await _repository.GetTemplateKindsPage(2, 1);
            page.Entities.Count.Should().Be(1);
            page.TotalCount.Should().Be(2);
            page.Entities.ElementAt(0).Should().BeEquivalentTo(_templateKinds.ElementAt(1));
        }

        [Test]
        public async Task Will_Return_Empty_Page_When_No_TemplateKinds()
        {
            _collection.DeleteMany(Builders<TemplateKind>.Filter.Empty);

            var page = await _repository.GetTemplateKindsPage(1, 1);
            page.Entities.Should().NotBeNull();
            page.Entities.Count.Should().Be(0);
            page.TotalCount.Should().Be(0);
        }
    }
}