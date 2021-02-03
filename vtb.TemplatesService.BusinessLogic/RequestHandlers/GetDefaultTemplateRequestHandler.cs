using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.BusinessLogic.Managers;
using vtb.TemplatesService.Contracts.Requests;
using vtb.TemplatesService.Contracts.Responses;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.RequestHandlers
{
    public class GetDefaultTemplateRequestHandler : IConsumer<GetDefaultTemplateRequest>
    {
        private readonly ILogger<GetDefaultTemplateRequestHandler> _logger;
        private readonly ITemplateManager _templateManager;

        public GetDefaultTemplateRequestHandler(ILogger<GetDefaultTemplateRequestHandler> logger, ITemplateManager templateManager)
        {
            _logger = logger;
            _templateManager = templateManager;
        }

        public async Task Consume(ConsumeContext<GetDefaultTemplateRequest> context)
        {
            using (_logger.BeginScope("Retrieve default template for key {templateKindKey}",
                context.Message.TemplateKindKey))
            {
                var defaultTemplate = default(Template);

                try
                {
                    defaultTemplate = await _templateManager.GetDefaultTemplate(context.Message.TemplateKindKey,
                        context.CancellationToken);

                    _logger.LogDebug("For kind {templateKindKey} default template is {templateId} and version is {templateVersionId}",
                        context.Message.TemplateKindKey, defaultTemplate.TemplateId, defaultTemplate.ActiveVersion?.TemplateVersionId);

                    await context.RespondAsync(new DefaultTemplateResponse(
                        defaultTemplate.TemplateId,
                        defaultTemplate.ActiveVersion?.TemplateVersionId ?? Guid.Empty));
                }
                catch (TemplateKindNotFoundException e)
                {
                    _logger.LogError("Template kind with key {templateKindKey} was not found.",
                        context.Message.TemplateKindKey);

                    await context.RespondAsync(new DefaultTemplateResponse(Guid.Empty, Guid.Empty));
                }
                catch (TemplateNotFoundException e)
                {
                    _logger.LogError("Default template for kind {templateKindKey} was not found.",
                        context.Message.TemplateKindKey);

                    await context.RespondAsync(new DefaultTemplateResponse(Guid.Empty, Guid.Empty));
                }
            }
        }
    }
}