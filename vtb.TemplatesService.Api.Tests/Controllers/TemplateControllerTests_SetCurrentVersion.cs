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
    public class TemplateControllerTests_SetCurrentVersion : TemplatesControllerTests
    {
        [Test]
        public async Task Produces_202Accepted()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.SetCurrentVersion(templateId, templateVersionId, ct))
                .Verifiable();

            var result = await _controller.SetCurrentVersion(templateId, templateVersionId, ct) as AcceptedAtRouteResult;
            result.Should().NotBeNull();
            result.RouteName.Should().Be("TemplateDetails");
            result.RouteValues["templateId"].Should().Be(templateId);

            _templateManager.Verify();
        }

        [Test]
        public async Task Produces_400BadRequest_On_ArgumentException()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.SetCurrentVersion(templateId, templateVersionId, ct))
                .ThrowsAsync(new ArgumentException());

            var result = await _controller.SetCurrentVersion(templateId, templateVersionId, ct) as BadRequestResult;
            result.Should().NotBeNull();
        }

        [Test]
        public async Task Produces_400NotFound_On_TemplateVersionNotFoundException()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.SetCurrentVersion(templateId, templateVersionId, ct))
                .ThrowsAsync(new TemplateVersionNotFoundException(templateId, templateVersionId));

            var result = await _controller.SetCurrentVersion(templateId, templateVersionId, ct) as BadRequestResult;
            result.Should().NotBeNull();
        }

        [Test]
        public async Task Produces_404NotFound_On_TemplateNotFoundException()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.SetCurrentVersion(templateId, templateVersionId, ct))
                .ThrowsAsync(new TemplateNotFoundException(templateId));

            var result = await _controller.SetCurrentVersion(templateId, templateVersionId, ct) as NotFoundResult;
            result.Should().NotBeNull();
        }
    }
}