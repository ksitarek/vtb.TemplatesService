using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.DataAccess.Repositories
{
    public interface ITemplatesRepository
    {
        Task AddTemplate(Template template, CancellationToken cancellationToken = default);

        Task<long> CountTemplatesByTemplateKindKey(string templateKindKey, CancellationToken cancellationToken = default);
        
        Task<List<KeyValuePair<string, long>>> CountTemplatesByTemplateKindKeys(IEnumerable<string> templateKindsKeys, CancellationToken cancellationToken);

        Task<Template> GetDefaultTemplate(string templateKindKey, CancellationToken cancellationToken = default);

        Task<Page<Template>> GetTemplatesPage(int page, int pageSize, CancellationToken cancellationToken = default);

        Task<Page<TemplateVersion>> GetTemplateVersionsPage(Guid templateId, int page, int pageSize, CancellationToken cancellationToken = default);

        Task<Template> GetTemplateWithActiveVersionOnly(Guid templateId, CancellationToken cancellationToken = default);

        Task<Template> GetTemplateWithAllVersions(Guid templateId, CancellationToken cancellationToken = default);

        Task RemoveTemplate(Guid templateId, CancellationToken cancellationToken = default);

        Task RemoveTemplateVersion(Guid templateId, Guid templateVersionId, CancellationToken cancellationToken = default);

        Task SetDefaultTemplate(string templateKindKey, Guid templateId, CancellationToken cancellationToken = default);

        Task<bool> TemplateExists(Guid templateId, CancellationToken cancellationToken = default);

        Task<bool> TemplateLabelTaken(string label, CancellationToken cancellationToken = default);

        Task<bool> TemplateVersionExists(Guid templateId, Guid templateVersionId, CancellationToken cancellationToken = default);

        Task UpdateTemplate(Template template, CancellationToken cancellationToken = default);

        Task UpdateTemplateVersion(Guid templateId, TemplateVersion templateVersion, CancellationToken cancellationToken = default);

    }
}