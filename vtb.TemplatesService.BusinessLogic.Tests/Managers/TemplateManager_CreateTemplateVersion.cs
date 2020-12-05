using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public class TemplateManager_CreateTemplateVersion : TemplateManagerTests
    {
        [Test]
        public async Task Will_Build_And_Insert_New_Template_Version()
        {
            var templateVersionId = Guid.NewGuid();
            var templateId = Guid.NewGuid();
            var content = "Lorem Ipsum, dolor sit amet";
            var isActive = false;
            var ct = CancellationToken.None;

            var templateBeforeUpdate = new Template()
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
                        TemplateVersionId = new Guid("d013797f-e190-4d28-b9dc-f49905bc3fe4"),
                        Content = "Lorem Ipsum", IsActive = false, Version = 2, CreatedAt = _utcNow
                    }
                },
            };

            var templateToUpdate = new Template()
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
                        TemplateVersionId = new Guid("d013797f-e190-4d28-b9dc-f49905bc3fe4"),
                        Content = "Lorem Ipsum", IsActive = false, Version = 2, CreatedAt = _utcNow
                    },
                    new TemplateVersion()
                    {
                        TemplateVersionId = templateVersionId,
                        Content = content, IsActive = isActive, Version = 3, CreatedAt = _utcNow
                    }
                },
            };

            _templatesRepositoryMock.Setup(x => x.GetTemplateWithAllVersions(templateId, ct))
                .ReturnsAsync(templateBeforeUpdate);

            _templatesRepositoryMock.Setup(x => x.UpdateTemplate(templateToUpdate, ct));

            await _manager.CreateTemplateVersion(templateVersionId, templateId, content, isActive, ct);

            _templatesRepositoryMock.Verify(x => x.GetTemplateWithAllVersions(templateId, ct), Times.Once);
            _templatesRepositoryMock.Setup(x => x.UpdateTemplate(
                    It.Is<Template>(y => JsonConvert.SerializeObject(y) == JsonConvert.SerializeObject(templateToUpdate)),
                    ct
                )
            );
        }

        [Test]
        public async Task Will_Build_And_Insert_New_Active_Template_Version()
        {
            var templateVersionId = Guid.NewGuid();
            var templateId = Guid.NewGuid();
            var content = "Lorem Ipsum, dolor sit amet";
            var isActive = true;
            var ct = CancellationToken.None;

            var templateBeforeUpdate = new Template()
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
                        TemplateVersionId = new Guid("d013797f-e190-4d28-b9dc-f49905bc3fe4"),
                        Content = "Lorem Ipsum", IsActive = false, Version = 2, CreatedAt = _utcNow
                    }
                },
            };

            var templateToUpdate = new Template()
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
                        Content = "Lorem Ipsum", IsActive = false, Version = 1, CreatedAt = _utcNow
                    },
                    new TemplateVersion()
                    {
                        TemplateVersionId = new Guid("d013797f-e190-4d28-b9dc-f49905bc3fe4"),
                        Content = "Lorem Ipsum", IsActive = false, Version = 2, CreatedAt = _utcNow
                    },
                    new TemplateVersion()
                    {
                        TemplateVersionId = templateVersionId,
                        Content = content, IsActive = isActive, Version = 3, CreatedAt = _utcNow
                    }
                },
            };

            _templatesRepositoryMock.Setup(x => x.GetTemplateWithAllVersions(templateId, ct))
                .ReturnsAsync(templateBeforeUpdate);

            _templatesRepositoryMock.Setup(x => x.UpdateTemplate(templateToUpdate, ct));

            await _manager.CreateTemplateVersion(templateVersionId, templateId, content, isActive, ct);

            _templatesRepositoryMock.Verify(x => x.GetTemplateWithAllVersions(templateId, ct), Times.Once);
            _templatesRepositoryMock.Setup(x => x.UpdateTemplate(
                    It.Is<Template>(y => JsonConvert.SerializeObject(y) == JsonConvert.SerializeObject(templateToUpdate)),
                    ct
                )
            );
        }

        [Test]
        public void Will_Throw_TemplateNotFoundException_When_No_Template()
        {
            var templateVersionId = Guid.NewGuid();
            var templateId = Guid.NewGuid();
            var content = "Lorem Ipsum, dolor sit amet";
            var isActive = false;
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.GetTemplateWithAllVersions(templateId, ct))
                .ReturnsAsync(default(Template));

            Assert.ThrowsAsync<TemplateNotFoundException>(async () => await _manager.CreateTemplateVersion(templateVersionId, templateId, content, isActive, ct));
        }

        [Test]
        public void Will_Throw_TemplateVersionCreationFailedException_When_Failed_To_UpdateTemplate()
        {
            var templateVersionId = Guid.NewGuid();
            var templateId = Guid.NewGuid();
            var content = "Lorem Ipsum, dolor sit amet";
            var isActive = false;
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.GetTemplateWithAllVersions(templateId, ct))
                .ReturnsAsync(new Template());
            _templatesRepositoryMock.Setup(x => x.UpdateTemplate(It.IsAny<Template>(), ct))
                .ThrowsAsync(new Exception());

            Assert.ThrowsAsync<TemplateVersionCreationFailedException>(async () => await _manager.CreateTemplateVersion(templateVersionId, templateId, content, isActive, ct));
        }

        [TestCase("00000000-0000-0000-0000-000000000000", "b0dfe5bf-c621-4de3-a657-b991b9384b0f", "LOREM")]
        [TestCase("b0dfe5bf-c621-4de3-a657-b991b9384b0f", "00000000-0000-0000-0000-000000000000", "LOREM")]
        [TestCase("b0dfe5bf-c621-4de3-a657-b991b9384b0f", "b0dfe5bf-c621-4de3-a657-b991b9384b0f", "")]
        [TestCase("b0dfe5bf-c621-4de3-a657-b991b9384b0f", "b0dfe5bf-c621-4de3-a657-b991b9384b0f", null)]
        [TestCase("b0dfe5bf-c621-4de3-a657-b991b9384b0f", "b0dfe5bf-c621-4de3-a657-b991b9384b0f", " ")]
        public void Will_Throw_ArguemntException_For_Invalid_Input(string templateVersionIdString,
            string templateIdString, string content)
        {
            var templateVersionId = Guid.Parse(templateVersionIdString);
            var templateId = Guid.Parse(templateIdString);

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _manager.CreateTemplateVersion(templateVersionId, templateId, content, false, CancellationToken.None));
        }
    }
}