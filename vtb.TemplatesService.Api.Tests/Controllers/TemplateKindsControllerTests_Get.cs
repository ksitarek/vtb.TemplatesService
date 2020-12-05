using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.Controllers
{
    public class TemplateKindsControllerTests_Get : TemplateKindsControllerTests
    {
        [Test]
        public async Task Produces_200OK()
        {
            var templateKindKey = "tk-1";
            var templateKindFromManager = new TemplateKind() { TemplateKindKey = templateKindKey };

            _templateKindManager.Setup(x => x.Get(templateKindKey, CancellationToken.None))
                .ReturnsAsync(templateKindFromManager)
                .Verifiable();

            var result = await _controller.Get(templateKindKey, CancellationToken.None) as ObjectResult;
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            result.Value.Should().BeEquivalentTo(templateKindFromManager);

            _templateKindManager.Verify();
        }

        [Test]
        public async Task Produces_400BadRequest_For_ArgumentException()
        {
            var templateKindKey = "tk-1";

            _templateKindManager.Setup(x => x.Get(templateKindKey, CancellationToken.None))
                .ThrowsAsync(new ArgumentException());

            var result = await _controller.Get(templateKindKey, CancellationToken.None);
            result.Should().BeOfType(typeof(BadRequestResult));
        }

        [Test]
        public async Task Produces_404BadRequest_For_TemplateKindNotFoundException()
        {
            var templateKindKey = "tk-1";

            _templateKindManager.Setup(x => x.Get(templateKindKey, CancellationToken.None))
                .ThrowsAsync(new TemplateKindNotFoundException(templateKindKey));

            var result = await _controller.Get(templateKindKey, CancellationToken.None);
            result.Should().BeOfType(typeof(NotFoundResult));
        }
    }
}