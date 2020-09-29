using System;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateKindKeyAlreadyTakenException : Exception
    {
        public string TemplateKindKey { get; }

        public TemplateKindKeyAlreadyTakenException(string templateKindKey) : base($"TemplateKind with TemplateKindKey: {templateKindKey} already exists")
        {
            TemplateKindKey = templateKindKey;
        }
    }
}