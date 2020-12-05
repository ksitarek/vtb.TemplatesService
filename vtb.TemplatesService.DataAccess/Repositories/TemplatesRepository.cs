using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using vtb.Auth.Tenant;
using vtb.TemplatesService.DataAccess.Seed;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.DataAccess.Repositories
{
    public class TemplatesRepository : ITemplatesRepository
    {
        private const string CollectionName = nameof(Templates);
        private readonly IMongoDatabase _mongoDatabase;
        private readonly ITenantIdProvider _tenantIdProvider;

        public TemplatesRepository(IMongoDatabase mongoDatabase, ITenantIdProvider tenantIdProvider)
        {
            _mongoDatabase = mongoDatabase;
            _tenantIdProvider = tenantIdProvider;
        }

        private static FilterDefinitionBuilder<Template> _filterBuilder => Builders<Template>.Filter;
        private static UpdateDefinitionBuilder<Template> _updateBuilder => Builders<Template>.Update;
        private IMongoCollection<Template> _collection => _mongoDatabase.GetCollection<Template>(CollectionName);

        public Task AddTemplate(Template template, CancellationToken cancellationToken = default)
        {
            template.TenantId = _tenantIdProvider.TenantId;
            return _collection.InsertOneAsync(template, null, cancellationToken);
        }

        public Task<long> CountTemplatesByTemplateKindKey(string templateKindKey, CancellationToken cancellationToken = default)
        {
            var filterBuilder = _filterBuilder;
            var filter = filterBuilder.Eq(x => x.TemplateKindKey, templateKindKey);

            return _collection.CountDocumentsAsync(filter, null, cancellationToken);
        }

        public async Task<List<KeyValuePair<string, long>>> CountTemplatesByTemplateKindKeys(IEnumerable<string> templateKindsKeys, CancellationToken cancellationToken)
        {
            var result = await _collection.Aggregate()
                .Match(x => templateKindsKeys.Contains(x.TemplateKindKey))
                .Group(x => x.TemplateKindKey, g => new { Key = g.Key, Value = g.LongCount() })
                .ToListAsync(cancellationToken);

            return templateKindsKeys.Select(x
                => new KeyValuePair<string, long>(
                    x,
                    result
                        .Where(z => z.Key == x)
                        .Select(z => z.Value)
                        .DefaultIfEmpty(0)
                        .FirstOrDefault()))
                .ToList();
        }

        public Task<Template> GetDefaultTemplate(string templateKindKey, CancellationToken cancellationToken = default)
        {
            // TODO improve this method to retrieve template with active version only, instead of the whole document
            var filter = TenantFilter()
                & _filterBuilder.Eq(x => x.TemplateKindKey, templateKindKey)
                & _filterBuilder.Eq(x => x.IsDefault, true);

            return _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Page<Template>> GetTemplatesPage(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var countFacetName = "count";
            var entitiesFacetName = "entities";

            var countFacet = AggregateFacet.Create(countFacetName,
                PipelineDefinition<Template, AggregateCountResult>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Count<Template>(),
                }));

            var entitiesFacet = AggregateFacet.Create(entitiesFacetName,
                PipelineDefinition<Template, Template>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Skip<Template>((page - 1) * pageSize),
                    PipelineStageDefinitionBuilder.Limit<Template>(pageSize),
                }));

            var aggregation = await _collection.Aggregate()
                .Match(TenantFilter())
                .Facet(countFacet, entitiesFacet)
                .ToListAsync(cancellationToken);

            var data = aggregation.First()
                .Facets.First(x => x.Name == entitiesFacetName)
                .Output<Template>();

            var countOutput = aggregation.First()
                .Facets.First(x => x.Name == countFacetName)
                .Output<AggregateCountResult>();

            long count = 0;
            if (countOutput.Any())
            {
                count = countOutput[0].Count;
            }

            return new Page<Template>(count, data);
        }

        public Task<Page<TemplateVersion>> GetTemplateVersionsPage(Guid templateId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var entities = _collection.AsQueryable()
                .Where(x => x.TenantId == _tenantIdProvider.TenantId && x.TemplateId == templateId)
                .SelectMany(x => x.Versions)
                .OrderByDescending(x => x.Version)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalCount = _collection.AsQueryable()
                .Where(x => x.TenantId == _tenantIdProvider.TenantId && x.TemplateId == templateId)
                .SelectMany(x => x.Versions)
                .Count();

            return Task.FromResult(new Page<TemplateVersion>(totalCount, entities));
        }

        public Task<Template> GetTemplateWithActiveVersionOnly(Guid templateId, CancellationToken cancellationToken = default)
        {
            var filterBuilder = _filterBuilder;
            var filter = TenantFilter();
            filter &= filterBuilder.Eq(x => x.TemplateId, templateId);
            filter &= filterBuilder.Eq($"{nameof(Template.Versions)}.{nameof(TemplateVersion.IsActive)}", true);

            return _collection
                .Aggregate()
                .Match(filter)
                .AppendStage(ActiveVersionOnlyPipelineStageDef())
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<Template> GetTemplateWithAllVersions(Guid templateId, CancellationToken cancellationToken = default)
        {
            var filterBuilder = _filterBuilder;
            var filter = filterBuilder.Eq(x => x.TemplateId, templateId) & TenantFilter();

            return _collection
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task RemoveTemplate(Guid templateId, CancellationToken cancellationToken = default)
        {
            var idFilter = _filterBuilder.Eq(x => x.TemplateId, templateId);
            return _collection.DeleteOneAsync(TenantFilter() & idFilter, cancellationToken);
        }

        public Task RemoveTemplateVersion(Guid templateId, Guid templateVersionId, CancellationToken cancellationToken = default)
        {
            var templateFilter = _filterBuilder.Eq(x => x.TemplateId, templateId) & TenantFilter();

            var templateVersionFilterBuilder = Builders<TemplateVersion>.Filter;
            var templateVersionFilter = templateVersionFilterBuilder.Eq(x => x.TemplateVersionId, templateVersionId)
                & templateVersionFilterBuilder.Eq(x => x.IsActive, false);

            var update = _updateBuilder.PullFilter(x => x.Versions, templateVersionFilter);

            return _collection.UpdateOneAsync(templateFilter, update, null, cancellationToken);
        }

        public async Task SetDefaultTemplate(string templateKindKey, Guid templateId, CancellationToken cancellationToken = default)
        {
            var templateFilter = TenantFilter() & _filterBuilder.Eq(x => x.TemplateId, templateId);
            var templateUpdate = _updateBuilder.Set(x => x.IsDefault, true);

            var otherTemplatesFilter = TenantFilter() & _filterBuilder.Ne(x => x.TemplateId, templateId);
            var otherTemplatesUpdate = _updateBuilder.Set(x => x.IsDefault, false);

            await Task.WhenAll(
                _collection.UpdateManyAsync(otherTemplatesFilter, otherTemplatesUpdate, null, cancellationToken),
                _collection.UpdateOneAsync(templateFilter, templateUpdate, null, cancellationToken)
            );
        }

        public async Task SetCurrentVersion(Guid templateId, Guid templateVersionId, CancellationToken ct)
        {
            var template = await GetTemplateWithAllVersions(templateId, ct);

            foreach (var templateVersion in template.Versions)
            {
                templateVersion.IsActive = templateVersion.TemplateVersionId == templateVersionId;
            }

            await UpdateTemplate(template, ct);
        }

        public Task<bool> TemplateExists(Guid templateId, CancellationToken cancellationToken = default)
        {
            var filter = _filterBuilder.Eq(x => x.TenantId, _tenantIdProvider.TenantId);
            filter &= _filterBuilder.Eq(x => x.TemplateId, templateId);
            return _collection.Find(filter).AnyAsync(cancellationToken);
        }

        public Task<bool> TemplateLabelTaken(string label, CancellationToken cancellationToken = default)
        {
            var filter = _filterBuilder.Eq(x => x.TenantId, _tenantIdProvider.TenantId);
            filter &= _filterBuilder.Eq(x => x.Label, label);
            return _collection.Find(filter).AnyAsync(cancellationToken);
        }

        public Task<bool> TemplateVersionExists(Guid templateId, Guid templateVersionId, CancellationToken cancellationToken = default)
        {
            var filter = _filterBuilder.Eq(x => x.TenantId, _tenantIdProvider.TenantId);
            filter &= _filterBuilder.Eq(x => x.TemplateId, templateId);
            filter &= _filterBuilder.ElemMatch(x => x.Versions, x => x.TemplateVersionId == templateVersionId);

            return _collection.Find(filter).AnyAsync();
        }

        public Task UpdateTemplate(Template template, CancellationToken cancellationToken = default)
        {
            template.TenantId = _tenantIdProvider.TenantId;

            var filterBuilder = _filterBuilder;
            var filter = filterBuilder.Eq(x => x.TemplateId, template.TemplateId) & TenantFilter();

            return _collection.ReplaceOneAsync(filter, template, new ReplaceOptions(), cancellationToken);
        }

        public Task UpdateTemplateVersion(Guid templateId, TemplateVersion templateVersion, CancellationToken cancellationToken = default)
        {
            var filter = TenantFilter()
                & _filterBuilder.Eq(x => x.TemplateId, templateId)
                & _filterBuilder.ElemMatch(x => x.Versions, x => x.TemplateVersionId == templateVersion.TemplateVersionId);

            var update = _updateBuilder
                .Set($"{nameof(Template.Versions)}.$.{nameof(TemplateVersion.Content)}", templateVersion.Content)
                .Set($"{nameof(Template.Versions)}.$.{nameof(TemplateVersion.UpdatedAt)}", templateVersion.UpdatedAt);

            return _collection.UpdateOneAsync(filter, update, null, cancellationToken);
        }

        private static PipelineStageDefinition<Template, Template> ActiveVersionOnlyPipelineStageDef()
        {
            return new BsonDocument()
            {
                {
                    "$addFields",
                    new BsonDocument()
                    {
                        {
                            nameof(Template.Versions),
                            new BsonDocument()
                            {
                                {
                                    "$filter",
                                    new BsonDocument()
                                    {
                                        { "input", $"${nameof(Template.Versions)}" },
                                        { "as", "v" },
                                        {
                                            "cond",
                                            new BsonDocument()
                                            {
                                                { "$eq", new BsonArray() { $"$$v.{nameof(TemplateVersion.IsActive)}", true } }
                                            }
                                        },
                                    }
                                }
                            }
                        }
                    }
                },
            };
        }

        private FilterDefinition<Template> TenantFilter() => _filterBuilder.Eq(x => x.TenantId, _tenantIdProvider.TenantId);
    }
}