using System;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateVersionRemovalFailedException : Exception
    {
        public TemplateVersionRemovalFailedException(Guid templateId, Guid templateVersionId, Exception e)
            : base($"Could not remove template version with Id: {templateVersionId} of template with Id: {templateId}", e)
        {
            TemplateId = templateId;
            TemplateVersionId = templateVersionId;
        }

        public Guid TemplateId { get; }
        public Guid TemplateVersionId { get; }
    }
}