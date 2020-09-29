using System.Collections.Generic;

namespace vtb.TemplatesService.DataAccess
{
    public class Page<T>
    {
        public Page(long count, IReadOnlyList<T> data)
        {
            TotalCount = count;
            Entities = data;
        }

        public long TotalCount { get; }
        public IReadOnlyList<T> Entities { get; }
    }
}