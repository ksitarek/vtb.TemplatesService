using System;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests.ExpectedResults
{
    internal class ExpectedTemplateListItem
    {
        public Guid TemplateId { get; set; }
        public string TemplateKindKey { get; set; }
        public string Label { get; set; }
        public int CurrentVersion { get; set; }
        public DateTimeOffset CurrentVersionCreatedAt { get; set; }

        public static ExpectedTemplateListItem From(Template template)
        {
            return new ExpectedTemplateListItem()
            {
                TemplateId = template.TemplateId,
                TemplateKindKey = template.TemplateKindKey,
                Label = template.Label,
                CurrentVersion = template.ActiveVersion.Version,
                CurrentVersionCreatedAt = template.ActiveVersion.CreatedAt
            };
        }
    }
}