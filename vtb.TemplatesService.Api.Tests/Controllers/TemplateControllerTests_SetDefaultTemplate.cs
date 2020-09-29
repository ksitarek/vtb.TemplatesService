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
    public class TemplateControllerTests_SetDefaultTemplate : TemplatesControllerTests
    {
        [Test]
        public async Task Produces_202Accepted()
        {
            var templateKindKey = "tk1";
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.SetDefaultTemplate(templateKindKey, templateId, ct))
                .Verifiable();

            var result = await _controller.SetDefaultTemplate(templateKindKey, templateId, ct) as AcceptedAtRouteResult;
            result.Should().NotBeNull();
            result.RouteName.Should().Be("DefaultTemplateDetails");
            result.RouteValues["templateKindKey"].Should().Be(templateKindKey);
        }

        [Test]
        public async Task Produces_400BadRequest_On_ArgumentException()
        {
            var templateKindKey = "tk1";
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.SetDefaultTemplate(templateKindKey, templateId, ct))
                .ThrowsAsync(new ArgumentException());

            var result = await _controller.SetDefaultTemplate(templateKindKey, templateId, ct) as BadRequestResult;
            result.Should().NotBeNull();
        }

        [Test]
        public async Task Produces_400BadRequest_On_TemplateKindNotFoundException()
        {
            var templateKindKey = "tk1";
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.SetDefaultTemplate(templateKindKey, templateId, ct))
                .ThrowsAsync(new TemplateKindNotFoundException(templateKindKey));

            var result = await _controller.SetDefaultTemplate(templateKindKey, templateId, ct) as BadRequestResult;
            result.Should().NotBeNull();
        }

        [Test]
        public async Task Produces_400BadRequest_On_TemplateNotFoundException()
        {
            var templateKindKey = "tk1";
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.SetDefaultTemplate(templateKindKey, templateId, ct))
                .ThrowsAsync(new TemplateNotFoundException(templateId));

            var result = await _controller.SetDefaultTemplate(templateKindKey, templateId, ct) as BadRequestResult;
            result.Should().NotBeNull();
        }
    }
}