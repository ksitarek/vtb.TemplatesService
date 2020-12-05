namespace vtb.TemplatesService.DataAccess.DTOs
{
    public class TemplateKindWithCount
    {
        public string TemplateKindKey { get; }
        public long Uses { get; }

        public TemplateKindWithCount(string templateKindKey, long uses)
        {
            TemplateKindKey = templateKindKey;
            Uses = uses;
        }
    }
}
