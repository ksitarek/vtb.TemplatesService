using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using vtb.TemplatesService.Api.Responses;
using vtb.TemplatesService.DataAccess;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.Controllers
{
    public class TemplatesControllerTests_GetTemplates : TemplatesControllerTests
    {
        [Test]
        public async Task Produces_200OK()
        {
            var pageFromManager = new Page<Template>(2, new List<Template>
            {
                new Template() { TemplateId = Guid.NewGuid(), TemplateKindKey = "tk1", Label = "Tenant 1 Template Kind 1 Template 1", TenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f") },
                new Template() { TemplateId = Guid.NewGuid(), TemplateKindKey = "tk1", Label = "Tenant 1 Template Kind 1 Template 2", TenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f") },
            });

            _templateManager.Setup(x => x.GetPage(1, 2, CancellationToken.None))
                .ReturnsAsync(pageFromManager)
                .Verifiable();

            var listFromMapper = new List<TemplateListItem>()
            {
                new TemplateListItem() { TemplateId = Guid.NewGuid(), TemplateKindKey = "tk1", Label = "Tenant 1 Template Kind 1 Template 1" },
                new TemplateListItem() { TemplateId = Guid.NewGuid(), TemplateKindKey = "tk1", Label = "Tenant 1 Template Kind 1 Template 2" },
            };

            _mapper.Setup(x => x.Map<List<TemplateListItem>>(pageFromManager.Entities))
                .Returns(listFromMapper)
                .Verifiable();

            var result = await _controller.GetTemplates(1, 2, CancellationToken.None) as ObjectResult;

            var expectedPageList = new ListPage<TemplateListItem>(2, listFromMapper);
            result.Value.Should().BeEquivalentTo(expectedPageList);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            _templateManager.Verify();
            _mapper.Verify();
        }

        [Test]
        public async Task Produces_400BadRequest_For_ArgumentException()
        {
            _templateManager.Setup(x => x.GetPage(1, 2, CancellationToken.None))
                .ThrowsAsync(new ArgumentException());

            var result = await _controller.GetTemplates(1, 2, CancellationToken.None);
            result.Should().BeOfType(typeof(BadRequestResult));
        }
    }
}