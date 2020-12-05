using System;

namespace vtb.TemplatesService.Api.Responses
{
    public class TemplateVersionListItem
    {
        public Guid TemplateVersionId { get; set; }
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}