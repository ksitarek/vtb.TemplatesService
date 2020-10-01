using System;
using System.Runtime.Serialization;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateVersionUpdateFailedException : Exception
    {
        public Guid TemplateId { get; }

        public Guid TemplateVersionId { get; }

        public TemplateVersionUpdateFailedException(Guid templateId, Guid templateVersionId, Exception e)
                            : base($"Could not update template version with Id: {templateVersionId} of template with Id: {templateId}", e)
        {
            TemplateId = templateId;
            TemplateVersionId = templateVersionId;
        }

        protected TemplateVersionUpdateFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            TemplateId = Guid.Parse(info.GetString($"{nameof(TemplateVersionUpdateFailedException)}.{nameof(TemplateId)}"));
            TemplateVersionId = Guid.Parse(info.GetString($"{nameof(TemplateVersionUpdateFailedException)}.{nameof(TemplateVersionId)}"));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue($"{nameof(TemplateVersionUpdateFailedException)}.{nameof(TemplateId)}", TemplateId.ToString());
            info.AddValue($"{nameof(TemplateVersionUpdateFailedException)}.{nameof(TemplateVersionId)}", TemplateVersionId.ToString());
        }
    }
}