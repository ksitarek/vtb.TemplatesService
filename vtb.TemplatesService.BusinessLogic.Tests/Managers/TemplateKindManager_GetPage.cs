using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.DataAccess;
using vtb.TemplatesService.DataAccess.DTOs;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public class TemplateKindManager_GetPage : TemplateKindManagerTests
    {
        [Test]
        public async Task Will_Return_Page_Of_TemplateKinds()
        {
            var page = 1;
            var pageSize = 2;
            var ct = CancellationToken.None;

            var templateKindsPage = new Page<TemplateKind>(2, new List<TemplateKind>
            {
                new TemplateKind() { TemplateKindKey = "tk-1" },
                new TemplateKind() { TemplateKindKey = "tk-2" },
            });

            var templateKindsCounters = new List<KeyValuePair<string, long>>()
            {
                new KeyValuePair<string, long>("tk-1", 0),
                new KeyValuePair<string, long>("tk-2", 10),
            };

            var expectedPage = new Page<TemplateKindWithCount>(2, new List<TemplateKindWithCount>()
            {
                new TemplateKindWithCount("tk-1", 0),
                new TemplateKindWithCount("tk-2", 10)
            });

            _templateKindsRepositoryMock.Setup(x => x.GetTemplateKindsPage(page, pageSize, ct))
                .ReturnsAsync(templateKindsPage)
                .Verifiable();

            var templateKindKeys = templateKindsPage.Entities.Select(x => x.TemplateKindKey);
            _templatesRepositoryMock.Setup(x => x.CountTemplatesByTemplateKindKeys(templateKindKeys, ct))
                .ReturnsAsync(templateKindsCounters)
                .Verifiable();

            var result = await _manager.GetPage(page, pageSize, ct);
            result.Should().BeEquivalentTo(expectedPage);

            _templateKindsRepositoryMock.Verify();
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