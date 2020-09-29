using System;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class SetDefaultTemplateFailedException : Exception
    {
        public SetDefaultTemplateFailedException(string templateKindKey, Guid templateId, Exception innerException)
            : base($"Could not set template with ID: {templateId} as default for template kind {templateKindKey}", innerException)
        {
            TemplateKindKey = templateKindKey;
            TemplateId = templateId;
        }

        public string TemplateKindKey { get; }
        public Guid TemplateId { get; }
    }
}