using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DataAccess;
using vtb.TemplatesService.DataAccess.Repositories;
using vtb.TemplatesService.DomainModel;
using vtb.Utils;

namespace vtb.TemplatesService.BusinessLogic.Managers
{
    public class TemplateKindManager : ITemplateKindManager
    {
        private readonly ITemplateKindsRepository _templateKindsRepository;
        private readonly ITemplatesRepository _templatesRepository;

        public TemplateKindManager(
            ITemplateKindsRepository templateKindsRepository,
            ITemplatesRepository templatesRepository)
        {
            _templateKindsRepository = templateKindsRepository;
            _templatesRepository = templatesRepository;
        }

        public async Task Create(TemplateKind templateKind, CancellationToken cancellationToken)
        {
            Check.NotEmpty(templateKind.TemplateKindKey, nameof(templateKind.TemplateKindKey));
            await ValidateUniqueness(templateKind.TemplateKindKey, cancellationToken);
            await _templateKindsRepository.AddTemplateKind(templateKind, cancellationToken);
        }

        public async Task<TemplateKind> Get(string templateKindKey, CancellationToken cancellationToken)
        {
            Check.NotEmpty(templateKindKey, nameof(templateKindKey));
            var templateKind = await _templateKindsRepository.GetTemplateKind(templateKindKey, cancellationToken);

            if (templateKind == null)
            {
                throw new TemplateKindNotFoundException(templateKindKey);
            }

            return templateKind;
        }

        public async Task<bool> Exists(string templateKindKey, CancellationToken cancellationToken)
        {
            Check.NotEmpty(templateKindKey, nameof(templateKindKey));
            return await _templateKindsRepository.GetTemplateKind(templateKindKey, cancellationToken) != null;
        }

        public Task<Page<TemplateKind>> GetPage(int page, int pageSize, CancellationToken cancellationToken)
        {
            Check.GreaterThan(page, 0, nameof(page));
            Check.GreaterThan(pageSize, 0, nameof(pageSize));

            return _templateKindsRepository.GetTemplateKindsPage(page, pageSize, cancellationToken);
        }

        public async Task Remove(string templateKindKey, CancellationToken cancellationToken)
        {
            Check.NotEmpty(templateKindKey, nameof(templateKindKey));

            var templatesCnt = await _templatesRepository.CountTemplatesByTemplateKindKey(templateKindKey, cancellationToken);
            if (templatesCnt > 0)
            {
                throw new TemplateKindInUseException(templateKindKey);
            }

            if (!await Exists(templateKindKey, cancellationToken))
            {
                throw new TemplateKindNotFoundException(templateKindKey);
            }

            await _templateKindsRepository.RemoveTemplateKind(templateKindKey, cancellationToken);
        }

        private async Task ValidateUniqueness(string templateKindKey, CancellationToken cancellationToken)
        {
            if (await Exists(templateKindKey, cancellationToken))
            {
                throw new TemplateKindKeyAlreadyTakenException(templateKindKey);
            }
        }
    }
}