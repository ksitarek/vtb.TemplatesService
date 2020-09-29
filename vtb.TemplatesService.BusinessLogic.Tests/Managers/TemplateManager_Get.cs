using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public class TemplateManager_Get : TemplateManagerTests
    {
        [Test]
        public async Task Will_Return_Template()
        {
            var templateId = Guid.NewGuid();
            var template = new Template() { TemplateId = templateId };
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.GetTemplateWithActiveVersionOnly(templateId, ct))
                .ReturnsAsync(template)
                .Verifiable();

            var returnedTemplate = await _manager.Get(templateId, ct);
            returnedTemplate.Should().BeEquivalentTo(template);

            _templatesRepositoryMock.Verify();
        }

        [TestCase(null)]
        [TestCase("00000000-0000-0000-0000-000000000000")]
        public void Will_Throw_ArgumentException_On_Invalid_Input(Guid templateId)
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await _manager.Get(templateId, CancellationToken.None));
        }

        [Test]
        public void Will_Throw_TemplateNotFoundException_When_Not_Found()
        {
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.GetTemplateWithActiveVersionOnly(templateId, ct))
                .ReturnsAsync(default(Template))
                .Verifiable();

            Assert.ThrowsAsync<TemplateNotFoundException>(async () => await _manager.Get(templateId, ct));

            _templatesRepositoryMock.Verify();
        }
    }
}