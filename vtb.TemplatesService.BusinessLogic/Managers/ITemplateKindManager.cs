using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.DataAccess;
using vtb.TemplatesService.DataAccess.DTOs;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.Managers
{
    public interface ITemplateKindManager
    {
        Task Create(TemplateKind templateKind, CancellationToken cancellationToken);

        Task Remove(string templateKindKey, CancellationToken cancellationToken);

        Task<Page<TemplateKindWithCount>> GetPage(int page, int pageSize, CancellationToken cancellationToken);

        Task<TemplateKind> Get(string templateKindKey, CancellationToken cancellationToken);

        Task<bool> Exists(string templateKindKey, CancellationToken cancellationToken);
    }
}