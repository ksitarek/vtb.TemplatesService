using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace vtb.TemplatesService.Api.Requests
{
    public class CreateTemplateVersion
    {
        [FromBody]
        [Required]
        public string Content { get; set; }

        [FromBody]
        [Required]
        public bool IsActive { get; set; }
    }
}