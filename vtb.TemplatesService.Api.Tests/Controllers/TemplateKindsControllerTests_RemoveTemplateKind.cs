using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic.Exceptions;

namespace vtb.TemplatesService.Api.Tests.Controllers
{
    public class TemplateKindsControllerTests_RemoveTemplateKind : TemplateKindsControllerTests
    {
        [Test]
        public async Task Produces_202Accepted()
        {
            var templateKindKey = "tk-1";
            _templateKindManager.Setup(x => x.Remove(templateKindKey, CancellationToken.None))
                .Verifiable();

            var result = await _controller.RemoveTemplateKind(templateKindKey, CancellationToken.None);
            result.Should().BeOfType(typeof(AcceptedResult));

            _templateKindManager.Verify();
        }

        [Test]
        public async Task Produces_400BadRequest_On_ArgumentException()
        {
            var templateKindKey = "tk-1";
            _templateKindManager.Setup(x => x.Remove(templateKindKey, CancellationToken.None))
                .ThrowsAsync(new ArgumentException());

            var result = await _controller.RemoveTemplateKind(templateKindKey, CancellationToken.None);
            result.Should().BeOfType(typeof(BadRequestResult));
        }

        [Test]
        public async Task Produces_404NotFound_On_TemplateKindNotFoundException()
        {
            var templateKindKey = "tk-1";
            _templateKindManager.Setup(x => x.Remove(templateKindKey, CancellationToken.None))
                .ThrowsAsync(new TemplateKindNotFoundException(templateKindKey));

            var result = await _controller.RemoveTemplateKind(templateKindKey, CancellationToken.None);
            result.Should().BeOfType(typeof(NotFoundResult));
        }

        [Test]
        public async Task Produces_409Conflict_On_TemplateKindInUseException()
        {
            var templateKindKey = "tk-1";
            _templateKindManager.Setup(x => x.Remove(templateKindKey, CancellationToken.None))
                .ThrowsAsync(new TemplateKindInUseException(templateKindKey));

            var result = await _controller.RemoveTemplateKind(templateKindKey, CancellationToken.None);
            result.Should().BeOfType(typeof(ConflictResult));
        }
    }
}