using System;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using vtb.TemplatesService.Api.Responses;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.Controllers
{
    public class TemplatesControllerTests_GetDefaultTemplate : TemplatesControllerTests
    {
        [Test]
        public async Task Produces_200Ok()
        {
            var templateKindKey = "tk1";
            var ct = CancellationToken.None;

            var defaultTemplate = Builder<Template>.CreateNew().Build();
            var defaultTemplateDetails = Builder<TemplateDetails>.CreateNew().Build();

            _templateManager.Setup(x => x.GetDefaultTemplate(templateKindKey, ct))
                .ReturnsAsync(defaultTemplate)
                .Verifiable();

            _mapper.Setup(x => x.Map<TemplateDetails>(defaultTemplate))
                .Returns(defaultTemplateDetails)
                .Verifiable();

            var result = await _controller.GetDefaultTemplate(templateKindKey, ct) as OkObjectResult;
            result.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(defaultTemplateDetails);

            _templateManager.Verify();
            _mapper.Verify();
        }

        [Test]
        public async Task Produces_400BadRequest_On_ArgumentException()
        {
            var templateKindKey = "tk1";
            var ct = CancellationToken.None;

            var defaultTemplate = Builder<Template>.CreateNew().Build();
            var defaultTemplateDetails = Builder<TemplateDetails>.CreateNew().Build();

            _templateManager.Setup(x => x.GetDefaultTemplate(templateKindKey, ct))
                .ThrowsAsync(new ArgumentException());

            var result = await _controller.GetDefaultTemplate(templateKindKey, ct) as BadRequestResult;
            result.Should().NotBeNull();
        }

        [Test]
        public async Task Produces_404NotFound_On_TemplateNotFoundException()
        {
            var templateKindKey = "tk1";
            var ct = CancellationToken.None;

            var defaultTemplate = Builder<Template>.CreateNew().Build();
            var defaultTemplateDetails = Builder<TemplateDetails>.CreateNew().Build();

            _templateManager.Setup(x => x.GetDefaultTemplate(templateKindKey, ct))
                .ThrowsAsync(new TemplateNotFoundException(templateKindKey));

            var result = await _controller.GetDefaultTemplate(templateKindKey, ct) as NotFoundResult;
            result.Should().NotBeNull();
        }
    }
}