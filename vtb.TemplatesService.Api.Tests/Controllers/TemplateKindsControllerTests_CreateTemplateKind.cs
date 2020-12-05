using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.Controllers
{
    public class TemplateKindsControllerTests_CreateTemplateKind : TemplateKindsControllerTests
    {
        [Test]
        public async Task Produces_201Created()
        {
            var templateKindKey = "tk-1";

            var result = await _controller.CreateTemplateKind(templateKindKey, CancellationToken.None) as CreatedAtRouteResult;
            result.RouteName.Should().Be("Get");
            result.RouteValues.Should().BeEquivalentTo(new Dictionary<string, string> { { "TemplateKindKey", templateKindKey } });

            _templateKindManager.Verify(x => x.Create(
                    It.Is<TemplateKind>(tk => tk.TemplateKindKey == templateKindKey),
                    CancellationToken.None
                ), Times.Once);
        }

        [Test]
        public async Task Produces_400BadRequest_For_ArgumentException()
        {
            var templateKindKey = "tk-1";

            _templateKindManager.Setup(x => x.Create(It.IsAny<TemplateKind>(), CancellationToken.None))
                .ThrowsAsync(new ArgumentException());

            var result = await _controller.CreateTemplateKind(templateKindKey, CancellationToken.None);
            result.Should().BeOfType(typeof(BadRequestResult));
        }

        [Test]
        public async Task Produces_409Conflict_For_TemplateKindKeyAlreadyTakenException()
        {
            var templateKindKey = "tk-1";

            _templateKindManager.Setup(x => x.Create(It.IsAny<TemplateKind>(), CancellationToken.None))
                .ThrowsAsync(new TemplateKindKeyAlreadyTakenException(templateKindKey));

            var result = await _controller.CreateTemplateKind(templateKindKey, CancellationToken.None);
            result.Should().BeOfType(typeof(ConflictResult));
        }
    }
}