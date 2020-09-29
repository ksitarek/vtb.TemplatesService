using Microsoft.AspNetCore.Authentication;
using Moq;
using NUnit.Framework;
using System;
using vtb.TemplatesService.BusinessLogic.Managers;
using vtb.TemplatesService.DataAccess.Repositories;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public abstract class TemplateManagerTests
    {
        protected Mock<ITemplatesRepository> _templatesRepositoryMock;
        protected Mock<ITemplateKindsRepository> _templateKindsRepositoryMock;
        protected Mock<ISystemClock> _systemClockMock;
        protected TemplateManager _manager;
        protected readonly DateTimeOffset _utcNow = new DateTimeOffset(DateTime.UtcNow);

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _templatesRepositoryMock = new Mock<ITemplatesRepository>();
            _templateKindsRepositoryMock = new Mock<ITemplateKindsRepository>();

            _systemClockMock = new Mock<ISystemClock>();
        }

        [SetUp]
        public void SetUp()
        {
            _templatesRepositoryMock.Reset();
            _templateKindsRepositoryMock.Reset();
            _systemClockMock.Setup(x => x.UtcNow).Returns(_utcNow).Verifiable();

            _manager = new TemplateManager(
                _templatesRepositoryMock.Object,
                _templateKindsRepositoryMock.Object,
                _systemClockMock.Object);
        }
    }
}