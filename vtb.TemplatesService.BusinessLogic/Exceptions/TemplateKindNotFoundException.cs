using System;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateKindNotFoundException : Exception
    {
        public string TemplateKindKey { get; }

        public TemplateKindNotFoundException(string templateKindKey) : base($"TemplateKind with TemplateKindKey: {templateKindKey} does not exists")
        {
            TemplateKindKey = templateKindKey;
        }
    }
}