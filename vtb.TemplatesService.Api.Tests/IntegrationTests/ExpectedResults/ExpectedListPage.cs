using System.Collections.Generic;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests.ExpectedResults
{
    internal class ExpectedListPage<T>
    {
        private readonly List<T> _entities;

        public IReadOnlyList<T> Entities => _entities.AsReadOnly();
        public long TotalCount { get; }

        public ExpectedListPage(long totalCount, List<T> entities)
        {
            TotalCount = totalCount;
            _entities = entities;
        }
    }
}