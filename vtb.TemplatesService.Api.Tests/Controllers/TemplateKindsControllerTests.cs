using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using vtb.TemplatesService.Api.Controllers;
using vtb.TemplatesService.BusinessLogic.Managers;

namespace vtb.TemplatesService.Api.Tests.Controllers
{
    public abstract class TemplateKindsControllerTests
    {
        protected Mock<ILogger<TemplateKindsController>> _loggerMock;
        protected Mock<IMapper> _mapper;
        protected Mock<ITemplateKindManager> _templateKindManager;
        protected TemplateKindsController _controller;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _loggerMock = new Mock<ILogger<TemplateKindsController>>();
            _mapper = new Mock<IMapper>();
            _templateKindManager = new Mock<ITemplateKindManager>();
        }

        [SetUp]
        public void SetUp()
        {
            _controller = new TemplateKindsController(
                _loggerMock.Object,
                _mapper.Object,
                _templateKindManager.Object);
        }
    }
}