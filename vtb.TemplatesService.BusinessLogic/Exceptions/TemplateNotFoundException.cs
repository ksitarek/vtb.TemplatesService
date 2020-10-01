using System;
using System.Runtime.Serialization;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateNotFoundException : Exception
    {
        public Guid TemplateId { get; }
        public string TemplateKindKey { get; }

        public TemplateNotFoundException(Guid templateId) : base($"Template with TemplateId: {templateId} does not exists")
        {
            TemplateId = templateId;
        }

        public TemplateNotFoundException(string templateKindKey) : base($"Default template for kind {templateKindKey} does not exists")
        {
            TemplateKindKey = templateKindKey;
        }

        protected TemplateNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            TemplateId = Guid.Parse(info.GetString($"{nameof(TemplateNotFoundException)}.{nameof(TemplateId)}"));
            TemplateKindKey = info.GetString($"{nameof(TemplateNotFoundException)}.{nameof(TemplateKindKey)}");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue($"{nameof(TemplateNotFoundException)}.{nameof(TemplateId)}", TemplateId.ToString());
            info.AddValue($"{nameof(TemplateNotFoundException)}.{nameof(TemplateKindKey)}", TemplateKindKey);
        }
    }
}