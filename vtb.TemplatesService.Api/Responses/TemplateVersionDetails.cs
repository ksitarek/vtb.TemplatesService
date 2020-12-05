using System;

namespace vtb.TemplatesService.Api.Responses
{
    public class TemplateVersionDetails
    {
        public Guid TemplateVersionId { get; set; }

        public int Version { get; set; }
        public string Content { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}