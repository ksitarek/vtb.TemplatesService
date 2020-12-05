using System;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests.ExpectedResults
{
    internal class ExpectedTemplateVersionListItem
    {
        public Guid TemplateVersionId { get; set; }
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public static ExpectedTemplateVersionListItem From(TemplateVersion version)
        {
            return new ExpectedTemplateVersionListItem()
            {
                TemplateVersionId = version.TemplateVersionId,
                Version = version.Version,
                IsActive = version.IsActive,
                CreatedAt = version.CreatedAt,
                UpdatedAt = version.UpdatedAt
            };
        }
    }
}