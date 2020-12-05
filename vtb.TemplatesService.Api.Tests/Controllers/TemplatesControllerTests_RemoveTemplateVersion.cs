using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using vtb.TemplatesService.BusinessLogic.Exceptions;

namespace vtb.TemplatesService.Api.Tests.Controllers
{
    public class TemplatesControllerTests_RemoveTemplateVersion : TemplatesControllerTests
    {
        [Test]
        public async Task Produces_204NoContent()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.RemoveTemplateVersion(templateId, templateVersionId, ct)).Verifiable();

            var result = await _controller.RemoveTemplateVersion(templateId, templateVersionId, ct) as NoContentResult;
            result.Should().NotBeNull();

            _templateManager.Verify();
        }

        [Test]
        public async Task Produces_400BadRequest_On_ArgumentException()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.RemoveTemplateVersion(templateId, templateVersionId, ct))
                .ThrowsAsync(new ArgumentException());

            var result = await _controller.RemoveTemplateVersion(templateId, templateVersionId, ct) as BadRequestResult;
            result.Should().NotBeNull();
        }

        [Test]
        public async Task Produces_404NotFound_On_TemplateNotFoundException()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.RemoveTemplateVersion(templateId, templateVersionId, ct))
                .ThrowsAsync(new TemplateNotFoundException(templateId));

            var result = await _controller.RemoveTemplateVersion(templateId, templateVersionId, ct) as NotFoundResult;
            result.Should().NotBeNull();
        }

        [Test]
        public async Task Produces_404NotFound_On_TemplateVersionNotFoundException()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.RemoveTemplateVersion(templateId, templateVersionId, ct))
                .ThrowsAsync(new TemplateVersionNotFoundException(templateId, templateVersionId));

            var result = await _controller.RemoveTemplateVersion(templateId, templateVersionId, ct) as NotFoundResult;
            result.Should().NotBeNull();
        }
    }
}