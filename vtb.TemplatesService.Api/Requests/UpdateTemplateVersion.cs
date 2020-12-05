using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace vtb.TemplatesService.Api.Requests
{
    public class UpdateTemplateVersion
    {
        [FromRoute]
        [Required]
        public Guid TemplateId { get; set; }

        [FromRoute]
        [Required]
        public Guid TemplateVersionId { get; set; }

        [FromBody]
        public UpdateTemplateVersionBody Body { get; set; }

        public class UpdateTemplateVersionBody
        {
            [Required]
            public string Content { get; set; }
        }
    }
}