using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.DataAccess.Repositories
{
    public interface ITemplateKindsRepository
    {
        Task AddTemplateKind(TemplateKind templateKind, CancellationToken cancellationToken = default);

        Task<Page<TemplateKind>> GetTemplateKindsPage(int page, int pageSize, CancellationToken cancellationToken = default);

        Task<TemplateKind> GetTemplateKind(string templateKindKey, CancellationToken cancellationToken = default);

        Task RemoveTemplateKind(string templateKindKey, CancellationToken cancellationToken = default);
    }
}