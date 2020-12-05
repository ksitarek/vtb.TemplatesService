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
        protected Mock<IMapper> _mapper;
        protected Mock<ITemplateKindManager> _templateKindManager;
        protected TemplateKindsController _controller;

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
    }
}