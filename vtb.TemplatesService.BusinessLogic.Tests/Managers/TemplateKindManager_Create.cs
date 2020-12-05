using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public class TemplateKindManager_Create : TemplateKindManagerTests
    {
        [Test]
        public async Task Stores_Template_Kind_In_Repository()
        {
            var templateKind = new TemplateKind() { TemplateKindKey = "tk-1" };
            var ct = CancellationToken.None;

            _templateKindsRepositoryMock.Setup(x => x.GetTemplateKind(templateKind.TemplateKindKey, ct))
                .ReturnsAsync(default(TemplateKind))
                .Verifiable();

            _templateKindsRepositoryMock.Setup(x => x.AddTemplateKind(templateKind, ct))
                .Verifiable();

            await _manager.Create(templateKind, ct);

            _templateKindsRepositoryMock.Verify();
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Throws_ArgumentException_When_Invalid_TemplateKindKey(string key)
        {
            var templateKind = new TemplateKind() { TemplateKindKey = key };
            var ct = CancellationToken.None;

            Assert.ThrowsAsync<ArgumentException>(async () => await _manager.Create(templateKind, ct));
        }

        [Test]
        public void Throws_TemplateKindKeyAlreadyTakenException_When_Already_Exists()
        {
            var templateKind = new TemplateKind() { TemplateKindKey = "tk-1" };
            var ct = CancellationToken.None;

            _templateKindsRepositoryMock.Setup(x => x.GetTemplateKind(templateKind.TemplateKindKey, ct))
                .ReturnsAsync(templateKind)
                .Verifiable();

            Assert.ThrowsAsync<TemplateKindKeyAlreadyTakenException>(async () => await _manager.Create(templateKind, ct));

            _templateKindsRepositoryMock.Verify();
            _templateKindsRepositoryMock.Verify(x =>
                x.AddTemplateKind(
                    It.IsAny<TemplateKind>(),
                    It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}