using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public class TemplateManager_GetTemplateVersion : TemplateManagerTests
    {
        [Test]
        public async Task Will_Return_Version_From_Repository()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var templateFromRepository = new Template()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                TenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f"),
                Versions = new List<TemplateVersion>()
                {
                    new TemplateVersion()
                    {
                        TemplateVersionId = new Guid("3c4fa624-b7b2-4c89-80f0-d7277332b3a2"),
                        Content = "Lorem Ipsum", IsActive = true, Version = 1, CreatedAt = _utcNow
                    },
                    new TemplateVersion()
                    {
                        TemplateVersionId = templateVersionId,
                        Content = "Lorem Ipsum", IsActive = false, Version = 2, CreatedAt = _utcNow
                    }
                },
            };

            _templatesRepositoryMock.Setup(x => x.GetTemplateWithAllVersions(templateId, ct))
                .ReturnsAsync(templateFromRepository)
                .Verifiable();

            var returnedVersion = await _manager.GetTemplateVersion(templateId, templateVersionId, ct);
            returnedVersion.Should().BeEquivalentTo(templateFromRepository.Versions[1]);

            _templatesRepositoryMock.Verify();
        }

        [Test]
        public void Will_Throw_TemplateVersionFoundException_When_Version_Not_Found_In_Template()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var templateFromRepository = new Template()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                TenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f"),
                Versions = new List<TemplateVersion>()
                {
                    new TemplateVersion()
                    {
                        TemplateVersionId = new Guid("3c4fa624-b7b2-4c89-80f0-d7277332b3a2"),
                        Content = "Lorem Ipsum", IsActive = true, Version = 1, CreatedAt = _utcNow
                    },
                    new TemplateVersion()
                    {
                        TemplateVersionId = Guid.NewGuid(),
                        Content = "Lorem Ipsum", IsActive = false, Version = 2, CreatedAt = _utcNow
                    }
                },
            };

            _templatesRepositoryMock.Setup(x => x.GetTemplateWithAllVersions(templateId, ct))
                .ReturnsAsync(templateFromRepository)
                .Verifiable();

            Assert.ThrowsAsync<TemplateVersionNotFoundException>(async () => await _manager.GetTemplateVersion(templateId, templateVersionId, ct));

            _templatesRepositoryMock.Verify();
        }

        [Test]
        public void Will_Throw_TemplateNotFoundException_When_Version_Not_Found_In_Template()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.GetTemplateWithAllVersions(templateId, ct))
                .ReturnsAsync(default(Template));

            Assert.ThrowsAsync<TemplateNotFoundException>(async () => await _manager.GetTemplateVersion(templateId, templateVersionId, ct));
        }

        [TestCase("00000000-0000-0000-0000-000000000000", "b0dfe5bf-c621-4de3-a657-b991b9384b0f")]
        [TestCase("b0dfe5bf-c621-4de3-a657-b991b9384b0f", "00000000-0000-0000-0000-000000000000")]
        public void Will_Throw_ArguemntException_For_Invalid_Input(string templateVersionIdString,
            string templateIdString)
        {
            var templateVersionId = Guid.Parse(templateVersionIdString);
            var templateId = Guid.Parse(templateIdString);
            Assert.ThrowsAsync<ArgumentException>(async () => await _manager.GetTemplateVersion(templateId, templateVersionId, CancellationToken.None));
        }
    }
}