namespace vtb.TemplatesService.Contracts.Requests
{
    public class GetDefaultTemplateRequest
    {
        public string TemplateKindKey { get; }

        public GetDefaultTemplateRequest()
        {
        }

        public GetDefaultTemplateRequest(string templateKindKey)
        {
            TemplateKindKey = templateKindKey;
        }
    }
}