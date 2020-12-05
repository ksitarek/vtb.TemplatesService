using System;
using System.Collections.Generic;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.DataAccess.Seed
{
    public static class Templates
    {
        public readonly static Guid Tenant1Id = Guid.Parse("862f1c70-d5fa-4c20-84e2-f4416ba7fd4b");
        public readonly static Guid Tenant2Id = Guid.Parse("7e492fff-28f2-4f09-b356-fd4ee0d35452");

        public static readonly Template Tenant1FirstInvoiceTemplate = new Template()
        {
            TemplateId = Guid.Parse("7813afd7-8a03-491a-8517-2eb878b103e1"),
            TenantId = Tenant1Id,
            Label = "Tenant 1 / Default Invoice Template",
            IsDefault = true,
            TemplateKindKey = "invoices",
            Versions = new List<TemplateVersion>()
            {
                new TemplateVersion()
                {
                    TemplateVersionId = Guid.Parse("76ace0ec-7e80-47f5-8d6d-3bd4770a2b58"),
                    Content = "Lorem ipsum, dolor sit amet",
                    Version = 1,
                    CreatedAt = new DateTimeOffset(2020, 09, 21, 6, 47, 23, 0, TimeSpan.FromHours(1)),
                    UpdatedAt = new DateTimeOffset(2020, 09, 21, 6, 47, 23, 0, TimeSpan.FromHours(1)),
                    IsActive = true
                },
                new TemplateVersion()
                {
                    TemplateVersionId = Guid.Parse("264ecdf4-65a1-4362-b7cd-c51ed92223c7"),
                    Content = "Lorem ipsum, dolor sit amet, consecteur...",
                    Version = 2,
                    CreatedAt = new DateTimeOffset(2020, 09, 21, 6, 49, 32, 0, TimeSpan.FromHours(1)),
                    UpdatedAt = new DateTimeOffset(2020, 09, 21, 6, 49, 32, 0, TimeSpan.FromHours(1)),
                    IsActive = false
                }
            }
        };

        public static readonly Template Tenant1SecondInvoiceTemplate = new Template()
        {
            TemplateId = Guid.Parse("0f6d4d20-dcdd-4909-9318-da2ea754f91e"),
            TenantId = Tenant1Id,
            Label = "Tenant 1 / Secondary Invoice Template",
            IsDefault = true,
            TemplateKindKey = "invoices",
            Versions = new List<TemplateVersion>()
            {
                new TemplateVersion()
                {
                    TemplateVersionId = Guid.Parse("08ea44df-77ca-4581-a85f-3f506eadcf15"),
                    Content = "Fusce sed nunc vitae elit vehicula convallis a eu turpis.",
                    Version = 1,
                    CreatedAt = new DateTimeOffset(2020, 09, 21, 6, 47, 23, 0, TimeSpan.FromHours(1)),
                    UpdatedAt = new DateTimeOffset(2020, 09, 21, 6, 47, 23, 0, TimeSpan.FromHours(1)),
                    IsActive = false
                },
                new TemplateVersion()
                {
                    TemplateVersionId = Guid.Parse("926d0fc7-bb35-41db-97e1-ff685ec35737"),
                    Content = "Praesent laoreet non risus vel vestibulum. Integer dignissim metus est, sit amet placerat dolor congue ac. Sed lacinia turpis augue, at semper elit hendrerit id. Suspendisse potenti. Ut tortor ipsum, tincidunt vel lectus vel, pellentesque congue dui. Cras mattis, ex vitae posuere pellentesque, ex nulla gravida sem, ac iaculis sem diam eu elit. Fusce a urna varius leo commodo auctor. Sed dapibus facilisis aliquet. Aenean mollis magna et sodales ultrices.",
                    Version = 2,
                    UpdatedAt = new DateTimeOffset(2020, 09, 21, 6, 49, 32, 0, TimeSpan.FromHours(1)),
                    IsActive = true
                }
            }
        };

        public static readonly Template Tenant1InvoiceTemplateWithoutActiveVersion = new Template()
        {
            TemplateId = Guid.Parse("0f6d4d20-dcdd-4909-9318-da2ea754f91e"),
            TenantId = Tenant1Id,
            Label = "Tenant 1 / Secondary Invoice Template",
            IsDefault = true,
            TemplateKindKey = "invoices",
            Versions = new List<TemplateVersion>()
            {
                new TemplateVersion()
                {
                    TemplateVersionId = Guid.Parse("08ea44df-77ca-4581-a85f-3f506eadcf15"),
                    Content = "Fusce sed nunc vitae elit vehicula convallis a eu turpis.",
                    Version = 1,
                    CreatedAt = new DateTimeOffset(2020, 09, 21, 6, 47, 23, 0, TimeSpan.FromHours(1)),
                    UpdatedAt = new DateTimeOffset(2020, 09, 21, 6, 47, 23, 0, TimeSpan.FromHours(1)),
                    IsActive = false
                },
                new TemplateVersion()
                {
                    TemplateVersionId = Guid.Parse("926d0fc7-bb35-41db-97e1-ff685ec35737"),
                    Content = "Praesent laoreet non risus vel vestibulum. Integer dignissim metus est, sit amet placerat dolor congue ac. Sed lacinia turpis augue, at semper elit hendrerit id. Suspendisse potenti. Ut tortor ipsum, tincidunt vel lectus vel, pellentesque congue dui. Cras mattis, ex vitae posuere pellentesque, ex nulla gravida sem, ac iaculis sem diam eu elit. Fusce a urna varius leo commodo auctor. Sed dapibus facilisis aliquet. Aenean mollis magna et sodales ultrices.",
                    Version = 2,
                    UpdatedAt = new DateTimeOffset(2020, 09, 21, 6, 49, 32, 0, TimeSpan.FromHours(1)),
                    IsActive = false
                }
            }
        };

        public static readonly Template Tenant2FirstInvoiceTemplate = new Template()
        {
            TemplateId = Guid.Parse("e4807fbe-b132-4ffe-a87d-23c78ac337ec"),
            TenantId = Tenant2Id,
            Label = "Tenant 2 / Default Invoice Template",
            IsDefault = true,
            TemplateKindKey = "invoices",
            Versions = new List<TemplateVersion>()
            {
                new TemplateVersion()
                {
                    TemplateVersionId = Guid.Parse("5013a7b4-4987-4cbb-9189-e74812c8112b"),
                    Content = "Lorem ipsum, dolor sit amet",
                    Version = 1,
                    CreatedAt = new DateTimeOffset(2020, 09, 21, 6, 47, 23, 0, TimeSpan.FromHours(1)),
                    IsActive = true
                },
                new TemplateVersion()
                {
                    TemplateVersionId = Guid.Parse("56176aff-42d7-4c3c-8203-927fca59cfff"),
                    Content = "Lorem ipsum, dolor sit amet, consecteur...",
                    Version = 2,
                    CreatedAt = new DateTimeOffset(2020, 09, 21, 6, 49, 32, 0, TimeSpan.FromHours(1)),
                    IsActive = false
                }
            }
        };

        public static readonly Template Tenant2SecondInvoiceTemplate = new Template()
        {
            TemplateId = Guid.Parse("48a771af-0460-4d11-9286-a64c9bfd7383"),
            TenantId = Tenant2Id,
            Label = "Tenant 2 / Secondary Invoice Template",
            IsDefault = true,
            TemplateKindKey = "invoices",
            Versions = new List<TemplateVersion>()
            {
                new TemplateVersion()
                {
                    TemplateVersionId = Guid.Parse("2b19ec37-e10d-4cda-b8e0-64befcea0f95"),
                    Content = "Fusce sed nunc vitae elit vehicula convallis a eu turpis.",
                    Version = 1,
                    CreatedAt = new DateTimeOffset(2020, 09, 21, 6, 47, 23, 0, TimeSpan.FromHours(1)),
                    IsActive = false
                },
                new TemplateVersion()
                {
                    TemplateVersionId = Guid.Parse("9e4103dc-5c97-4faf-8ecb-f0603b2bd562"),
                    Content = "Praesent laoreet non risus vel vestibulum. Integer dignissim metus est, sit amet placerat dolor congue ac. Sed lacinia turpis augue, at semper elit hendrerit id. Suspendisse potenti. Ut tortor ipsum, tincidunt vel lectus vel, pellentesque congue dui. Cras mattis, ex vitae posuere pellentesque, ex nulla gravida sem, ac iaculis sem diam eu elit. Fusce a urna varius leo commodo auctor. Sed dapibus facilisis aliquet. Aenean mollis magna et sodales ultrices.",
                    Version = 2,
                    CreatedAt = new DateTimeOffset(2020, 09, 21, 6, 49, 32, 0, TimeSpan.FromHours(1)),
                    IsActive = true
                }
            }
        };
    }
}