using System.ComponentModel.DataAnnotations;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests.ExpectedResults
{
    internal class ExpectedUpdateTemplateVersionBody
    {
        [Required] public string Content { get; set; }

        [Required] public bool IsActive { get; set; }
    }
}