using System;
using System.Net.Http;
using System.Threading.Tasks;
using vtb.TemplatesService.Api.Tests.IntegrationTests.ExpectedResults;
using vtb.Testing.Rest;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests.Clients
{
    internal class TemplatesTestsClient
    {
        private const string Endpoint = "v1/templates";
        private readonly IRestClient _restClient;

        internal TemplatesTestsClient(HttpClient httpClient)
        {
            _restClient = new RestClient(httpClient);
        }

        internal ValueTask<ExpectedListPage<ExpectedTemplateListItem>> GetTemplates(int page, int pageSize)
            => _restClient.GetContentAsync<ExpectedListPage<ExpectedTemplateListItem>>(
                $"{Endpoint}?page={page}&pageSize={pageSize}");

        internal ValueTask<ExpectedTemplateDetails> Get(Guid templateId)
            => _restClient.GetContentAsync<ExpectedTemplateDetails>($"{Endpoint}/{templateId}");

        internal ValueTask<Uri> AddNewTemplate(ExpectedCreateTemplate request)
            => _restClient.PostAsync($"{Endpoint}", request);

        internal ValueTask<Uri> AddNewTemplateVersion(Guid templateId, ExpectedCreateTemplateVersion request)
            => _restClient.PostAsync($"{Endpoint}/{templateId}/versions", request);

        internal ValueTask<ExpectedTemplateVersionDetails> GetTemplateVersionDetails(Guid templateId,
            Guid templateVersionId)
            => _restClient.GetContentAsync<ExpectedTemplateVersionDetails>(
                $"{Endpoint}/{templateId}/versions/{templateVersionId}");

        internal ValueTask<ExpectedListPage<ExpectedTemplateVersionListItem>> GetTemplateVersions(Guid templateId, int page,
            int pageSize)
            => _restClient.GetContentAsync<ExpectedListPage<ExpectedTemplateVersionListItem>>($"{Endpoint}/{templateId}/versions?page={page}&pageSize={pageSize}");

        internal ValueTask<Uri> UpdateTemplateVersion(Guid templateId, Guid templateVersionId,
            ExpectedUpdateTemplateVersionBody body)
            => _restClient.PostAsync($"{Endpoint}/{templateId}/versions/{templateVersionId}", body);

        internal ValueTask RemoveTemplate(Guid templateId)
            => _restClient.DeleteContentAsync($"{Endpoint}/{templateId}");

        internal ValueTask RemoveTemplateVersion(Guid templateId, Guid templateVersionId)
            => _restClient.DeleteContentAsync($"{Endpoint}/{templateId}/versions/{templateVersionId}");

        internal ValueTask<ExpectedTemplateDetails> GetDefaultTemplate(string templateKindKey)
            => _restClient.GetContentAsync<ExpectedTemplateDetails>($"{Endpoint}/default/{templateKindKey}");

        internal ValueTask<Uri> SetDefaultTemplate(string templateKindKey, Guid templateId)
            => _restClient.PostAsync($"{Endpoint}/default/{templateKindKey}", templateId);
    }
}