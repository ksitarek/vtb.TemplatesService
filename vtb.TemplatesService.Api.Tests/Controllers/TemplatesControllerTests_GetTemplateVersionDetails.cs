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
    public class TemplatesControllerTests_GetTemplateVersionDetails : TemplatesControllerTests
    {
        [Test]
        public async Task Produces_200Ok()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var templateVersion = Builder<TemplateVersion>.CreateNew().Build();
            var templateVersionDetails = Builder<TemplateVersionDetails>.CreateNew().Build();

            _templateManager.Setup(x => x.GetTemplateVersion(templateId, templateVersionId, ct))
                .ReturnsAsync(templateVersion)
                .Verifiable();

            _mapper.Setup(x => x.Map<TemplateVersionDetails>(templateVersion))
                .Returns(templateVersionDetails)
                .Verifiable();

            var result = await _controller.GetTemplateVersionDetails(templateId, templateVersionId, ct) as OkObjectResult;
            result.Should().NotBeNull();
            ((OkObjectResult)result).Value.Should().Be(templateVersionDetails);

            _templateManager.Verify();
            _mapper.Verify();
        }

        [Test]
        public async Task Produces_400BadRequest_On_ArgumentException()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var templateVersion = Builder<TemplateVersion>.CreateNew().Build();
            var templateVersionDetails = Builder<TemplateVersionDetails>.CreateNew().Build();

            _templateManager.Setup(x => x.GetTemplateVersion(templateId, templateVersionId, ct))
                .Throws(new ArgumentException());

            var result = await _controller.GetTemplateVersionDetails(templateId, templateVersionId, ct);
            result.Should().BeOfType(typeof(BadRequestResult));
        }

        [Test]
        public async Task Produces_404NotFound_On_TemplateNotFoundException()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var templateVersion = Builder<TemplateVersion>.CreateNew().Build();
            var templateVersionDetails = Builder<TemplateVersionDetails>.CreateNew().Build();

            _templateManager.Setup(x => x.GetTemplateVersion(templateId, templateVersionId, ct))
                .Throws(new TemplateNotFoundException(templateId));

            var result = await _controller.GetTemplateVersionDetails(templateId, templateVersionId, ct);
            result.Should().BeOfType(typeof(NotFoundResult));
        }

        [Test]
        public async Task Produces_404NotFound_On_TemplateVersionNotFoundException()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var templateVersion = Builder<TemplateVersion>.CreateNew().Build();
            var templateVersionDetails = Builder<TemplateVersionDetails>.CreateNew().Build();

            _templateManager.Setup(x => x.GetTemplateVersion(templateId, templateVersionId, ct))
                .Throws(new TemplateVersionNotFoundException(templateId, templateVersionId));

            var result = await _controller.GetTemplateVersionDetails(templateId, templateVersionId, ct);
            result.Should().BeOfType(typeof(NotFoundResult));
        }
    }
}