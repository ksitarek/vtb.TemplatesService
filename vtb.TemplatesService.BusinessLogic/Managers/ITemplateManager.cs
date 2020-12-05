using System;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.DataAccess;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.Managers
{
    public interface ITemplateManager
    {
        Task<Template> Get(Guid templateId, CancellationToken cancellationToken);

        Task<Page<Template>> GetPage(int page, int pageSize, CancellationToken cancellationToken);

        Task CreateTemplate(Guid newTemplateId, string templateKindKey, string label, string content, CancellationToken cancellationToken);

        Task CreateTemplateVersion(Guid newTemplateVersionId, Guid templateId, string content, bool isActive, CancellationToken cancellationToken);

        Task<TemplateVersion> GetTemplateVersion(Guid templateId, Guid templateVersionId, CancellationToken cancellationToken);

        Task<Page<TemplateVersion>> GetTemplateVersionsPage(Guid templateId, int page, int pageSize, CancellationToken cancellationToken);

        Task UpdateTemplateVersion(Guid templateId, Guid templateVersionId, string content, CancellationToken cancellationToken);

        Task RemoveTemplate(Guid templateId, CancellationToken cancellationToken);

        Task RemoveTemplateVersion(Guid templateId, Guid templateVersionId, CancellationToken cancellationToken);

        Task<Template> GetDefaultTemplate(string templateKindKey, CancellationToken cancellationToken);

        Task SetDefaultTemplate(string templateKindKey, Guid templateId, CancellationToken cancellationToken);
        Task SetCurrentVersion(Guid templateId, Guid templateVersionId, CancellationToken cancellationToken);
    }
}