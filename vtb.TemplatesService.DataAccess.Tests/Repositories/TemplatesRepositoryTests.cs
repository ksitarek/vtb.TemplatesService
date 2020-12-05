using FluentAssertions;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using vtb.Auth.Tenant;
using vtb.TemplatesService.DataAccess.Repositories;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.DataAccess.Tests.Repositories
{
    public class TemplatesRepositoryTests : MongoDbTests
    {
        private IMongoCollection<Template> _collection;
        private Mock<ITenantIdProvider> _tenantIdProvider;
        private TemplatesRepository _repository;

        private readonly Guid Tenant1Id = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f");
        private readonly Guid Tenant2Id = new Guid("cc6f58cc-8446-4886-83d6-4917278d3082");

        private IReadOnlyList<Template> _templates = new List<Template>()
        {
            new Template()
            {
                TemplateId = Guid.NewGuid(), TemplateKindKey = "tk1", Label = "Tenant 1 Template Kind 1 Template 1", TenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f"), Versions = new List<TemplateVersion>()
                {
                    new TemplateVersion() { TemplateVersionId = new Guid("3c4fa624-b7b2-4c89-80f0-d7277332b3a2"), Content = "Lorem Ipsum", IsActive = true, Version = 1, CreatedAt = new DateTimeOffset() },
                    new TemplateVersion() { TemplateVersionId = new Guid("d013797f-e190-4d28-b9dc-f49905bc3fe4"), Content = "Lorem Ipsum", IsActive = false, Version = 2, CreatedAt = new DateTimeOffset() }
                },
            },
            new Template() { TemplateId = Guid.NewGuid(), TemplateKindKey = "tk1", Label = "Tenant 1 Template Kind 1 Template 2", TenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f"), IsDefault = true, Versions = new List<TemplateVersion>()
                {
                    new TemplateVersion() { TemplateVersionId = new Guid("de6f55dd-d40f-4020-8857-7f12788c8a05"), Content = "Lorem Ipsum 2", IsActive = true, Version = 1, CreatedAt = new DateTimeOffset() },
                    new TemplateVersion() { TemplateVersionId = new Guid("59b49e83-0009-4c5c-977f-22290dc36ac2"), Content = "Lorem Ipsum 2", IsActive = false, Version = 2, CreatedAt = new DateTimeOffset() }
                },
            },
            new Template() { TemplateId = Guid.NewGuid(), TemplateKindKey = "tk2", Label = "Tenant 1 Template Kind 2 Template 1", TenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f") },
            new Template() { TemplateId = Guid.NewGuid(), TemplateKindKey = "tk2", Label = "Tenant 1 Template Kind 2 Template 2", TenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f") },

            new Template() { TemplateId = Guid.NewGuid(), TemplateKindKey = "tk1", Label = "Tenant 2 Template Kind 1 Template 1", TenantId = new Guid("cc6f58cc-8446-4886-83d6-4917278d3082"), IsDefault = true},
            new Template() { TemplateId = Guid.NewGuid(), TemplateKindKey = "tk1", Label = "Tenant 2 Template Kind 1 Template 2", TenantId = new Guid("cc6f58cc-8446-4886-83d6-4917278d3082") },
            new Template() { TemplateId = Guid.NewGuid(), TemplateKindKey = "tk2", Label = "Tenant 2 Template Kind 2 Template 1", TenantId = new Guid("cc6f58cc-8446-4886-83d6-4917278d3082") },
            new Template() { TemplateId = Guid.NewGuid(), TemplateKindKey = "tk3", Label = "Tenant 2 Template Kind 2 Template 1", TenantId = new Guid("2C01E7D6-7762-4A0E-82C0-EC13272833DC") },
        }.AsReadOnly();

        [SetUp]
        public void SetUp()
        {
            _collection = _database.GetCollection<Template>("Templates");
            _collection.InsertMany(_templates);

            _tenantIdProvider = new Mock<ITenantIdProvider>();

            _repository = new TemplatesRepository(_database, _tenantIdProvider.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _collection.DeleteMany(Builders<Template>.Filter.Empty);
        }

        [Test]
        public async Task Will_Insert_Template_To_Collection_Preserving_Tenant_Context()
        {
            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            var template = new Template() { TemplateId = Guid.NewGuid() };
            await _repository.AddTemplate(template, CancellationToken.None);

            var filter = Builders<Template>.Filter.Eq(x => x.TemplateId, template.TemplateId);
            (await _collection.Find(filter).FirstAsync()).Should()
                .BeEquivalentTo(new Template() { TemplateId = template.TemplateId, TenantId = Tenant1Id });
        }

        [Test]
        public async Task Will_Return_Template_With_All_Versions_By_TemplateId()
        {
            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();
            var template = await _repository.GetTemplateWithAllVersions(_templates[0].TemplateId);
            template.Should().BeEquivalentTo(_templates[0]);
        }

        [Test]
        public async Task Will_Return_Template_With_Active_Version_Only_By_TemplateId()
        {
            var expectedTemplate = _templates[0];
            expectedTemplate.Versions = expectedTemplate.Versions.Where(x => x.IsActive).ToList();

            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();
            var template = await _repository.GetTemplateWithActiveVersionOnly(_templates[0].TemplateId);
            template.Should().BeEquivalentTo(expectedTemplate);
        }

        [Test]
        public async Task Will_Return_Page_Of_Templates_Preserving_Tenant_Context()
        {
            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();
            var expectedEntities = _templates.Where(x => x.TenantId == Tenant1Id);

            var page = await _repository.GetTemplatesPage(1, 2);

            page.Entities.Count.Should().Be(2);
            page.Entities.Should().BeEquivalentTo(expectedEntities.Take(2));
            page.TotalCount.Should().Be(4);

            page = await _repository.GetTemplatesPage(2, 2);

            page.Entities.Count.Should().Be(2);
            page.Entities.Should().BeEquivalentTo(expectedEntities.Skip(2).Take(2));
            page.TotalCount.Should().Be(4);
        }

        [Test]
        public async Task Will_Return_Empty_Page_When_No_Templates()
        {
            _collection.DeleteMany(Builders<Template>.Filter.Eq(x => x.TenantId, Tenant1Id));
            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            var page = await _repository.GetTemplatesPage(1, _templates.Count());

            page.Entities.Should().NotBeNull();
            page.Entities.Count.Should().Be(0);
            page.TotalCount.Should().Be(0);
        }

        [Test]
        public async Task Will_Not_Return_Template_Owned_By_Another_Tenant()
        {
            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant2Id).Verifiable();
            var template = await _repository.GetTemplateWithAllVersions(_templates[0].TemplateId);
            template.Should().BeNull();
        }

        [Test]
        public async Task Will_Count_Templates_By_TemplateKindKey_Without_Tenant_Context()
        {
            _tenantIdProvider.Setup(x => x.TenantId).Returns(Guid.NewGuid()).Verifiable();

            var cnt = await _repository.CountTemplatesByTemplateKindKey("tk1");
            cnt.Should().Be(_templates.Count(x => x.TemplateKindKey == "tk1"));

            _tenantIdProvider.Verify(x => x.TenantId, Times.Never);
        }

        [Test]
        public async Task Will_Return_Template_Marked_As_Default()
        {
            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();
            var defaultTemplate = await _repository.GetDefaultTemplate("tk1", CancellationToken.None);

            defaultTemplate.Should().BeEquivalentTo(_templates.ElementAt(1));
            _tenantIdProvider.Verify();
        }

        [Test]
        public async Task Will_Return_Page_Of_TemplateVersions()
        {
            var templateId = _templates[0].TemplateId;
            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            var firstPage = await _repository.GetTemplateVersionsPage(templateId, 1, 1, CancellationToken.None);
            var secondPage = await _repository.GetTemplateVersionsPage(templateId, 2, 1, CancellationToken.None);

            firstPage.TotalCount.Should().Be(2);
            secondPage.TotalCount.Should().Be(2);
            firstPage.Entities.Count.Should().Be(1);
            secondPage.Entities.Count.Should().Be(1);
            firstPage.Entities.ElementAt(0).Should().BeEquivalentTo(_templates[0].Versions.Single(x => x.Version == 2));
            secondPage.Entities.ElementAt(0).Should().BeEquivalentTo(_templates[0].Versions.Single(x => x.Version == 1));

            _tenantIdProvider.Verify();
        }

        [Test]
        public async Task Will_Return_Empty_Page_Of_TemplateVersions_When_Template_Not_Found()
        {
            var templateId = Guid.NewGuid();
            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            var firstPage = await _repository.GetTemplateVersionsPage(templateId, 1, 1, CancellationToken.None);
            firstPage.TotalCount.Should().Be(0);
            firstPage.Entities.Count.Should().Be(0);

            _tenantIdProvider.Verify();
        }

        [Test]
        public async Task Will_Remove_Template()
        {
            var templateId = _templates[0].TemplateId;
            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            await _repository.RemoveTemplate(templateId, CancellationToken.None);

            (await _collection.Find(Builders<Template>.Filter.Eq(x => x.TemplateId, templateId))
                .FirstOrDefaultAsync()).Should().BeNull();

            _tenantIdProvider.Verify();
        }

        [Test]
        public async Task Will_Not_Remove_Template_Of_Another_Tenant()
        {
            var templateId = _templates[0].TemplateId;
            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant2Id).Verifiable();

            await _repository.RemoveTemplate(templateId, CancellationToken.None);

            (await _collection.Find(Builders<Template>.Filter.Eq(x => x.TemplateId, templateId))
                .FirstOrDefaultAsync()).Should().NotBeNull();

            _tenantIdProvider.Verify();
        }

        [Test]
        public async Task Will_Remove_Template_Version()
        {
            var templateId = _templates[0].TemplateId;
            var templateVersionId = _templates[0].Versions
                .Where(x => x.IsActive == false)
                .Select(x => x.TemplateVersionId)
                .First();

            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            await _repository.RemoveTemplateVersion(templateId, templateVersionId, CancellationToken.None);

            var template = _collection.Find(x => x.TemplateId == templateId).First();
            template.Versions.Count.Should().Be(1);

            _tenantIdProvider.Verify();
        }

        [Test]
        public async Task Will_Not_Remove_Active_Template_Version()
        {
            var templateId = _templates[0].TemplateId;
            var templateVersionId = _templates[0].ActiveVersion.TemplateVersionId;

            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            await _repository.RemoveTemplateVersion(templateId, templateVersionId, CancellationToken.None);

            var template = _collection.Find(x => x.TemplateId == templateId).First();
            template.Versions.Count.Should().Be(2);

            _tenantIdProvider.Verify();
        }

        [Test]
        public async Task Will_Not_Remove_Template_Version_Of_Another_Tenant()
        {
            var templateId = _templates[0].TemplateId;
            var templateVersionId = _templates[0].Versions
                .Where(x => x.IsActive == false)
                .Select(x => x.TemplateVersionId)
                .First();

            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant2Id).Verifiable();

            await _repository.RemoveTemplateVersion(templateId, templateVersionId, CancellationToken.None);

            var template = _collection.Find(x => x.TemplateId == templateId).First();
            template.Versions.Count.Should().Be(2);

            _tenantIdProvider.Verify();
        }

        [Test]
        public async Task Will_Set_Default_Template()
        {
            var templateKindKey = "tk1";
            var templateId = _templates[0].TemplateId;

            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            await _repository.SetDefaultTemplate(templateKindKey, templateId, CancellationToken.None);

            var template = await _collection.Find(Builders<Template>.Filter.Eq(x => x.TemplateId, templateId)).FirstOrDefaultAsync();
            template.IsDefault.Should().BeTrue();

            _collection.AsQueryable()
                .Count(x => x.TenantId == Tenant1Id && x.IsDefault)
                .Should().Be(1);

            _tenantIdProvider.Verify();
        }

        [Test]
        public async Task Will_Update_Template()
        {
            var templateToUpdate = Clone(_templates[0]);
            templateToUpdate.Label = "Lorem Ipsum";

            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            await _repository.UpdateTemplate(templateToUpdate, CancellationToken.None);

            var updatedTemplate = await _collection.Find(Builders<Template>.Filter.Eq(x => x.TemplateId, templateToUpdate.TemplateId)).FirstOrDefaultAsync();
            updatedTemplate.Should().BeEquivalentTo(templateToUpdate);

            _tenantIdProvider.Verify();
        }

        [Test]
        public async Task Will_Not_Update_Other_Tenant_Template()
        {
            var templateToUpdate = Clone(_templates[0]);
            templateToUpdate.Label = "Lorem Ipsum";

            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant2Id).Verifiable();

            await _repository.UpdateTemplate(templateToUpdate, CancellationToken.None);

            var updatedTemplate = await _collection.Find(Builders<Template>.Filter.Eq(x => x.TemplateId, templateToUpdate.TemplateId)).FirstOrDefaultAsync();
            updatedTemplate.Should().BeEquivalentTo(_templates[0]);

            _tenantIdProvider.Verify();
        }

        [Test]
        public async Task Will_Update_Template_Version()
        {
            var templateId = _templates[0].TemplateId;
            var templateVersionToUpdate = Clone(_templates[0].Versions[0]);
            templateVersionToUpdate.Content = Guid.NewGuid().ToString();
            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            await _repository.UpdateTemplateVersion(templateId, templateVersionToUpdate, CancellationToken.None);

            var template = await _collection.Find(Builders<Template>.Filter.Eq(x => x.TemplateId, templateId)).FirstOrDefaultAsync();
            template.Versions[0].Should().BeEquivalentTo(templateVersionToUpdate);
            _tenantIdProvider.Verify();
        }

        [Test]
        public async Task Will_Not_Update_Another_Tenant_Template_Version()
        {
            var templateId = _templates[0].TemplateId;
            var templateVersionToUpdate = Clone(_templates[0].Versions[0]);
            templateVersionToUpdate.Content = Guid.NewGuid().ToString();
            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant2Id).Verifiable();

            await _repository.UpdateTemplateVersion(templateId, templateVersionToUpdate, CancellationToken.None);

            var template = await _collection.Find(Builders<Template>.Filter.Eq(x => x.TemplateId, templateId)).FirstOrDefaultAsync();
            template.Versions[0].Should().BeEquivalentTo(_templates[0].Versions[0]);
            _tenantIdProvider.Verify();
        }

        [Test]
        public async Task Will_Return_True_When_Template_Found()
        {
            var templateId = _templates[0].TemplateId;
            var ct = CancellationToken.None;

            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            var result = await _repository.TemplateExists(templateId, ct);
            result.Should().BeTrue();
        }

        [Test]
        public async Task Will_Return_False_When_Template_Not_Found()
        {
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            var result = await _repository.TemplateExists(templateId, ct);
            result.Should().BeFalse();
        }

        [Test]
        public async Task Will_Return_False_When_Template_Belongs_To_Other_Tenant()
        {
            var templateId = _templates[0].TemplateId;
            var ct = CancellationToken.None;

            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant2Id).Verifiable();

            var result = await _repository.TemplateExists(templateId, ct);
            result.Should().BeFalse();
        }

        [Test]
        public async Task Will_Return_True_When_TemplateVersion_Found()
        {
            var templateId = _templates[0].TemplateId;
            var templateVersionId = _templates[0].Versions[0].TemplateVersionId;
            var ct = CancellationToken.None;

            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            var result = await _repository.TemplateVersionExists(templateId, templateVersionId, ct);
            result.Should().BeTrue();
        }

        [Test]
        public async Task Will_Return_False_When_TemplateVersion_Template_Not_Found()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = _templates[0].Versions[0].TemplateVersionId;
            var ct = CancellationToken.None;

            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            var result = await _repository.TemplateVersionExists(templateId, templateVersionId, ct);
            result.Should().BeFalse();
        }

        [Test]
        public async Task Will_Return_False_When_TemplateVersion_Not_Found()
        {
            var templateId = _templates[0].TemplateId;
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            var result = await _repository.TemplateVersionExists(templateId, templateVersionId, ct);
            result.Should().BeFalse();
        }

        [Test]
        public async Task Will_Return_False_When_TemplateVersion_Belongs_To_Other_Tenant()
        {
            var templateId = _templates[0].TemplateId;
            var templateVersionId = _templates[0].Versions[0].TemplateVersionId;
            var ct = CancellationToken.None;

            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant2Id).Verifiable();

            var result = await _repository.TemplateVersionExists(templateId, templateVersionId, ct);
            result.Should().BeFalse();
        }

        [Test]
        public async Task Will_Return_True_When_Template_Label_Is_Taken()
        {
            var label = _templates[0].Label;
            var ct = CancellationToken.None;

            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            var result = await _repository.TemplateLabelTaken(label, ct);
            result.Should().BeTrue();
        }

        [Test]
        public async Task Will_Return_False_When_Template_Label_Is_Not_Taken()
        {
            var label = Guid.NewGuid().ToString();
            var ct = CancellationToken.None;

            _tenantIdProvider.Setup(x => x.TenantId).Returns(Tenant1Id).Verifiable();

            var result = await _repository.TemplateLabelTaken(label, ct);
            result.Should().BeFalse();
        }

        [Test]
        public async Task Will_Return_Use_Count_For_Each_Of_TemplateKinds() {
            var kinds = new[] { "tk1", "tk3", "tk4" };
            var ct = CancellationToken.None;

            var result = await _repository.CountTemplatesByTemplateKindKeys(kinds, ct);
            result.Count.Should().Be(3);
            result.FirstOrDefault(x => x.Key == "tk1").Value.Should().Be(4);
            result.FirstOrDefault(x => x.Key == "tk3").Value.Should().Be(1);
            result.FirstOrDefault(x => x.Key == "tk4").Value.Should().Be(0);
        }

        private T Clone<T>(T obj) => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
    }
}