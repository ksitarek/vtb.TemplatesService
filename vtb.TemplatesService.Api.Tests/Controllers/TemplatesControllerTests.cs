using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using vtb.TemplatesService.Api.Controllers;
using vtb.TemplatesService.BusinessLogic.Managers;

namespace vtb.TemplatesService.Api.Tests.Controllers
{
    public abstract class TemplatesControllerTests
    {
        protected TemplatesController _controller;
        protected Mock<ILogger<TemplatesController>> _logger;
        protected Mock<IMapper> _mapper;
        protected Mock<ITemplateManager> _templateManager;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new Mock<ILogger<TemplatesController>>();
            _mapper = new Mock<IMapper>();
            _templateManager = new Mock<ITemplateManager>();
        }

        [SetUp]
        public void SetUp()
        {
            _controller = new TemplatesController(_logger.Object, _mapper.Object, _templateManager.Object);
        }
    }
}