using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using vtb.TemplatesService.Api.Controllers;
using vtb.TemplatesService.BusinessLogic.Managers;

namespace vtb.TemplatesService.Api.Tests.Controllers
{
    public class TemplateKindsControllerTests_Exists
    {
        private Mock<ILogger<TemplateKindsController>> _loggerMock;
        private Mock<IMapper> _mapper;
        private Mock<ITemplateKindManager> _templateKindManager;
        private TemplateKindsController _controller;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mapper = new Mock<IMapper>();
            _templateKindManager = new Mock<ITemplateKindManager>();
        }

        [SetUp]
        public void SetUp()
        {
            _controller = new TemplateKindsController(
                _mapper.Object,
                _templateKindManager.Object);
        }

        [Test]
        public async Task Produces_200OK_With_True()
        {
            var templateKindKey = "tk-1";
            _templateKindManager.Setup(x => x.Exists(templateKindKey, CancellationToken.None))
                .ReturnsAsync(true)
                .Verifiable();

            var result = await _controller.Exists(templateKindKey, CancellationToken.None) as OkObjectResult;
            result.Value.Should().Be(true);

            _templateKindManager.Verify();
        }

        [Test]
        public async Task Produces_200OK_With_False()
        {
            var templateKindKey = "tk-1";
            _templateKindManager.Setup(x => x.Exists(templateKindKey, CancellationToken.None))
                .ReturnsAsync(false)
                .Verifiable();

            var result = await _controller.Exists(templateKindKey, CancellationToken.None) as OkObjectResult;
            result.Value.Should().Be(false);

            _templateKindManager.Verify();
        }

        [Test]
        public async Task Produces_400BadRequest_On_ArgumentException()
        {
            var templateKindKey = "tk-1";
            _templateKindManager.Setup(x => x.Exists(templateKindKey, CancellationToken.None))
                .ThrowsAsync(new ArgumentException());

            var result = await _controller.Exists(templateKindKey, CancellationToken.None);
            result.Should().BeOfType(typeof(BadRequestResult));
        }
    }
}