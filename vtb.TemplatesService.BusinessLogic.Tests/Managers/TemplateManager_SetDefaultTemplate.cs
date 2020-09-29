using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public class TemplateManager_SetDefaultTemplate : TemplateManagerTests
    {
        [Test]
        public async Task Will_Set_Default_Template_In_Repository()
        {
            var templateKindKey = "tk1";
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateKindsRepositoryMock.Setup(x => x.GetTemplateKind(templateKindKey, ct))
                .ReturnsAsync(new TemplateKind())
                .Verifiable();

            _templatesRepositoryMock.Setup(x => x.TemplateExists(templateId, ct))
                .ReturnsAsync(true)
                .Verifiable();

            _templatesRepositoryMock.Setup(x => x.SetDefaultTemplate(templateKindKey, templateId, ct))
                .Verifiable();

            await _manager.SetDefaultTemplate(templateKindKey, templateId, ct);

            _templatesRepositoryMock.Verify();
            _templateKindsRepositoryMock.Verify();
        }

        [Test]
        public void Will_Throw_SetDefaultTemplateFailedException_When_Repository_Fails()
        {
            var templateKindKey = "tk1";
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateKindsRepositoryMock.Setup(x => x.GetTemplateKind(templateKindKey, ct))
                .ReturnsAsync(new TemplateKind())
                .Verifiable();

            _templatesRepositoryMock.Setup(x => x.TemplateExists(templateId, ct))
                .ReturnsAsync(true)
                .Verifiable();

            _templatesRepositoryMock.Setup(x => x.SetDefaultTemplate(templateKindKey, templateId, ct))
                .Throws(new Exception());

            Assert.ThrowsAsync<SetDefaultTemplateFailedException>(async () => await _manager.SetDefaultTemplate(templateKindKey, templateId, ct));
        }

        [TestCase("", "b0dfe5bf-c621-4de3-a657-b991b9384b0f")]
        [TestCase(" ", "b0dfe5bf-c621-4de3-a657-b991b9384b0f")]
        [TestCase("a", "00000000-0000-0000-0000-000000000000")]
        public void Will_Throw_ArgumentException_For_Invalid_Input(string templateKindKey, string templateIdString)
        {
            var templateId = Guid.Parse(templateIdString);
            var ct = CancellationToken.None;

            Assert.ThrowsAsync<ArgumentException>(async () => await _manager.SetDefaultTemplate(templateKindKey, templateId, ct));
        }

        [Test]
        public void Will_Throw_TemplateKindNotFoundException_When_No_TemplateKind()
        {
            var templateKindKey = "tk1";
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateKindsRepositoryMock.Setup(x => x.GetTemplateKind(templateKindKey, ct))
                .ReturnsAsync(default(TemplateKind));
                
            Assert.ThrowsAsync<TemplateKindNotFoundException>(async () => await _manager.SetDefaultTemplate(templateKindKey, templateId, ct));
        }
        
        [Test]
        public void Will_Throw_TemplateNotFoundException_When_No_Template()
        {
            var templateKindKey = "tk1";
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateKindsRepositoryMock.Setup(x => x.GetTemplateKind(templateKindKey, ct))
                .ReturnsAsync(new TemplateKind())
                .Verifiable();

            _templatesRepositoryMock.Setup(x => x.TemplateExists(templateId, ct))
                .ReturnsAsync(false)
                .Verifiable();

            Assert.ThrowsAsync<TemplateNotFoundException>(async () => await _manager.SetDefaultTemplate(templateKindKey, templateId, ct));
        }
    }
}