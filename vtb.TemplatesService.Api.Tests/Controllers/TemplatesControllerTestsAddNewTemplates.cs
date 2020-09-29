using System;
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
    public class TemplatesControllerTestsAddNewTemplates : TemplatesControllerTests
    {
        [Test]
        public async Task Produces_201Created()
        {
            var request = new CreateTemplate()
            {
                TemplateKindKey = "tk1",
                TemplateLabel = "Lipsum",
                Content = "Lorem ipsum, dolor sit amet"
            };
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.CreateTemplate(It.IsAny<Guid>(), request.TemplateKindKey, request.TemplateLabel, request.Content, ct))
                .Verifiable();

            var result = await _controller.AddNewTemplate(request, CancellationToken.None) as CreatedAtRouteResult;
            result.Should().NotBeNull();
            result.RouteName.Should().Be("TemplateDetails");

            var templateId = Guid.Parse(result.RouteValues["templateId"].ToString());
            _templateManager.Verify(x => x.CreateTemplate(templateId, request.TemplateKindKey, request.TemplateLabel, request.Content, ct));
        }

        [Test]
        public async Task Produces_400BadRequest_On_ArgumentException()
        {
            var request = new CreateTemplate()
            {
                TemplateKindKey = "tk1",
                TemplateLabel = "Lipsum",
                Content = "Lorem ipsum, dolor sit amet"
            };
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.CreateTemplate(It.IsAny<Guid>(), request.TemplateKindKey, request.TemplateLabel, request.Content, ct))
                .Throws(new ArgumentException());

            var result = await _controller.AddNewTemplate(request, CancellationToken.None);
            result.Should().BeOfType(typeof(BadRequestResult));
        }

        [Test]
        public async Task Produces_500InternalServerError_When_TemplateCreationFailedException()
        {
            var request = new CreateTemplate()
            {
                TemplateKindKey = "tk1",
                TemplateLabel = "Lipsum",
                Content = "Lorem ipsum, dolor sit amet"
            };
            var ct = CancellationToken.None;

            _templateManager.Setup(x => x.CreateTemplate(It.IsAny<Guid>(), request.TemplateKindKey, request.TemplateLabel, request.Content, ct))
                .Throws(new TemplateCreationFailedException(request.TemplateKindKey, request.TemplateLabel, new Exception()));

            var result = await _controller.AddNewTemplate(request, CancellationToken.None);
            result.Should().BeOfType(typeof(StatusCodeResult));
            ((StatusCodeResult)result).StatusCode.Should().Be(500);
        }
    }
}