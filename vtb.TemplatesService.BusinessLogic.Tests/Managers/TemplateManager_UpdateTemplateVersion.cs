using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public class TemplateManager_UpdateTemplateVersion : TemplateManagerTests
    {
        [Test]
        public async Task Will_Update_Template_Version_Using_Repository()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var content = "Lorem Ipsum";
            var ct = CancellationToken.None;
            

            _templatesRepositoryMock.Setup(x => x.TemplateExists(templateId, ct)).ReturnsAsync(true);
            _templatesRepositoryMock.Setup(x => x.TemplateVersionExists(templateId, templateVersionId, ct)).ReturnsAsync(true);

            _templatesRepositoryMock.Setup(x => x.UpdateTemplateVersion(templateId, It.IsAny<TemplateVersion>(), ct));

            await _manager.UpdateTemplateVersion(templateId, templateVersionId, content, ct);

            _templatesRepositoryMock.Verify(x => x.UpdateTemplateVersion(templateId,
                It.Is<TemplateVersion>(tv =>
                    tv.TemplateVersionId == templateVersionId &&
                    tv.Content == content &&
                    tv.UpdatedAt == _utcNow
                ), ct));
        }

        [Test]
        public void Will_Throw_TemplateVersionUpdateFailedException_When_Repository_Fails()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var content = "Lorem Ipsum";
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.TemplateExists(templateId, ct)).ReturnsAsync(true);
            _templatesRepositoryMock.Setup(x => x.TemplateVersionExists(templateId, templateVersionId, ct)).ReturnsAsync(true);

            _templatesRepositoryMock.Setup(x => x.UpdateTemplateVersion(templateId, It.IsAny<TemplateVersion>(), ct))
                .Throws(new Exception());

            Assert.ThrowsAsync<TemplateVersionUpdateFailedException>(async () =>
                await _manager.UpdateTemplateVersion(templateId, templateVersionId, content, ct));
        }

        [TestCase("00000000-0000-0000-0000-000000000000", "b0dfe5bf-c621-4de3-a657-b991b9384b0f", "a")]
        [TestCase("b0dfe5bf-c621-4de3-a657-b991b9384b0f", "00000000-0000-0000-0000-000000000000", "a")]
        [TestCase("b0dfe5bf-c621-4de3-a657-b991b9384b0f", "b0dfe5bf-c621-4de3-a657-b991b9384b0f", "")]
        [TestCase("b0dfe5bf-c621-4de3-a657-b991b9384b0f", "b0dfe5bf-c621-4de3-a657-b991b9384b0f", " ")]
        public void Will_Throw_ArgumentException_For_Invalid_Input(string templateIdString,
            string templateVersionIdString, string content)
        {
            var templateId = Guid.Parse(templateIdString);
            var templateVersionId = Guid.Parse(templateVersionIdString);
            
            var ct = CancellationToken.None;

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _manager.UpdateTemplateVersion(templateId, templateVersionId, content, ct));
        }

        [Test]
        public void Will_Throw_TemplateNotFoundException_When_Template_Not_Exists()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var content = "Lorem Ipsum";
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.TemplateExists(templateId, ct)).ReturnsAsync(false);
            _templatesRepositoryMock.Setup(x => x.TemplateVersionExists(templateId, templateVersionId, ct)).ReturnsAsync(true);

            Assert.ThrowsAsync<TemplateNotFoundException>(async () => await _manager.UpdateTemplateVersion(templateId, templateVersionId, content, ct));
        }

        [Test]
        public void Will_Throw_TemplateVersionNotFoundException_When_TemplateVersion_Not_Exists()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var content = "Lorem Ipsum";
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.TemplateExists(templateId, ct)).ReturnsAsync(true);
            _templatesRepositoryMock.Setup(x => x.TemplateVersionExists(templateId, templateVersionId, ct)).ReturnsAsync(false);

            Assert.ThrowsAsync<TemplateVersionNotFoundException>(async () => await _manager.UpdateTemplateVersion(templateId, templateVersionId, content, ct));
        }
    }
}