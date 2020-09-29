using System;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateVersionUpdateFailedException : Exception
    {
        public TemplateVersionUpdateFailedException(Guid templateId, Guid templateVersionId, Exception e)
            : base($"Could not update template version with Id: {templateVersionId} of template with Id: {templateId}", e)
        {
            TemplateId = templateId;
            TemplateVersionId = templateVersionId;
        }

        public Guid TemplateId { get; }
        public Guid TemplateVersionId { get; }
    }
}