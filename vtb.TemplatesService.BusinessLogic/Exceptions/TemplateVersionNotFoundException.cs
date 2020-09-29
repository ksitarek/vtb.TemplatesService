using System;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateVersionNotFoundException : Exception
    {
        public TemplateVersionNotFoundException(Guid templateId, Guid templateVersionId) : base($"Template version with Id {templateVersionId} was not found in template with Id {templateVersionId}")
        {
            TemplateId = templateId;
            TemplateVersionId = templateVersionId;
        }

        public Guid TemplateId { get; }
        public Guid TemplateVersionId { get; }
    }
}