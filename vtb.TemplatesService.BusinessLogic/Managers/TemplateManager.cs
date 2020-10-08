using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DataAccess;
using vtb.TemplatesService.DataAccess.Repositories;
using vtb.TemplatesService.DomainModel;
using vtb.Utils;

namespace vtb.TemplatesService.BusinessLogic.Managers
{
    public class TemplateManager : ITemplateManager
    {
        private readonly ITemplatesRepository _templatesRepository;
        private readonly ITemplateKindsRepository _templateKindsRepository;
        private readonly ISystemClock _systemClock;

        public TemplateManager(
            ITemplatesRepository templatesRepository,
            ITemplateKindsRepository templateKindsRepository,
            ISystemClock systemClock)
        {
            _templatesRepository = templatesRepository;
            _templateKindsRepository = templateKindsRepository;
            _systemClock = systemClock;
        }

        public async Task CreateTemplate(Guid newTemplateId, string templateKindKey, string label, string content, CancellationToken cancellationToken)
        {
            Check.GuidNotEmpty(newTemplateId, nameof(newTemplateId));
            Check.NotEmpty(templateKindKey, nameof(templateKindKey));
            Check.NotEmpty(label, nameof(label));
            Check.NotEmpty(content, nameof(content));

            if (await _templatesRepository.TemplateLabelTaken(label))
            {
                throw new TemplateLabelAlreadyTakenException(label);
            }

            var template = new Template()
            {
                TemplateId = newTemplateId,
                TemplateKindKey = templateKindKey,
                Label = label,
                Versions = new List<TemplateVersion>()
                {
                    BuildNewTemplateVersion(Guid.NewGuid(), content, true)
                },
            };

            try
            {
                await _templatesRepository.AddTemplate(template, cancellationToken);
            }
            catch (Exception e)
            {
                throw new TemplateCreationFailedException(templateKindKey, label, e);
            }
        }

        public async Task CreateTemplateVersion(Guid newTemplateVersionId, Guid templateId, string content, bool isActive, CancellationToken cancellationToken)
        {
            Check.GuidNotEmpty(newTemplateVersionId, nameof(newTemplateVersionId));
            Check.GuidNotEmpty(templateId, nameof(templateId));
            Check.NotEmpty(content, nameof(content));

            var template = await _templatesRepository.GetTemplateWithAllVersions(templateId, cancellationToken);
            if (template == null)
            {
                throw new TemplateNotFoundException(templateId);
            }

            var templateVersion = BuildNewTemplateVersion(newTemplateVersionId, content, isActive);

            if (isActive)
            {
                foreach (var version in template.Versions)
                {
                    version.IsActive = false;
                }
            }

            templateVersion.Version = template.Versions.Count + 1;
            template.Versions.Add(templateVersion);

            try
            {
                await _templatesRepository.UpdateTemplate(template, cancellationToken);
            }
            catch (Exception e)
            {
                throw new TemplateVersionCreationFailedException(templateId, e);
            }
        }

        public async Task<Template> Get(Guid templateId, CancellationToken cancellationToken)
        {
            Check.GuidNotEmpty(templateId, nameof(templateId));
            var template = await _templatesRepository.GetTemplateWithActiveVersionOnly(templateId, cancellationToken);

            if (template == null)
            {
                throw new TemplateNotFoundException(templateId);
            }

            return template;
        }

        public async Task<Template> GetDefaultTemplate(string templateKindKey, CancellationToken cancellationToken)
        {
            Check.NotEmpty(templateKindKey, nameof(templateKindKey));

            var result = await _templatesRepository.GetDefaultTemplate(templateKindKey, cancellationToken);
            if (result == null)
            {
                throw new TemplateNotFoundException(templateKindKey);
            }

            return result;
        }

        public Task<Page<Template>> GetPage(int page, int pageSize, CancellationToken cancellationToken)
        {
            Check.GreaterThan(page, 0, nameof(page));
            Check.GreaterThan(pageSize, 0, nameof(pageSize));

            return _templatesRepository.GetTemplatesPage(page, pageSize, cancellationToken);
        }

        public async Task<TemplateVersion> GetTemplateVersion(Guid templateId, Guid templateVersionId, CancellationToken cancellationToken)
        {
            Check.GuidNotEmpty(templateId, nameof(templateId));
            Check.GuidNotEmpty(templateVersionId, nameof(templateVersionId));

            var template = await _templatesRepository.GetTemplateWithAllVersions(templateId, cancellationToken);
            if (template == null)
            {
                throw new TemplateNotFoundException(templateId);
            }

            var version = template.Versions.SingleOrDefault(x => x.TemplateVersionId == templateVersionId);
            if (version == null)
            {
                throw new TemplateVersionNotFoundException(templateId, templateVersionId);
            }

            return version;
        }

