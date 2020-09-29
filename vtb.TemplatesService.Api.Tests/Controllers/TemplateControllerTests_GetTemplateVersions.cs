using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.Api.Responses;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DataAccess;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.Controllers
{
    public class TemplateControllerTests_GetTemplateVersions : TemplatesControllerTests
    {
        [Test]
        public async Task Produces_200Ok()
        {
            var templateId = Guid.NewGuid();
            var page = 1;
            var pageSize = 1;
            var ct = CancellationToken.None;

            var templateVersions = Builder<TemplateVersion>.CreateListOfSize(2).Build() as List<TemplateVersion>;
            var templateVersionListItems = Builder<TemplateVersionListItem>.CreateListOfSize(2).Build() as List<TemplateVersionListItem>;
            var templateVersionsPage = new Page<TemplateVersion>(2, templateVersions);
            var templateVersionListItemsPage = new ListPage<TemplateVersionListItem>(2, templateVersionListItems);

            _templateManager.Setup(x => x.GetTemplateVersionsPage(templateId, page, pageSize, ct))
                .ReturnsAsync(templateVersionsPage)
                .Verifiable();

            _mapper.Setup(x => x.Map<List<TemplateVersionListItem>>(templateVersions))
                .Returns(templateVersionListItems)
                .Verifiable();

            var result = await _controller.GetTemplateVersions(templateId, page, pageSize, ct) as OkObjectResult;
            result.Value.Should().BeEquivalentTo(templateVersionListItemsPage);

            _templateManager.Verify();
            _mapper.Verify();
        }

        [Test]
        public async Task Produces_400BadRequest_On_ArgumentException()
        {
            var templateId = Guid.NewGuid();
            var page = 1;
            var pageSize = 1;
            var ct = CancellationToken.None;

            var templateVersions = Builder<TemplateVersion>.CreateListOfSize(2).Build() as List<TemplateVersion>;
            var templateVersionListItems = Builder<TemplateVersionListItem>.CreateListOfSize(2).Build() as List<TemplateVersionListItem>;
            var templateVersionsPage = new Page<TemplateVersion>(2, templateVersions);
            var templateVersionListItemsPage = new ListPage<TemplateVersionListItem>(2, templateVersionListItems);

            _templateManager.Setup(x => x.GetTemplateVersionsPage(templateId, page, pageSize, ct))
                .Throws(new ArgumentException());

            var result = await _controller.GetTemplateVersions(templateId, page, pageSize, ct);
            result.Should().BeOfType(typeof(BadRequestResult));
        }

        [Test]
        public async Task Produces_404NotFound_On_TemplateNotFoundException()
        {
            var templateId = Guid.NewGuid();
            var page = 1;
            var pageSize = 1;
            var ct = CancellationToken.None;

            var templateVersions = Builder<TemplateVersion>.CreateListOfSize(2).Build() as List<TemplateVersion>;
            var templateVersionListItems = Builder<TemplateVersionListItem>.CreateListOfSize(2).Build() as List<TemplateVersionListItem>;
            var templateVersionsPage = new Page<TemplateVersion>(2, templateVersions);
            var templateVersionListItemsPage = new ListPage<TemplateVersionListItem>(2, templateVersionListItems);

            _templateManager.Setup(x => x.GetTemplateVersionsPage(templateId, page, pageSize, ct))
                .Throws(new TemplateNotFoundException(templateId));

            var result = await _controller.GetTemplateVersions(templateId, page, pageSize, ct);
            result.Should().BeOfType(typeof(NotFoundResult));
        }
    }
}