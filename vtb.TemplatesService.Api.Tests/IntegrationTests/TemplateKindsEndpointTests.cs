using NUnit.Framework;
using vtb.TemplatesService.Api.Tests.IntegrationTests.Clients;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public class TemplateKindsEndpointTests : BaseApiEndpointTests
    {
        internal TemplateKindsTestsClient _client;

        [SetUp]
        public void SetUp()
        {
            _client = new TemplateKindsTestsClient(_httpClient);
            _httpClient.DefaultRequestHeaders.Clear();
        }
    }
}