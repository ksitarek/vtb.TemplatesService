﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using vtb.TemplatesService.Api.Tests.IntegrationTests.ExpectedResults;
using vtb.Testing.Rest;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests.Clients
{
    internal class TemplateKindsTestsClient
    {
        private const string Endpoint = "v1/template-kinds";
        private readonly IRestClient _restClient;

        internal TemplateKindsTestsClient(HttpClient httpClient)
        {
            _restClient = new RestClient(httpClient);
        }

        internal ValueTask<ExpectedListPage<ExpectedTemplateKindListItem>> GetTemplateKinds(int page = 1, int pageSize = 10)
            => _restClient.GetContentAsync<ExpectedListPage<ExpectedTemplateKindListItem>>($"{Endpoint}?page={page}&pageSize={pageSize}");

        internal ValueTask<ExpectedTemplateKind> GetTemplateKind(string templateKindKey)
            => _restClient.GetContentAsync<ExpectedTemplateKind>($"{Endpoint}/{templateKindKey}");

        internal ValueTask<Uri> CreateTemplateKind(string templateKindKey)
            => _restClient.PutAsync($"{Endpoint}/{templateKindKey}");

        internal ValueTask RemoveTemplateKind(string templateKindKey)
            => _restClient.DeleteContentAsync($"{Endpoint}/{templateKindKey}");

        internal ValueTask<bool> Exists(string templateKindKey)
            => _restClient.GetContentAsync<bool>($"{Endpoint}/exists/{templateKindKey}");
    }
}