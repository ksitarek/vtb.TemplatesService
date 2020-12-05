using MongoDB.Driver;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace vtb.TemplatesService.DataAccess.Seed
{
    public class Seeder
    {
        private readonly IMongoDatabase _database;

        public Seeder(IMongoDatabase database)
        {
            _database = database;
        }

        public Task Seed<TDoc>(Type seedClassType)
            where TDoc : class
        {
            var docType = typeof(TDoc);

            var seedData = seedClassType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.IsStatic && f.FieldType == docType);

            var collection = _database.GetCollection<TDoc>(seedClassType.Name.ToString());
            var documents = seedData.Select(x => x.GetValue(null) as TDoc);
            return collection.InsertManyAsync(documents);
        }
    }
}