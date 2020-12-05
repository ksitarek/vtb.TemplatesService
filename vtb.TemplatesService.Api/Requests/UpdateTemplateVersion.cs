using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace vtb.TemplatesService.Api.Requests
{
    public class UpdateTemplateVersion
    {
        [Required]
        public string Content { get; set; }
    }
}