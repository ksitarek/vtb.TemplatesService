using System.Text.Json.Serialization;

namespace vtb.TemplatesService.Contracts.Requests
{
    public class GetDefaultTemplateRequest
    {
        public string TemplateKindKey { get; }

        [JsonConstructor]
        public GetDefaultTemplateRequest(string templateKindKey)
        {
            TemplateKindKey = templateKindKey;
        }
    }
}