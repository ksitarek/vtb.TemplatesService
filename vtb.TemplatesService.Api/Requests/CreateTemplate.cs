using System.ComponentModel.DataAnnotations;

namespace vtb.TemplatesService.Api.Requests
{
    public class CreateTemplate
    {
        [Required]
        public string TemplateKindKey { get; set; }

        [Required]
        public string TemplateLabel { get; set; }

        [Required]
        public string Content { get; set; }
    }
}