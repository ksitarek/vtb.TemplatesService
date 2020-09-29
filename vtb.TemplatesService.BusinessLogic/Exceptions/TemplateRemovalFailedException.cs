using System;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateRemovalFailedException : Exception
    {
        public TemplateRemovalFailedException(Guid templateId, Exception e)
            : base($"Could not remove template with Id: {templateId}", e)
        {
            TemplateId = templateId;
        }

        public Guid TemplateId { get; }
    }
}