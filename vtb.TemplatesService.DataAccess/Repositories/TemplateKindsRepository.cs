using MongoDB.Driver;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using vtb.TemplatesService.DataAccess.Seed;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.DataAccess.Repositories
{
    public class TemplateKindsRepository : ITemplateKindsRepository
    {
        private readonly IMongoDatabase _mongoDatabase;

        private const string CollectionName = nameof(TemplateKinds);
        private IMongoCollection<TemplateKind> _collection => _mongoDatabase.GetCollection<TemplateKind>(CollectionName);

        public TemplateKindsRepository(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public Task AddTemplateKind(TemplateKind templateKind, CancellationToken cancellationToken = default)
            => _collection.InsertOneAsync(templateKind, null, cancellationToken);

        public Task<TemplateKind> GetTemplateKind(string templateKindKey, CancellationToken cancellationToken = default)
        {
            var filter = FilterByTemplateKindKey(templateKindKey);
            return _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public Task RemoveTemplateKind(string templateKindKey, CancellationToken cancellationToken = default)
        {
            var filter = FilterByTemplateKindKey(templateKindKey);
            return _collection.DeleteOneAsync(filter, cancellationToken);
        }

        private static FilterDefinition<TemplateKind> FilterByTemplateKindKey(string templateKindKey)
            => Builders<TemplateKind>.Filter.Eq(x => x.TemplateKindKey, templateKindKey);

        public async Task<Page<TemplateKind>> GetTemplateKindsPage(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var countFacetName = "count";
            var entitiesFacetName = "entities";

            var countFacet = AggregateFacet.Create(countFacetName,
                PipelineDefinition<TemplateKind, AggregateCountResult>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Count<TemplateKind>(),
                }));

            var sortDefinition = Builders<TemplateKind>.Sort.Ascending(x => x.TemplateKindKey);
            var entitiesFacet = AggregateFacet.Create(entitiesFacetName,
                PipelineDefinition<TemplateKind, TemplateKind>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Sort(sortDefinition),
                    PipelineStageDefinitionBuilder.Skip<TemplateKind>((page - 1) * pageSize),
                    PipelineStageDefinitionBuilder.Limit<TemplateKind>(pageSize),
                }));

            var aggregation = await _collection.Aggregate()
                .Facet(countFacet, entitiesFacet)
                .ToListAsync(cancellationToken);

            var data = aggregation.First()
                .Facets.First(x => x.Name == entitiesFacetName)
                .Output<TemplateKind>();

            var countOutput = aggregation.First()
                .Facets.First(x => x.Name == countFacetName)
                .Output<AggregateCountResult>();

            long count = 0;
            if (countOutput.Any())
            {
                count = countOutput.First()
                    .Count;
            }

            return new Page<TemplateKind>(count, data);
        }
    }
}