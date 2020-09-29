using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DataAccess;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public class TemplateManager_GetTemplateVersionsPage : TemplateManagerTests
    {
        [Test]
        public async Task Will_Return_Page_Of_TemplateVersion()
        {
            var page = 1;
            var pageSize = 2;
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var templateVersionsPage = new Page<TemplateVersion>(2, new List<TemplateVersion>()
            {
                new TemplateVersion()
                {
                    TemplateVersionId = new Guid("3c4fa624-b7b2-4c89-80f0-d7277332b3a2"), Content = "Lorem Ipsum",
                    IsActive = true, Version = 1, CreatedAt = new DateTimeOffset()
                },
                new TemplateVersion()
                {
                    TemplateVersionId = new Guid("d013797f-e190-4d28-b9dc-f49905bc3fe4"), Content = "Lorem Ipsum",
                    IsActive = false, Version = 2, CreatedAt = new DateTimeOffset()
                }
            });

            _templatesRepositoryMock.Setup(x => x.TemplateExists(templateId, ct))
                .ReturnsAsync(true)
                .Verifiable();

            _templatesRepositoryMock.Setup(x => x.GetTemplateVersionsPage(templateId, page, pageSize, ct))
                .ReturnsAsync(templateVersionsPage)
                .Verifiable();

            var result = await _manager.GetTemplateVersionsPage(templateId, page, pageSize, ct);
            result.Should().BeEquivalentTo(templateVersionsPage);

            _templatesRepositoryMock.Verify();
        }

        [TestCase("00000000-0000-0000-0000-000000000000", 1, 1)]
        [TestCase("b0dfe5bf-c621-4de3-a657-b991b9384b0f", 0, 1)]
        [TestCase("b0dfe5bf-c621-4de3-a657-b991b9384b0f", 1, 0)]
        public void Will_Throw_ArgumentException_For_Invalid_Input(string templateIdString, int page, int pageSize)
        {
            var templateId = Guid.Parse(templateIdString);
            var ct = CancellationToken.None;

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _manager.GetTemplateVersionsPage(templateId, page, pageSize, ct));
        }

        [Test]
        public async Task Will_Throw_TemplateNotFoundException_When_Template_Not_Found()
        {
            var page = 1;
            var pageSize = 2;
            var templateId = Guid.NewGuid();
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.TemplateExists(templateId, ct))
                .ReturnsAsync(false);

            Assert.ThrowsAsync<TemplateNotFoundException>(async () =>
                await _manager.GetTemplateVersionsPage(templateId, page, pageSize, ct));
        }
    }
}