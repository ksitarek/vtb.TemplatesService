using System;
using System.Runtime.Serialization;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateRemovalFailedException : Exception
    {
        public Guid TemplateId { get; }

        public TemplateRemovalFailedException(Guid templateId, Exception e)
                    : base($"Could not remove template with Id: {templateId}", e)
        {
            TemplateId = templateId;
        }

        protected TemplateRemovalFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            TemplateId = Guid.Parse(info.GetString($"{nameof(TemplateRemovalFailedException)}.{nameof(TemplateId)}"));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue($"{nameof(TemplateRemovalFailedException)}.{nameof(TemplateId)}", TemplateId.ToString());
        }
    }
}