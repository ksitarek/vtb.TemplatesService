using System;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests.ExpectedResults
{
    internal class ExpectedTemplateVersionDetails
    {
        public Guid TemplateVersionId { get; set; }
        public int Version { get; set; }
        public string Content { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        internal static ExpectedTemplateVersionDetails From(TemplateVersion templateVersion)
        {
            return new ExpectedTemplateVersionDetails()
            {
                TemplateVersionId = templateVersion.TemplateVersionId,
                Content = templateVersion.Content,
                CreatedAt = templateVersion.CreatedAt,
                UpdatedAt = templateVersion.UpdatedAt,
                IsActive = templateVersion.IsActive,
                Version = templateVersion.Version
            };
        }
    }
}