using Mongo2Go;
using MongoDB.Driver;
using NUnit.Framework;

namespace vtb.TemplatesService.DataAccess.Tests
{
    [NonParallelizable]
    public abstract class MongoDbTests
    {
        private MongoDbRunner _runner;
        protected IMongoDatabase _database;

        [OneTimeSetUp]
        public void CreateConnection()
        {
            _runner = MongoDbRunner.Start(singleNodeReplSet: true, singleNodeReplSetWaitTimeout: 10);

            var mongoClient = new MongoClient(_runner.ConnectionString);
            _database = mongoClient.GetDatabase("test_db");
        }

        [OneTimeTearDown]
        public void DestroyConnection()
        {
            _runner?.Dispose();
        }
    }
}