﻿using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using vtb.TemplatesService.Api.Requests;
using vtb.TemplatesService.BusinessLogic.Exceptions;

namespace vtb.TemplatesService.Api.Tests.Controllers
{
    public class TemplatesControllerTests_UpdateTemplateVersion : TemplatesControllerTests
    {
        [Test]
        public async Task Produces_202Accepted()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var content = "lorem ipsum";
            var isActive = true;
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.UpdateTemplateVersion(templateId, templateVersionId, content, isActive, ct))
                .Verifiable();

            var request = new UpdateTemplateVersion()
            {
                TemplateId = templateId,
                TemplateVersionId = templateVersionId,
                Body = new UpdateTemplateVersion.UpdateTemplateVersionBody
                {
                    Content = content,
                    IsActive = isActive
                }
            };

            var result = await _controller.UpdateTemplateVersion(request, ct) as AcceptedAtRouteResult;
            result.Should().NotBeNull();
            result.RouteName.Should().Be("TemplateVersionDetails");
            result.RouteValues["templateId"].Should().Be(templateId);
            result.RouteValues["templateVersionId"].Should().Be(templateVersionId);

            _templateManager.Verify();
        }

        [Test]
        public async Task Produces_400BadRequest_On_ArgumentException()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var content = "lorem ipsum";
            var isActive = true;
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.UpdateTemplateVersion(templateId, templateVersionId, content, isActive, ct))
                .ThrowsAsync(new ArgumentException());

            var request = new UpdateTemplateVersion()
            {
                TemplateId = templateId,
                TemplateVersionId = templateVersionId,
                Body = new UpdateTemplateVersion.UpdateTemplateVersionBody
                {
                    Content = content,
                    IsActive = isActive
                }
            };
            var result = await _controller.UpdateTemplateVersion(request, ct);
            result.Should().BeOfType(typeof(BadRequestResult));
        }

        [Test]
        public async Task Produces_404NotFound_On_TemplateNotFoundException()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var content = "lorem ipsum";
            var isActive = true;
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.UpdateTemplateVersion(templateId, templateVersionId, content, isActive, ct))
                .ThrowsAsync(new TemplateNotFoundException(templateId));

            var request = new UpdateTemplateVersion()
            {
                TemplateId = templateId,
                TemplateVersionId = templateVersionId,
                Body = new UpdateTemplateVersion.UpdateTemplateVersionBody
                {
                    Content = content,
                    IsActive = isActive
                }
            };
            var result = await _controller.UpdateTemplateVersion(request, ct);
            result.Should().BeOfType(typeof(NotFoundResult));
        }

        [Test]
        public async Task Produces_404NotFound_On_TemplateVersionNotFoundException()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var content = "lorem ipsum";
            var isActive = true;
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.UpdateTemplateVersion(templateId, templateVersionId, content, isActive, ct))
                .ThrowsAsync(new TemplateVersionNotFoundException(templateId, templateVersionId));

            var request = new UpdateTemplateVersion()
            {
                TemplateId = templateId,
                TemplateVersionId = templateVersionId,
                Body = new UpdateTemplateVersion.UpdateTemplateVersionBody
                {
                    Content = content,
                    IsActive = isActive
                }
            };
            var result = await _controller.UpdateTemplateVersion(request, ct);
            result.Should().BeOfType(typeof(NotFoundResult));
        }
    }
}