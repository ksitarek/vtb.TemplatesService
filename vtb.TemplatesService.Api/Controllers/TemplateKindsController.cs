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
using vtb.TemplatesService.Api.Responses;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.BusinessLogic.Managers;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Controllers
{
    [Route("v1/template-kinds")]
    [ApiController]
    [Authorize]
    public class TemplateKindsController : SafeControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITemplateKindManager _templateKindManager;

        public TemplateKindsController(
            ILogger<TemplateKindsController> logger,
            IMapper mapper,
            ITemplateKindManager templateKindManager) : base(logger)
        {
            _mapper = mapper;
            _templateKindManager = templateKindManager;

            MapExceptionsToStatusCodes();
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListPage<TemplateKindListItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<IActionResult> GetTemplateKinds([FromQuery]int page, [FromQuery]int pageSize, CancellationToken cancellationToken)
        {
            return SafeInvoke(async () =>
            {
                var templateKinds = await _templateKindManager.GetPage(page, pageSize, cancellationToken);
                return Ok(new ListPage<TemplateKindListItem>(templateKinds.TotalCount, _mapper.Map<List<TemplateKindListItem>>(templateKinds.Entities)));
            });
        }

        [HttpGet("{templateKindKey}", Name = "Get")]
        [ProducesResponseType(typeof(TemplateKind), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> Get([FromRoute]string templateKindKey, CancellationToken cancellationToken)
        {
            return SafeInvoke(async () => Ok(await _templateKindManager.Get(templateKindKey, cancellationToken)));
        }

        [HttpPut("{templateKindKey}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [RequirePermission(Permissions.SystemManage)]
        public Task<IActionResult> CreateTemplateKind([FromRoute]string templateKindKey, CancellationToken cancellationToken)
        {
            return SafeInvoke(async () =>
            {
                await _templateKindManager.Create(new TemplateKind() { TemplateKindKey = templateKindKey }, cancellationToken);
                return CreatedAtRoute("Get", new { TemplateKindKey = templateKindKey }, null);
            });
        }

        [HttpDelete("{templateKindKey}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [RequirePermission(Permissions.SystemManage)]
        public Task<IActionResult> RemoveTemplateKind([FromRoute]string templateKindKey, CancellationToken cancellationToken)
        {
            return SafeInvoke(async () =>
            {
                await _templateKindManager.Remove(templateKindKey, cancellationToken);
                return Accepted();
            });
        }

        [HttpGet("exists/{templateKindKey}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<IActionResult> Exists([FromRoute]string templateKindKey, CancellationToken cancellationToken)
        {
            return SafeInvoke(async () => Ok(await _templateKindManager.Exists(templateKindKey, cancellationToken)));
        }

        private void MapExceptionsToStatusCodes()
        {
            _exceptionToResponseMap.Add(typeof(ArgumentException), () => BadRequest());
            _exceptionToResponseMap.Add(typeof(TemplateKindInUseException), () => Conflict());
            _exceptionToResponseMap.Add(typeof(TemplateKindNotFoundException), () => NotFound());
            _exceptionToResponseMap.Add(typeof(TemplateKindKeyAlreadyTakenException), () => Conflict());
        }
    }
}