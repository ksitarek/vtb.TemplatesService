using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using vtb.TemplatesService.Api.MapperProfiles;
using vtb.TemplatesService.Api.Responses;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.MapperProfiles
{
    public class TemplateProfileTests
    {
        private IMapper _mapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TemplatesProfile>();
            });

            _mapper = mapperConfig.CreateMapper();
        }

        [Test]
        public void Maps_Template_Without_Active_Version_To_TemplateListItem()
        {
            var templateId = Guid.NewGuid();
            var tenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f");
            var dateTime = new DateTimeOffset(DateTime.Now);

            var input = new Template()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                TenantId = tenantId,

                Versions = new List<TemplateVersion>()
                {
                    new TemplateVersion()
                    {
                        TemplateVersionId = Guid.NewGuid(),
                        Content = "Lorem ipsun, dolor sit amet",
                        CreatedAt = dateTime,
                        Version = 1
                    }
                },
            };

            var expectedOutput = new TemplateListItem()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                CurrentVersion = 0,
                CurrentVersionCreatedAt = DateTimeOffset.MinValue,
            };

            var actualOutput = _mapper.Map<TemplateListItem>(input);
            actualOutput.Should().BeEquivalentTo(expectedOutput);
        }

        [Test]
        public void Maps_Template_With_Active_Version_To_TemplateListItem()
        {
            var templateId = Guid.NewGuid();
            var tenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f");
            var dateTime = new DateTimeOffset(DateTime.Now);

            var input = new Template()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                TenantId = tenantId,

                Versions = new List<TemplateVersion>()
                {
                    new TemplateVersion()
                    {
                        TemplateVersionId = Guid.NewGuid(),
                        Content = "Lorem ipsun, dolor sit amet",
                        CreatedAt = dateTime,
                        Version = 1,
                        IsActive = true
                    }
                },
            };

            var expectedOutput = new TemplateListItem()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                CurrentVersion = 1,
                CurrentVersionCreatedAt = dateTime,
            };

            var actualOutput = _mapper.Map<TemplateListItem>(input);
            actualOutput.Should().BeEquivalentTo(expectedOutput);
        }

        [Test]
        public void Maps_Template_With_Null_Versions_Collection_To_TemplateListItem()
        {
            var templateId = Guid.NewGuid();
            var tenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f");

            var input = new Template()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                TenantId = tenantId,

                Versions = null,
            };

            var expectedOutput = new TemplateListItem()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                CurrentVersion = 0,
                CurrentVersionCreatedAt = DateTimeOffset.MinValue,
            };

            var actualOutput = _mapper.Map<TemplateListItem>(input);
            actualOutput.Should().BeEquivalentTo(expectedOutput);
        }

        [Test]
        public void Maps_Template_With_Empty_Versions_Collection_To_TemplateListItem()
        {
            var templateId = Guid.NewGuid();
            var tenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f");

            var input = new Template()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                TenantId = tenantId,
                Versions = new List<TemplateVersion>(),
            };

            var expectedOutput = new TemplateListItem()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                CurrentVersion = 0,
                CurrentVersionCreatedAt = DateTimeOffset.MinValue,
            };

            var actualOutput = _mapper.Map<TemplateListItem>(input);
            actualOutput.Should().BeEquivalentTo(expectedOutput);
        }

        [Test]
        public void Maps_Template_Without_Active_Version_To_TemplateDetails()
        {
            var templateId = Guid.NewGuid();
            var tenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f");
            var dateTime = new DateTimeOffset(DateTime.Now);
            var versionId = Guid.NewGuid();

            var input = new Template()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                TenantId = tenantId,

                Versions = new List<TemplateVersion>()
                {
                    new TemplateVersion()
                    {
                        TemplateVersionId = Guid.NewGuid(),
                        Content = "Lorem ipsun, dolor sit amet",
                        CreatedAt = dateTime,
                        Version = 1
                    }
                },
            };

            var expectedOutput = new TemplateDetails()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                CurrentVersionId = Guid.Empty,
                CurrentVersion = 0,
                CurrentVersionContent = null,
                CurrentVersionCreatedAt = DateTimeOffset.MinValue,
            };

            var actualOutput = _mapper.Map<TemplateDetails>(input);
            actualOutput.Should().BeEquivalentTo(expectedOutput);
        }

        [Test]
        public void Maps_Template_With_Active_Version_To_TemplateDetails()
        {
            var templateId = Guid.NewGuid();
            var templateVersionId = Guid.NewGuid();
            var tenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f");
            var dateTime = new DateTimeOffset(DateTime.Now);

            var input = new Template()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                TenantId = tenantId,

                Versions = new List<TemplateVersion>()
                {
                    new TemplateVersion()
                    {
                        TemplateVersionId = templateVersionId,
                        Content = "Lorem ipsun, dolor sit amet",
                        CreatedAt = dateTime,
                        UpdatedAt = dateTime.AddDays(3),
                        Version = 1,
                        IsActive = true
                    }
                },
            };

            var expectedOutput = new TemplateDetails()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                CurrentVersionId = templateVersionId,
                CurrentVersion = 1,
                CurrentVersionContent = "Lorem ipsun, dolor sit amet",
                CurrentVersionCreatedAt = dateTime,
                CurrentVersionUpdatedAt = dateTime.AddDays(3)
            };

            var actualOutput = _mapper.Map<TemplateDetails>(input);
            actualOutput.Should().BeEquivalentTo(expectedOutput);
        }

        [Test]
        public void Maps_Template_With_Null_Versions_Collection_To_TemplateDetails()
        {
            var templateId = Guid.NewGuid();
            var tenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f");

            var input = new Template()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                TenantId = tenantId,

                Versions = null,
            };

            var expectedOutput = new TemplateDetails()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                CurrentVersionId = Guid.Empty,
                CurrentVersion = 0,
                CurrentVersionContent = null,
                CurrentVersionCreatedAt = DateTimeOffset.MinValue,
            };

            var actualOutput = _mapper.Map<TemplateDetails>(input);
            actualOutput.Should().BeEquivalentTo(expectedOutput);
        }

        [Test]
        public void Maps_Template_With_Empty_Versions_Collection_To_TemplateDetails()
        {
            var templateId = Guid.NewGuid();
            var tenantId = new Guid("b0dfe5bf-c621-4de3-a657-b991b9384b0f");

            var input = new Template()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                TenantId = tenantId,
                Versions = new List<TemplateVersion>(),
            };

            var expectedOutput = new TemplateDetails()
            {
                TemplateId = templateId,
                TemplateKindKey = "tk1",
                Label = "Tenant 1 Template Kind 1 Template 1",
                CurrentVersionId = Guid.Empty,
                CurrentVersion = 0,
                CurrentVersionContent = null,
                CurrentVersionCreatedAt = DateTimeOffset.MinValue,
            };

            var actualOutput = _mapper.Map<TemplateDetails>(input);
            actualOutput.Should().BeEquivalentTo(expectedOutput);
        }
    }
}