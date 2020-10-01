using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using vtb.Auth.Permissions;
using vtb.TemplatesService.Api.Requests;
using vtb.TemplatesService.Api.Responses;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.BusinessLogic.Managers;

namespace vtb.TemplatesService.Api.Controllers
{
    [Route("v1/templates")]
    [ApiController]
    [Authorize]
    public class TemplatesController : SafeControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITemplateManager _templateManager;

        public TemplatesController(
            ILogger<TemplatesController> logger,
            IMapper mapper,
            ITemplateManager templateManager) : base(logger)
        {
            _mapper = mapper;
            _templateManager = templateManager;

            MapExceptionsToStatusCodes();
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListPage<TemplateListItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<IActionResult> GetTemplates([FromQuery]int page, [FromQuery]int pageSize, CancellationToken cancellationToken)
        {
            return SafeInvoke(async () =>
            {
                var templates = await _templateManager.GetPage(page, pageSize, cancellationToken);
                return Ok(new ListPage<TemplateListItem>(templates.TotalCount, _mapper.Map<List<TemplateListItem>>(templates.Entities)));
            });
        }

        [HttpGet("{templateId}", Name = "TemplateDetails")]
        [ProducesResponseType(typeof(TemplateDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> Get([FromRoute]Guid templateId, CancellationToken cancellationToken)
        {
            return SafeInvoke(async () => Ok(_mapper.Map<TemplateDetails>(await _templateManager.Get(templateId, cancellationToken))));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [RequirePermission(Permissions.ManageTemplates)]
        public Task<IActionResult> AddNewTemplate([FromBody] CreateTemplate request, CancellationToken cancellationToken)
        {
            return SafeInvoke(async () =>
            {
                var templateId = Guid.NewGuid();
                await _templateManager.CreateTemplate(templateId, request.TemplateKindKey, request.TemplateLabel, request.Content, cancellationToken);
                return CreatedAtRoute("TemplateDetails", new { templateId }, null);
            });
        }

        [HttpPost("{templateId}/versions")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequirePermission(Permissions.ManageTemplates)]
        public Task<IActionResult> AddNewTemplateVersion([FromRoute] Guid templateId, CreateTemplateVersion request, CancellationToken cancellationToken)
        {
            return SafeInvoke(async () =>
            {
                var templateVersionId = Guid.NewGuid();
                await _templateManager.CreateTemplateVersion(templateVersionId, templateId, request.Content, request.IsActive, cancellationToken);
                return CreatedAtRoute("TemplateVersionDetails", new { templateId, templateVersionId }, null);
            });
        }

        [HttpGet("{templateId}/versions/{templateVersionId}", Name = "TemplateVersionDetails")]
        [ProducesResponseType(typeof(TemplateVersionDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequirePermission(Permissions.ManageTemplates)]
        public Task<IActionResult> GetTemplateVersionDetails([FromRoute]Guid templateId, [FromRoute]Guid templateVersionId, CancellationToken cancellationToken)
        {
            return SafeInvoke(async () =>
            {
                return Ok(_mapper.Map<TemplateVersionDetails>(await _templateManager.GetTemplateVersion(templateId, templateVersionId, cancellationToken)));
            });
        }

        [HttpGet("{templateId}/versions")]
        [ProducesResponseType(typeof(ListPage<TemplateVersionListItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequirePermission(Permissions.ManageTemplates)]
        public Task<IActionResult> GetTemplateVersions([FromRoute]Guid templateId, int page, int pageSize, CancellationToken cancellationToken)
        {
            return SafeInvoke(async () =>
            {
                var templateVersionsPage = await _templateManager.GetTemplateVersionsPage(templateId, page, pageSize, cancellationToken);
                return Ok(new ListPage<TemplateVersionListItem>(templateVersionsPage.TotalCount, _mapper.Map<List<TemplateVersionListItem>>(templateVersionsPage.Entities)));
            });
        }

        [HttpPut("{templateId}/versions/{templateVersionId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequirePermission(Permissions.ManageTemplates)]
        public Task<IActionResult> UpdateTemplateVersion(UpdateTemplateVersion request, CancellationToken cancellationToken)
        {
            return SafeInvoke(async () =>
            {
                await _templateManager.UpdateTemplateVersion(request.TemplateId, request.TemplateVersionId, request.Body.Content, request.Body.IsActive, cancellationToken);
                return AcceptedAtRoute("TemplateVersionDetails", new { request.TemplateId, request.TemplateVersionId }, null);
            });
        }

        [HttpDelete("{templateId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequirePermission(Permissions.ManageTemplates)]
        public Task<IActionResult> RemoveTemplate([FromRoute]Guid templateId, CancellationToken cancellationToken)
        {
            return SafeInvoke(async () =>
            {
                await _templateManager.RemoveTemplate(templateId, cancellationToken);
                return NoContent();
            });
        }

        [HttpDelete("{templateId}/versions/{templateVersionId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequirePermission(Permissions.ManageTemplates)]
        public Task<IActionResult> RemoveTemplateVersion([FromRoute]Guid templateId, [FromRoute]Guid templateVersionId, CancellationToken cancellationToken)
        {
            return SafeInvoke(async () =>
            {
                await _templateManager.RemoveTemplateVersion(templateId, templateVersionId, cancellationToken);
                return NoContent();
            });
        }

        [HttpGet("default/{templateKindKey}", Name = "DefaultTemplateDetails")]
        [ProducesResponseType(typeof(TemplateDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> GetDefaultTemplate([FromRoute]string templateKindKey, CancellationToken cancellationToken)
        {
            return SafeInvoke(async () =>
            {
                return Ok(_mapper.Map<TemplateDetails>(await _templateManager.GetDefaultTemplate(templateKindKey, cancellationToken)));
            });
        }

        [HttpPost("default/{templateKindKey}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequirePermission(Permissions.ManageTemplates)]
        public Task<IActionResult> SetDefaultTemplate([FromRoute]string templateKindKey, [FromBody]Guid templateId, CancellationToken cancellationToken)
        {
            return SafeInvoke(async () =>
            {
                await _templateManager.SetDefaultTemplate(templateKindKey, templateId, cancellationToken);
                return AcceptedAtRoute("DefaultTemplateDetails", new { templateKindKey }, null);
            }, new Dictionary<Type, Func<IActionResult>>()
            {
                { typeof(TemplateNotFoundException), BadRequest },
                { typeof(TemplateKindNotFoundException), BadRequest }
            });
        }

        private void MapExceptionsToStatusCodes()
        {
            _exceptionToResponseMap.Add(typeof(ArgumentException), BadRequest);
            _exceptionToResponseMap.Add(typeof(TemplateNotFoundException), NotFound);
            _exceptionToResponseMap.Add(typeof(TemplateVersionNotFoundException), NotFound);
            _exceptionToResponseMap.Add(typeof(TemplateLabelAlreadyTakenException), Conflict);
            _exceptionToResponseMap.Add(typeof(CannotRemoveActiveTemplateVersionException), Conflict);
        }
    }
}