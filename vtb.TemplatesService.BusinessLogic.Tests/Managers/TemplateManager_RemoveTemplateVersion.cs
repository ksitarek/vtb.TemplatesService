using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using MongoDB.Driver;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public class TemplateManager_RemoveTemplateVersion : TemplateManagerTests
    {
        [Test]
        public async Task Will_Remove_Template_From_Repository()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var templateVersion = Builder<TemplateVersion>.CreateNew()
                .With(x => x.TemplateVersionId = Guid.NewGuid())
                .With(x => x.IsActive = true)
                .Build();
            
            var template = Builder<Template>.CreateNew()
                .With(x => x.TemplateId = templateId)
                .With(x => x.Versions = new List<TemplateVersion>() {templateVersion})
                .Build();

            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.GetTemplateWithActiveVersionOnly(template.TemplateId, ct))
                .ReturnsAsync(template).Verifiable();
            _templatesRepositoryMock.Setup(x => x.TemplateVersionExists(template.TemplateId, templateVersionId, ct)).ReturnsAsync(true).Verifiable();
            _templatesRepositoryMock.Setup(x => x.RemoveTemplateVersion(template.TemplateId, templateVersionId, ct))
                .Verifiable();

            await _manager.RemoveTemplateVersion(template.TemplateId, templateVersionId, ct);

            _templatesRepositoryMock.Verify();
        }
        
        
        [Test]
        public void Will_Throw_CannotRemoveActiveTemplateVersion_When_Version_Is_Active()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var templateVersion = Builder<TemplateVersion>.CreateNew()
                .With(x => x.TemplateVersionId = templateVersionId)
                .With(x => x.IsActive = true)
                .Build();
            
            var template = Builder<Template>.CreateNew()
                .With(x => x.TemplateId = templateId)
                .With(x => x.Versions = new List<TemplateVersion>() {templateVersion})
                .Build();

            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.GetTemplateWithActiveVersionOnly(template.TemplateId, ct))
                .ReturnsAsync(template).Verifiable();

            Assert.ThrowsAsync<CannotRemoveActiveTemplateVersion>(async () => await _manager.RemoveTemplateVersion(template.TemplateId, templateVersionId, ct));

            _templatesRepositoryMock.Verify();
        }

        [Test]
        public void Will_Throw_TemplateVersionRemovalFailedException_When_Repository_Fails()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var templateVersion = Builder<TemplateVersion>.CreateNew()
                .With(x => x.TemplateVersionId = Guid.NewGuid())
                .With(x => x.IsActive = true)
                .Build();
            
            var template = Builder<Template>.CreateNew()
                .With(x => x.TemplateId = templateId)
                .With(x => x.Versions = new List<TemplateVersion>() {templateVersion})
                .Build();

            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.GetTemplateWithActiveVersionOnly(template.TemplateId, ct))
                .ReturnsAsync(template).Verifiable();
            _templatesRepositoryMock.Setup(x => x.TemplateVersionExists(templateId, templateVersionId, ct)).ReturnsAsync(true).Verifiable();
            _templatesRepositoryMock.Setup(x => x.RemoveTemplateVersion(templateId, templateVersionId, ct))
                .Throws(new Exception());

            Assert.ThrowsAsync<TemplateVersionRemovalFailedException>(async () => await _manager.RemoveTemplateVersion(templateId, templateVersionId, ct));
        }

        [TestCase("00000000-0000-0000-0000-000000000000", "b0dfe5bf-c621-4de3-a657-b991b9384b0f")]
        [TestCase("b0dfe5bf-c621-4de3-a657-b991b9384b0f", "00000000-0000-0000-0000-000000000000")]
        public void Will_Throw_ArgumentException_For_Invalid_Input(string templateIdString, string templateVersionIdString)
        {
            var templateId = Guid.Parse(templateIdString);
            var templateVersionId = Guid.Parse(templateVersionIdString);
            var ct = CancellationToken.None;

            Assert.ThrowsAsync<ArgumentException>(async () => await _manager.RemoveTemplateVersion(templateId, templateVersionId, ct));
        }

        [Test]
        public void Will_Throw_TemplateNotFoundException_When_Template_Not_Found()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();

            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.GetTemplateWithActiveVersionOnly(templateId, ct))
                .ReturnsAsync(default(Template)).Verifiable();
            _templatesRepositoryMock.Setup(x => x.TemplateVersionExists(templateId, templateVersionId, ct)).ReturnsAsync(true).Verifiable();

            Assert.ThrowsAsync<TemplateNotFoundException>(async () => await _manager.RemoveTemplateVersion(templateId, templateVersionId, ct));
        }

        [Test]
        public void Will_Throw_TemplateVersionNotFoundException_When_Template_Version_Not_Found()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var templateVersion = Builder<TemplateVersion>.CreateNew()
                .With(x => x.TemplateVersionId = Guid.NewGuid())
                .With(x => x.IsActive = true)
                .Build();
            
            var template = Builder<Template>.CreateNew()
                .With(x => x.TemplateId = templateId)
                .With(x => x.Versions = new List<TemplateVersion>() {templateVersion})
                .Build();

            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.GetTemplateWithActiveVersionOnly(template.TemplateId, ct))
                .ReturnsAsync(template).Verifiable();
            _templatesRepositoryMock.Setup(x => x.TemplateVersionExists(templateId, templateVersionId, ct)).ReturnsAsync(false).Verifiable();

            Assert.ThrowsAsync<TemplateVersionNotFoundException>(async () => await _manager.RemoveTemplateVersion(templateId, templateVersionId, ct));
        }
    }
}