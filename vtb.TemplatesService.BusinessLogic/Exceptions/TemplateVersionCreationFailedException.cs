using System;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateVersionCreationFailedException : Exception
    {
        public TemplateVersionCreationFailedException(Guid templateId, Exception innerException) : base($"Failed to create new version for template with Id: {templateId}", innerException)
        {
            TemplateId = templateId;
        }

        public Guid TemplateId { get; }
    }
}