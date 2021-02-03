using System;
using System.Text.Json.Serialization;

namespace vtb.TemplatesService.Contracts.Responses
{
    public class DefaultTemplateResponse
    {
        public Guid TemplateId { get; }
        public Guid TemplateVersionId { get; }

        [JsonConstructor]
        public DefaultTemplateResponse(Guid templateId, Guid templateVersionId)
        {
            TemplateId = templateId;
            TemplateVersionId = templateVersionId;
        }
    }
}