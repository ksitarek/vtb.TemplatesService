using System;
using System.Runtime.Serialization;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateVersionCreationFailedException : Exception
    {
        public Guid TemplateId { get; }

        public TemplateVersionCreationFailedException(Guid templateId, Exception innerException) : base($"Failed to create new version for template with Id: {templateId}", innerException)
        {
            TemplateId = templateId;
        }

        protected TemplateVersionCreationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            TemplateId = Guid.Parse(info.GetString($"{nameof(TemplateVersionCreationFailedException)}.{nameof(TemplateId)}"));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue($"{nameof(TemplateVersionCreationFailedException)}.{nameof(TemplateId)}", TemplateId.ToString());
        }
    }
}