using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic.Exceptions;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public class TemplateManager_RemoveTemplate : TemplateManagerTests
    {
        [Test]
        public async Task Will_Remove_Template_From_Repository()
        {
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.TemplateExists(templateId, ct)).ReturnsAsync(true)
                .Verifiable();
            _templatesRepositoryMock.Setup(x => x.RemoveTemplate(templateId, ct))
                .Verifiable();

            await _manager.RemoveTemplate(templateId, ct);

            _templatesRepositoryMock.Verify();
        }

        [Test]
        public void Will_Throw_TemplateRemovalFailedException_When_Repository_Fails()
        {
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.TemplateExists(templateId, ct)).ReturnsAsync(true);
            _templatesRepositoryMock.Setup(x => x.RemoveTemplate(It.IsAny<Guid>(), CancellationToken.None))
                .Throws(new Exception());

            Assert.ThrowsAsync<TemplateRemovalFailedException>(async () => await _manager.RemoveTemplate(templateId, ct));
        }

        [Test]
        public void Will_Throw_TemplateNotFoundException_When_Template_Not_Found()
        {
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.TemplateExists(templateId, ct)).ReturnsAsync(false);

            Assert.ThrowsAsync<TemplateNotFoundException>(async () => await _manager.RemoveTemplate(templateId, ct));
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        public void Will_Throw_ArgumentException_For_Invalid_Input(string templateIdString)
        {
            var templateId = Guid.Parse(templateIdString);
            var ct = CancellationToken.None;

            Assert.ThrowsAsync<ArgumentException>(async () => await _manager.RemoveTemplate(templateId, ct));
        }
    }
}