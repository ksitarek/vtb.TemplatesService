using System;
using System.Runtime.Serialization;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class CannotRemoveActiveTemplateVersionException : Exception
    {
        public Guid TemplateId { get; }

        public Guid TemplateVersionId { get; }

        public CannotRemoveActiveTemplateVersionException(Guid templateId, Guid templateVersionId)
                            : base($"Could not remove templave version with Id: {templateVersionId} of tehmplate with Id: {templateId}. The version is active.")
        {
            TemplateId = templateId;
            TemplateVersionId = templateVersionId;
        }

        protected CannotRemoveActiveTemplateVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            TemplateId = Guid.Parse(info.GetString($"{nameof(CannotRemoveActiveTemplateVersionException)}.{nameof(TemplateId)}"));
            TemplateVersionId = Guid.Parse(info.GetString($"{nameof(CannotRemoveActiveTemplateVersionException)}.{nameof(TemplateVersionId)}"));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue($"{nameof(CannotRemoveActiveTemplateVersionException)}.{nameof(TemplateId)}", TemplateId.ToString());
            info.AddValue($"{nameof(CannotRemoveActiveTemplateVersionException)}.{nameof(TemplateVersionId)}", TemplateVersionId.ToString());
        }
    }
}