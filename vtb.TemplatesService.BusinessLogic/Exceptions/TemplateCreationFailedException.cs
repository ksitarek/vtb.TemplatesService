using System;
using System.Runtime.Serialization;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateCreationFailedException : Exception
    {
        public string TemplateKindKey { get; }

        public string TemplateLabel { get; }

        public TemplateCreationFailedException(string templateKindKey, string templateLabel, Exception innerException)
                            : base($"Failed to create template of kind {templateKindKey} with label '{templateLabel}'", innerException)
        {
            TemplateKindKey = templateKindKey;
            TemplateLabel = templateLabel;
        }

        protected TemplateCreationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            TemplateKindKey = info.GetString($"{nameof(TemplateCreationFailedException)}.{nameof(TemplateKindKey)}");
            TemplateLabel = info.GetString($"{nameof(TemplateCreationFailedException)}.{nameof(TemplateLabel)}");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue($"{nameof(TemplateCreationFailedException)}.{nameof(TemplateKindKey)}", TemplateKindKey);
            info.AddValue($"{nameof(TemplateCreationFailedException)}.{nameof(TemplateLabel)}", TemplateLabel);
        }
    }
}