using System;
using System.Runtime.Serialization;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    [Serializable]
    public class SetDefaultTemplateFailedException : Exception
    {
        public Guid TemplateId { get; }

        public string TemplateKindKey { get; }

        public SetDefaultTemplateFailedException(string templateKindKey, Guid templateId, Exception innerException)
                            : base($"Could not set template with ID: {templateId} as default for template kind {templateKindKey}", innerException)
        {
            TemplateKindKey = templateKindKey;
            TemplateId = templateId;
        }

        protected SetDefaultTemplateFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            TemplateId = Guid.Parse(info.GetString($"{nameof(SetDefaultTemplateFailedException)}.{nameof(TemplateId)}"));
            TemplateKindKey = info.GetString($"{nameof(SetDefaultTemplateFailedException)}.{nameof(TemplateKindKey)}");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue($"{nameof(SetDefaultTemplateFailedException)}.{nameof(TemplateId)}", TemplateId.ToString());
            info.AddValue($"{nameof(SetDefaultTemplateFailedException)}.{nameof(TemplateKindKey)}", TemplateKindKey);
        }
    }
}