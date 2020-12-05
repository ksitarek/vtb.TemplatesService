using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic.Exceptions;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public class TemplateManager_SetCurrentVersion : TemplateManagerTests
    {
        [Test]
        public async Task Will_Set_CurrentVersion_For_Template_In_Repository()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.TemplateExists(templateId, ct))
                .ReturnsAsync(true)
                .Verifiable();

            _templatesRepositoryMock.Setup(x => x.TemplateVersionExists(templateId, templateVersionId, ct))
                .ReturnsAsync(true)
                .Verifiable();

            _templatesRepositoryMock.Setup(x => x.SetCurrentVersion(templateId, templateVersionId, ct))
                .Verifiable();

            await _manager.SetCurrentVersion(templateId, templateVersionId, ct);

            _templatesRepositoryMock.Verify();
        }

        [TestCase("", "ff8217fd-d986-478c-983e-7198e6ebd66b")]
        [TestCase("a31d42d7-a765-4f9a-bdcd-b17f3494bef3", "")]
        public void Will_Throw_ArgumentException_When_Invalid_Input(string templateIdString, string templateVersionIdString)
        {
            Guid.TryParse(templateIdString, out Guid templateId);
            Guid.TryParse(templateVersionIdString, out Guid templateVersionId);
            var ct = CancellationToken.None;

            Assert.ThrowsAsync<ArgumentException>(async () => await _manager.SetCurrentVersion(templateId, templateVersionId, ct));
        }

        [Test]
        public void Will_Throw_TemplateNotFoundException_When_No_Template()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.TemplateExists(templateId, ct))
                .ReturnsAsync(false)
                .Verifiable();

            Assert.ThrowsAsync<TemplateNotFoundException>(async () => await _manager.SetCurrentVersion(templateId, templateVersionId, ct));
        }

        [Test]
        public void Will_Throw_TemplateVersionNotFoundException_When_No_TemplateVersion()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.TemplateExists(templateId, ct))
                .ReturnsAsync(true)
                .Verifiable();

            _templatesRepositoryMock.Setup(x => x.TemplateVersionExists(templateId, templateVersionId, ct))
                .ReturnsAsync(false)
                .Verifiable();

            Assert.ThrowsAsync<TemplateVersionNotFoundException>(async () => await _manager.SetCurrentVersion(templateId, templateVersionId, ct));
        }

        [Test]
        public void Will_Throw_SetCurrentTemplateVersionFailedException_When_Repository_Fails()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.TemplateExists(templateId, ct))
                .ReturnsAsync(true)
                .Verifiable();

            _templatesRepositoryMock.Setup(x => x.TemplateVersionExists(templateId, templateVersionId, ct))
                .ReturnsAsync(true)
                .Verifiable();

            _templatesRepositoryMock.Setup(x => x.SetCurrentVersion(templateId, templateVersionId, ct))
                .Throws(new Exception());

            Assert.ThrowsAsync<SetCurrentTemplateVersionFailedException>(async () => await _manager.SetCurrentVersion(templateId, templateVersionId, ct));
        }
    }
}