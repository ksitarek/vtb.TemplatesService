using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public class TemplateKindManager_Get : TemplateKindManagerTests
    {
        [Test]
        public async Task Will_Return_TemplateKind()
        {
            var templateKindKey = "tk-1";
            var templateKind = new TemplateKind() { TemplateKindKey = templateKindKey };
            var ct = CancellationToken.None;

            _templateKindsRepositoryMock.Setup(x => x.GetTemplateKind(templateKindKey, ct))
                .ReturnsAsync(templateKind)
                .Verifiable();

            var returnedTemplateKind = await _manager.Get(templateKindKey, ct);
            returnedTemplateKind.Should().BeEquivalentTo(templateKind);

            _templateKindsRepositoryMock.Verify();
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Will_Throw_ArgumentException_On_Invalid_Input(string templateKindKey)
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await _manager.Get(templateKindKey, CancellationToken.None));
        }

        [Test]
        public void Will_Throw_TemplateKindNotFound_When_Not_Found()
        {
            var templateKindKey = "tk-1";
            var ct = CancellationToken.None;

            _templateKindsRepositoryMock.Setup(x => x.GetTemplateKind(templateKindKey, ct))
                .ReturnsAsync(default(TemplateKind))
                .Verifiable();

            Assert.ThrowsAsync<TemplateKindNotFoundException>(async () => await _manager.Get(templateKindKey, ct));

            _templateKindsRepositoryMock.Verify();
        }
    }
}