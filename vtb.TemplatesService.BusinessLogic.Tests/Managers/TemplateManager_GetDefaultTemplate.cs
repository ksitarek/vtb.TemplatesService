using FizzWare.NBuilder;
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
    public class TemplateManager_GetDefaultTemplate : TemplateManagerTests
    {
        [Test]
        public async Task Will_Retrieve_Default_Template_From_Repository()
        {
            var templateKindKey = "tk1";
            var ct = CancellationToken.None;

            var templateFromRepository = Builder<Template>.CreateNew().Build();

            _templatesRepositoryMock.Setup(x => x.GetDefaultTemplate(templateKindKey, ct))
                .ReturnsAsync(templateFromRepository);

            var result = await _manager.GetDefaultTemplate(templateKindKey, ct);
            result.Should().BeEquivalentTo(templateFromRepository);

            _templatesRepositoryMock.Verify();
        }

        [Test]
        public void Will_Throw_TemplateNotFoundException_When_Not_Found()
        {
            var templateKindKey = "tk1";
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.GetDefaultTemplate(templateKindKey, ct))
                .ReturnsAsync(default(Template));

            Assert.ThrowsAsync<TemplateNotFoundException>(async () => await _manager.GetDefaultTemplate(templateKindKey, ct));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public void Will_Throw_ArgumentException_When_Invalid_Input(string templateKindKey)
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await _manager.GetDefaultTemplate(templateKindKey, CancellationToken.None));
        }
    }
}