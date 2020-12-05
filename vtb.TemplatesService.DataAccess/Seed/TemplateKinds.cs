using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.DataAccess.Seed
{
    public static class TemplateKinds
    {
        public static readonly TemplateKind InvoiceTemplateKind = new TemplateKind()
        {
            TemplateKindKey = "invoices"
        };

        public static readonly TemplateKind EmailTemplateKind = new TemplateKind()
        {
            TemplateKindKey = "emails"
        };
    }
}