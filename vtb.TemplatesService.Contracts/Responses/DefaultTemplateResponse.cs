using System;

namespace vtb.TemplatesService.Contracts.Responses
{
    public class DefaultTemplateResponse
    {
        public Guid TemplateId { get; }
        public Guid TemplateVersionId { get; }

        public DefaultTemplateResponse(Guid templateId, Guid templateVersionId)
        {
            TemplateId = templateId;
            TemplateVersionId = templateVersionId;
        }
    }
}