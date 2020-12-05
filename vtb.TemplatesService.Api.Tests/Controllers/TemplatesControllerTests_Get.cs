using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.Api.Responses;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.Controllers
{
    public class TemplatesControllerTests_Get : TemplatesControllerTests
    {
        [Test]
        public async Task Produces_200OK()
        {
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;
            var templateFromManager = new Template() { TemplateId = templateId };
            var templateDetailsFromMapper = new TemplateDetails() { TemplateId = templateId };

            _templateManager.Setup(x => x.Get(templateId, ct))
                .ReturnsAsync(templateFromManager)
                .Verifiable();

            _mapper.Setup(x => x.Map<TemplateDetails>(templateFromManager))
                .Returns(templateDetailsFromMapper)
                .Verifiable();

            var result = await _controller.Get(templateId, ct) as ObjectResult;
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            result.Value.Should().BeEquivalentTo(templateDetailsFromMapper);

            _templateManager.Verify();
            _mapper.Verify();
        }

        [Test]
        public async Task Produces_400BadRequest_For_ArgumentException()
        {
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.Get(templateId, ct))
                .ThrowsAsync(new ArgumentException());

            var result = await _controller.Get(templateId, ct);
            result.Should().BeOfType(typeof(BadRequestResult));
        }

        [Test]
        public async Task Produces_404NotFound_For_TemplateNotFoundException()
        {
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.Get(templateId, ct))
                .ThrowsAsync(new TemplateNotFoundException(templateId));

            var result = await _controller.Get(templateId, ct);
            result.Should().BeOfType(typeof(NotFoundResult));
        }
    }
}