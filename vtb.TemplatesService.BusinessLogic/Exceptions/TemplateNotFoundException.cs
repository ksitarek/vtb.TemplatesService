using System;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateNotFoundException : Exception
    {
        public string TemplateKindKey { get; }
        public Guid TemplateId { get; }

        public TemplateNotFoundException(Guid templateId) : base($"Template with TemplateId: {templateId} does not exists")
        {
            TemplateId = templateId;
        }
        public TemplateNotFoundException(string templateKindKey) : base($"Default template for kind {templateKindKey} does not exists")
        {
            TemplateKindKey = templateKindKey;
        }
    }
}