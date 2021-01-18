using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.BusinessLogic.Managers;
using vtb.TemplatesService.Contracts.Requests;
using vtb.TemplatesService.Contracts.Responses;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.RequestHandlers
{
    public class GetDefaultTemplateRequestHandler : IConsumer<IGetDefaultTemplateRequest>
    {
        private readonly ILogger<GetDefaultTemplateRequestHandler> _logger;
        private readonly ITemplateManager _templateManager;

        public GetDefaultTemplateRequestHandler(ILogger<GetDefaultTemplateRequestHandler> logger, ITemplateManager templateManager)
        {
            _logger = logger;
            _templateManager = templateManager;
        }

        public async Task Consume(ConsumeContext<IGetDefaultTemplateRequest> context)
        {
            using (_logger.BeginScope("Retrieve default template for key {templateKindKey}",
                context.Message.TemplateKindKey))
            {
                var defaultTemplate = default(Template);

                try
                {
                    defaultTemplate = await _templateManager.GetDefaultTemplate(context.Message.TemplateKindKey, context.CancellationToken);
                }
                catch (TemplateKindNotFoundException e)
                {
                    _logger.LogError("Template kind with key {templateKindKey} was not found.", context.Message.TemplateKindKey);
                }

                _logger.LogDebug("For kind {templateKindKey} default template is {templateId} and version is {templateVersionId}",
                    context.Message.TemplateKindKey, defaultTemplate.TemplateId, defaultTemplate.ActiveVersion?.TemplateVersionId);

                await context.RespondAsync<IDefaultTemplateResponse>(new
                {
                    TemplateId = defaultTemplate.TemplateId,
                    TemplateVersionId = defaultTemplate.ActiveVersion?.TemplateVersionId
                });
            }
        }
    }
}