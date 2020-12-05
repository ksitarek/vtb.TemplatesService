using System;
using System.Runtime.Serialization;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    [Serializable]
    public class SetCurrentTemplateVersionFailedException : Exception
    {
        public Guid TemplateId { get; }

        public Guid TemplateVersionId { get; set; }

        public SetCurrentTemplateVersionFailedException(Guid templateId, Guid templateVersionId, Exception innerException)
                            : base($"Could not set TemplateVersion with ID: {templateVersionId} as current for template {templateId}", innerException)
        {
            TemplateId = templateId;
            TemplateVersionId = templateVersionId;
        }

        protected SetCurrentTemplateVersionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            TemplateId = Guid.Parse(info.GetString($"{nameof(SetCurrentTemplateVersionFailedException)}.{nameof(TemplateId)}"));
            TemplateVersionId = Guid.Parse(info.GetString($"{nameof(SetCurrentTemplateVersionFailedException)}.{nameof(TemplateVersionId)}"));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue($"{nameof(SetCurrentTemplateVersionFailedException)}.{nameof(TemplateId)}", TemplateId.ToString());
            info.AddValue($"{nameof(SetCurrentTemplateVersionFailedException)}.{nameof(TemplateVersionId)}", TemplateVersionId.ToString());
        }
    }
}