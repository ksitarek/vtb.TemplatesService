using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.Api.Requests;
using vtb.TemplatesService.BusinessLogic.Exceptions;

namespace vtb.TemplatesService.Api.Tests.Controllers
{
    public class TemplatesControllerTests_AddNewTemplateVersion : TemplatesControllerTests
    {
        [Test]
        public async Task Produces_201Created()
        {
            var templateId = Guid.NewGuid();
            var request = new CreateTemplateVersion
            {
                Content = "Lorem Ipsum",
                IsActive = true
            };

            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.CreateTemplateVersion(It.IsAny<Guid>(), templateId, request.Content, request.IsActive, ct));

            var result = await _controller.AddNewTemplateVersion(templateId, request, ct) as CreatedAtRouteResult;
            result.Should().NotBeNull();
            result.RouteName.Should().Be("TemplateVersionDetails");

            var routeTemplateId = Guid.Parse(result.RouteValues["templateId"].ToString());
            routeTemplateId.Should().Be(templateId);

            var templateVersionId = Guid.Parse(result.RouteValues["templateVersionId"].ToString());
            _templateManager.Setup(x => x.CreateTemplateVersion(templateVersionId, templateId, request.Content, request.IsActive, ct));
        }

        [Test]
        public async Task Produces_400BadRequest_On_ArgumentException()
        {
            var templateId = Guid.NewGuid();
            var request = new CreateTemplateVersion
            {
                Content = "Lorem Ipsum",
                IsActive = true
            };

            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.CreateTemplateVersion(It.IsAny<Guid>(), templateId, request.Content, request.IsActive, ct))
                .Throws(new ArgumentException());

            var result = await _controller.AddNewTemplateVersion(templateId, request, ct);
            result.Should().BeOfType(typeof(BadRequestResult));
        }

        [Test]
        public async Task Produces_404NotFound_On_TemplateNotFoundException()
        {
            var templateId = Guid.NewGuid();
            var request = new CreateTemplateVersion
            {
                Content = "Lorem Ipsum",
                IsActive = true
            };

            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.CreateTemplateVersion(It.IsAny<Guid>(), templateId, request.Content, request.IsActive, ct))
                .Throws(new TemplateNotFoundException(templateId));

            var result = await _controller.AddNewTemplateVersion(templateId, request, ct);
            result.Should().BeOfType(typeof(NotFoundResult));
        }

        [Test]
        public async Task Produces_500InternalServerError_On_TemplateVersionCreationFailedException()
        {
            var templateId = Guid.NewGuid();
            var request = new CreateTemplateVersion
            {
                Content = "Lorem Ipsum",
                IsActive = true
            };

            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.CreateTemplateVersion(It.IsAny<Guid>(), templateId, request.Content, request.IsActive, ct))
                .Throws(new TemplateVersionCreationFailedException(templateId, new Exception()));

            var result = await _controller.AddNewTemplateVersion(templateId, request, ct);
            result.Should().BeOfType(typeof(StatusCodeResult));

            ((StatusCodeResult)result).StatusCode.Should().Be(500);
        }
    }
}