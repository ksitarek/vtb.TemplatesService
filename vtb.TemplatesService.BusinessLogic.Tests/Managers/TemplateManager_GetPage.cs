using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using vtb.TemplatesService.DataAccess;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public class TemplateManager_GetPage : TemplateManagerTests
    {
        [Test]
        public async Task Will_Return_Page_Of_Templates()
        {
            var page = 1;
            var pageSize = 2;
            var ct = CancellationToken.None;

            var templatesPage = new Page<Template>(2, new List<Template>()
            {
                new Template() { TemplateId = Guid.NewGuid(), TemplateKindKey = "tk1", Label = "Tenant 1 Template Kind 1 Template 1", TenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f") },
                new Template() { TemplateId = Guid.NewGuid(), TemplateKindKey = "tk1", Label = "Tenant 1 Template Kind 1 Template 2", TenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f") },
            });

            _templatesRepositoryMock.Setup(x => x.GetTemplatesPage(page, pageSize, ct))
                .ReturnsAsync(templatesPage)
                .Verifiable();

            var result = await _manager.GetPage(page, pageSize, ct);
            result.Should().BeEquivalentTo(templatesPage);

            _templatesRepositoryMock.Verify();
        }

        [TestCase(0, 1)]
        [TestCase(-1, 1)]
        [TestCase(1, 0)]
        [TestCase(1, -1)]
        public void Will_Throw_ArgumentException_For_Invalid_Input(int page, int pageSize)
        {
            var ct = CancellationToken.None;
            Assert.ThrowsAsync<ArgumentException>(async () => await _manager.GetPage(page, pageSize, ct));
        }
    }
}