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
    public class TemplatesControllerTests_RemoveTemplate : TemplatesControllerTests
    {
        [Test]
        public async Task Produces_204NoContent()
        {
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.RemoveTemplate(templateId, ct)).Verifiable();

            var result = await _controller.RemoveTemplate(templateId, ct) as NoContentResult;
            result.Should().NotBeNull();

            _templateManager.Verify();
        }

        [Test]
        public async Task Produces_400BadRequest_On_ArgumentException()
        {
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.RemoveTemplate(templateId, ct))
                .ThrowsAsync(new ArgumentException());

            var result = await _controller.RemoveTemplate(templateId, ct) as BadRequestResult;
            result.Should().NotBeNull();
        }

        [Test]
        public async Task Produces_404NotFound_On_TemplateNotFoundException()
        {
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.RemoveTemplate(templateId, ct))
                .ThrowsAsync(new TemplateNotFoundException(templateId));

            var result = await _controller.RemoveTemplate(templateId, ct) as NotFoundResult;
            result.Should().NotBeNull();
        }
    }
}