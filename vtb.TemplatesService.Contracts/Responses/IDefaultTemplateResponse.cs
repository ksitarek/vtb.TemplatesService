using System;

namespace vtb.TemplatesService.Contracts.Responses
{
    public interface IDefaultTemplateResponse
    {
        Guid TemplateId { get; }
        Guid TemplateVersionId { get; }
    }
}