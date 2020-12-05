using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.Api.Responses;
using vtb.TemplatesService.DataAccess;
using vtb.TemplatesService.DataAccess.DTOs;

namespace vtb.TemplatesService.Api.Tests.Controllers
{
    public class TemplateKindsControllerTests_GetTemplateKinds : TemplateKindsControllerTests
    {
        [Test]
        public async Task Produces_200OK()
        {
            var pageFromManager = new Page<TemplateKindWithCount>(2, new List<TemplateKindWithCount>
            {
                new TemplateKindWithCount("tk1", 1),
                new TemplateKindWithCount("tk2", 4),
            }.AsReadOnly());

            _templateKindManager.Setup(x => x.GetPage(1, 2, CancellationToken.None))
                .ReturnsAsync(pageFromManager);

            var listFromMapper = new List<TemplateKindListItem>
            {
                    new TemplateKindListItem {  TemplateKindKey = "tk1", Uses = 1 },
                    new TemplateKindListItem {  TemplateKindKey = "tk2", Uses = 4 },
                };

            _mapper.Setup(x => x.Map<List<TemplateKindListItem>>(pageFromManager.Entities))
                .Returns(listFromMapper);

            var result = await _controller.GetTemplateKinds(1, 2, CancellationToken.None) as ObjectResult;

            var expectedPageList = new ListPage<TemplateKindListItem>(2, listFromMapper);
            result.Value.Should().BeEquivalentTo(expectedPageList);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            _templateKindManager.Verify(x => x.GetPage(1, 2, CancellationToken.None), Times.Once);
            _mapper.Verify(x => x.Map<List<TemplateKindListItem>>(It.IsAny<IReadOnlyList<TemplateKindWithCount>>()), Times.Once);
        }

        [Test]
        public async Task Produces_400BadRequest_For_ArgumentException()
        {
            _templateKindManager.Setup(x => x.GetPage(1, 2, CancellationToken.None))
                .Throws(new ArgumentException());

            var result = await _controller.GetTemplateKinds(1, 2, CancellationToken.None);
            result.Should().BeOfType(typeof(BadRequestResult));
        }
    }
}