using System;
using System.Runtime.Serialization;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    [Serializable]
    public class TemplateVersionRemovalFailedException : Exception
    {
        public Guid TemplateId { get; }

        public Guid TemplateVersionId { get; }

        public TemplateVersionRemovalFailedException(Guid templateId, Guid templateVersionId, Exception e)
                            : base($"Could not remove template version with Id: {templateVersionId} of template with Id: {templateId}", e)
        {
            TemplateId = templateId;
            TemplateVersionId = templateVersionId;
        }

        protected TemplateVersionRemovalFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            TemplateId = Guid.Parse(info.GetString($"{nameof(TemplateVersionRemovalFailedException)}.{nameof(TemplateId)}"));
            TemplateVersionId = Guid.Parse(info.GetString($"{nameof(TemplateVersionRemovalFailedException)}.{nameof(TemplateVersionId)}"));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue($"{nameof(TemplateVersionRemovalFailedException)}.{nameof(TemplateId)}", TemplateId.ToString());
            info.AddValue($"{nameof(TemplateVersionRemovalFailedException)}.{nameof(TemplateVersionId)}", TemplateVersionId.ToString());
        }
    }
}