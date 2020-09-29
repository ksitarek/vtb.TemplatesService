using System;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateKindInUseException : Exception
    {
        public string TemplateKindKey { get; }

        public TemplateKindInUseException(string templateKindKey) : base($"TemplateKind with TemplateKindKey: {templateKindKey} is still in use")
        {
            TemplateKindKey = templateKindKey;
        }
    }
}