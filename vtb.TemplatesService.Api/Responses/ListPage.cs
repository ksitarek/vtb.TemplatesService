using System.Collections.Generic;

namespace vtb.TemplatesService.Api.Responses
{
    public class ListPage<T>
    {
        private readonly List<T> _entities;

        public IReadOnlyList<T> Entities => _entities.AsReadOnly();
        public long TotalCount { get; }

        public ListPage(long totalCount, List<T> entities)
        {
            TotalCount = totalCount;
            _entities = entities;
        }
    }
}