using System;
using System.Runtime.Serialization;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateVersionNotFoundException : Exception
    {
        public Guid TemplateId { get; }

        public Guid TemplateVersionId { get; }

        public TemplateVersionNotFoundException(Guid templateId, Guid templateVersionId) : base($"Template version with Id {templateVersionId} was not found in template with Id {templateVersionId}")
        {
            TemplateId = templateId;
            TemplateVersionId = templateVersionId;
        }

        protected TemplateVersionNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            TemplateId = Guid.Parse(info.GetString($"{nameof(TemplateVersionNotFoundException)}.{nameof(TemplateId)}"));
            TemplateVersionId = Guid.Parse(info.GetString($"{nameof(TemplateVersionNotFoundException)}.{nameof(TemplateVersionId)}"));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue($"{nameof(TemplateVersionNotFoundException)}.{nameof(TemplateId)}", TemplateId.ToString());
            info.AddValue($"{nameof(TemplateVersionNotFoundException)}.{nameof(TemplateVersionId)}", TemplateVersionId.ToString());
        }
    }
}