using System;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests.ExpectedResults
{
    internal class ExpectedTemplateDetails
    {
        public Guid TemplateId { get; set; }
        public string TemplateKindKey { get; set; }
        public string Label { get; set; }

        public Guid CurrentVersionId { get; set; }
        public int CurrentVersion { get; set; }
        public DateTimeOffset CurrentVersionCreatedAt { get; set; }
        public string CurrentVersionContent { get; set; }

        public static ExpectedTemplateDetails From(Template template)
        {
            return new ExpectedTemplateDetails()
            {
                TemplateId = template.TemplateId,
                TemplateKindKey = template.TemplateKindKey,
                Label = template.Label,
                CurrentVersionId = template.ActiveVersion.TemplateVersionId,
                CurrentVersion = template.ActiveVersion.Version,
                CurrentVersionCreatedAt = template.ActiveVersion.CreatedAt,
                CurrentVersionContent = template.ActiveVersion.Content
            };
        }
    }
}