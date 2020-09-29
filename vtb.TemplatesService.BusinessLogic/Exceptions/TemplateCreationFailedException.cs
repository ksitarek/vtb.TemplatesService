using System;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateCreationFailedException : Exception
    {
        public TemplateCreationFailedException(string templateKindKey, string templateLabel, Exception innerException)
            : base($"Failed to create template of kind {templateKindKey} with label '{templateLabel}'", innerException)
        {
            TemplateKindKey = templateKindKey;
            TemplateLabel = templateLabel;
        }

        public string TemplateKindKey { get; }
        public string TemplateLabel { get; }
    }
}