        public async Task<Page<TemplateVersion>> GetTemplateVersionsPage(Guid templateId, int page, int pageSize, CancellationToken cancellationToken)
        {
            Check.GuidNotEmpty(templateId, nameof(templateId));
            Check.GreaterThan(page, 0, nameof(page));
            Check.GreaterThan(pageSize, 0, nameof(pageSize));

            if (!await _templatesRepository.TemplateExists(templateId, cancellationToken))
            {
                throw new TemplateNotFoundException(templateId);
            }

            return await _templatesRepository.GetTemplateVersionsPage(templateId, page, pageSize, cancellationToken);
        }

        public async Task RemoveTemplate(Guid templateId, CancellationToken cancellationToken)
        {
            Check.GuidNotEmpty(templateId, nameof(templateId));

            if (!await _templatesRepository.TemplateExists(templateId))
            {
                throw new TemplateNotFoundException(templateId);
            }

            try
            {
                await _templatesRepository.RemoveTemplate(templateId, cancellationToken);
            }
            catch (Exception e)
            {
                throw new TemplateRemovalFailedException(templateId, e);
            }
        }

        public async Task RemoveTemplateVersion(Guid templateId, Guid templateVersionId, CancellationToken cancellationToken)
        {
            Check.GuidNotEmpty(templateId, nameof(templateId));
            Check.GuidNotEmpty(templateVersionId, nameof(templateVersionId));

            var template = await _templatesRepository.GetTemplateWithActiveVersionOnly(templateId, cancellationToken);
            if (template == null)
            {
                throw new TemplateNotFoundException(templateId);
            }

            if (template.ActiveVersion.TemplateVersionId == templateVersionId)
            {
                throw new CannotRemoveActiveTemplateVersionException(templateId, templateVersionId);
            }

            if (!await _templatesRepository.TemplateVersionExists(templateId, templateVersionId, cancellationToken))
            {
                throw new TemplateVersionNotFoundException(templateId, templateVersionId);
            }

            try
            {
                await _templatesRepository.RemoveTemplateVersion(templateId, templateVersionId, cancellationToken);
            }
            catch (Exception e)
            {
                throw new TemplateVersionRemovalFailedException(templateId, templateVersionId, e);
            }
        }

        public async Task SetDefaultTemplate(string templateKindKey, Guid templateId, CancellationToken cancellationToken)
        {
            Check.NotEmpty(templateKindKey, nameof(templateKindKey));
            Check.GuidNotEmpty(templateId, nameof(templateId));

            if (await _templateKindsRepository.GetTemplateKind(templateKindKey, cancellationToken) == null)
            {
                throw new TemplateKindNotFoundException(templateKindKey);
            }

            if (!await _templatesRepository.TemplateExists(templateId, cancellationToken))
            {
                throw new TemplateNotFoundException(templateId);
            }

            try
            {
                await _templatesRepository.SetDefaultTemplate(templateKindKey, templateId, cancellationToken);
            }
            catch (Exception e)
            {
                throw new SetDefaultTemplateFailedException(templateKindKey, templateId, e);
            }
        }

        public async Task UpdateTemplateVersion(Guid templateId, Guid templateVersionId, string content, bool isActive, CancellationToken cancellationToken)
        {
            Check.GuidNotEmpty(templateId, nameof(templateId));
            Check.GuidNotEmpty(templateVersionId, nameof(templateVersionId));
            Check.NotEmpty(content, nameof(content));

            if (!await _templatesRepository.TemplateExists(templateId))
            {
                throw new TemplateNotFoundException(templateId);
            }

            if (!await _templatesRepository.TemplateVersionExists(templateId, templateVersionId, cancellationToken))
            {
                throw new TemplateVersionNotFoundException(templateId, templateVersionId);
            }

            try
            {
                var templateVersionToUpdate = new TemplateVersion() { TemplateVersionId = templateVersionId, Content = content, IsActive = isActive };
                await _templatesRepository.UpdateTemplateVersion(templateId, templateVersionToUpdate, cancellationToken);
            }
            catch (Exception e)
            {
                throw new TemplateVersionUpdateFailedException(templateId, templateVersionId, e);
            }
        }

        private TemplateVersion BuildNewTemplateVersion(Guid templateVersionId, string content, bool isActive)
        {
            return new TemplateVersion()
            {
                TemplateVersionId = templateVersionId,
                CreatedAt = _systemClock.UtcNow,
                Version = 1,
                Content = content,
                IsActive = isActive,
            };
        }
    }
}