using System;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class CannotRemoveActiveTemplateVersionException : Exception
    {
        public CannotRemoveActiveTemplateVersionException(Guid templateId, Guid templateVersionId)
            : base($"Could not remove templave version with Id: {templateVersionId} of tehmplate with Id: {templateId}. The version is active.")
        {
            TemplateId = templateId;
            TemplateVersionId = templateVersionId;
        }

        public Guid TemplateId { get; }
        public Guid TemplateVersionId { get; }
    }
}