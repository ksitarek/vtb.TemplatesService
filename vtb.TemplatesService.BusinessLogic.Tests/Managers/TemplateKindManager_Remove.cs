using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public class TemplateKindManager_Remove : TemplateKindManagerTests
    {
        [Test]
        public async Task Will_Remove_TemplateKind_From_Repository()
        {
            var templateKindKey = "tk-1";
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(X => X.CountTemplatesByTemplateKindKey(templateKindKey, ct))
                .ReturnsAsync(0)
                .Verifiable();

            _templateKindsRepositoryMock.Setup(x => x.GetTemplateKind(templateKindKey, ct))
                .ReturnsAsync(new TemplateKind());

            _templateKindsRepositoryMock.Setup(x => x.RemoveTemplateKind(templateKindKey, ct))
                .Verifiable();

            await _manager.Remove(templateKindKey, ct);

            _templateKindsRepositoryMock.Verify();
            _templatesRepositoryMock.Verify();
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Will_Throw_For_Invalid_Input(string key)
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await _manager.Remove(key, CancellationToken.None));
        }

        [Test]
        public void Will_Throw_TemplateKindInUseException_If_TemplateKind_Being_Used()
        {
            var templateKindKey = "tk-1";
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(X => X.CountTemplatesByTemplateKindKey(templateKindKey, ct))
                .ReturnsAsync(1)
                .Verifiable();

            Assert.ThrowsAsync<TemplateKindInUseException>(async () => await _manager.Remove(templateKindKey, ct));

            _templatesRepositoryMock.Verify();
        }

        [Test]
        public void Will_Throw_TemplateKindNotFoundException_If_NotFound()
        {
            var templateKindKey = "tk-1";
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(X => X.CountTemplatesByTemplateKindKey(templateKindKey, ct))
                .ReturnsAsync(0)
                .Verifiable();

            _templateKindsRepositoryMock.Setup(x => x.GetTemplateKind(templateKindKey, ct))
                .ReturnsAsync(default(TemplateKind));

            Assert.ThrowsAsync<TemplateKindNotFoundException>(async () => await _manager.Remove(templateKindKey, ct));

            _templateKindsRepositoryMock.Verify();
            _templatesRepositoryMock.Verify();
        }
    }
}