using System;

namespace vtb.TemplatesService.Api.Responses
{
    public class TemplateDetails
    {
        public Guid TemplateId { get; set; }
        public string TemplateKindKey { get; set; }
        public string Label { get; set; }

        public Guid CurrentVersionId { get; set; }
        public int CurrentVersion { get; set; }
        public DateTimeOffset CurrentVersionCreatedAt { get; set; }
        public string CurrentVersionContent { get; set; }
    }
}