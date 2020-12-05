using Moq;
using NUnit.Framework;
using vtb.TemplatesService.BusinessLogic.Managers;
using vtb.TemplatesService.DataAccess.Repositories;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public abstract class TemplateKindManagerTests
    {
        protected Mock<ITemplatesRepository> _templatesRepositoryMock;
        protected Mock<ITemplateKindsRepository> _templateKindsRepositoryMock;
        protected TemplateKindManager _manager;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _templatesRepositoryMock = new Mock<ITemplatesRepository>();
            _templateKindsRepositoryMock = new Mock<ITemplateKindsRepository>();
        }

        [SetUp]
        public void SetUp()
        {
            _templatesRepositoryMock.Reset();
            _templateKindsRepositoryMock.Reset();

            _manager = new TemplateKindManager(
                _templateKindsRepositoryMock.Object,
                _templatesRepositoryMock.Object);
        }
    }
}