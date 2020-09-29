using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public class TemplateKindManager_Exists : TemplateKindManagerTests
    {
        [Test]
        public async Task Returns_True_When_Found()
        {
            var templateKindKey = "tk-1";
            var ct = CancellationToken.None;

            _templateKindsRepositoryMock.Setup(x => x.GetTemplateKind(templateKindKey, ct))
                .ReturnsAsync(new TemplateKind());

            var result = await _manager.Exists(templateKindKey, ct);
            result.Should().BeTrue();
        }

        [Test]
        public async Task Returns_False_When_Not_Found()
        {
            var templateKindKey = "tk-1";
            var ct = CancellationToken.None;

            _templateKindsRepositoryMock.Setup(x => x.GetTemplateKind(templateKindKey, ct))
                .ReturnsAsync(default(TemplateKind));

            var result = await _manager.Exists(templateKindKey, ct);
            result.Should().BeFalse();
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Throws_ArgumentException_On_Invalid_Input(string key)
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await _manager.Exists(key, CancellationToken.None));
        }
    }
}