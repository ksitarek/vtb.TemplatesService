using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using System;
using vtb.TemplatesService.Api.MapperProfiles;
using vtb.TemplatesService.Api.Responses;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.MapperProfiles
{
    public class TemplateVersionsProfileTest
    {
        private IMapper _mapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TemplateVersionsProfile>();
            });

            _mapper = mapperConfig.CreateMapper();
        }

        [Test]
        public void Will_Map_TemplateVersion_To_TemplateVersionListItem()
        {
            var version = new TemplateVersion()
            {
                TemplateVersionId = Guid.NewGuid(),
                Content = "Lorem ipsun, dolor sit amet",
                CreatedAt = DateTimeOffset.UtcNow,
                Version = 1,
                IsActive = true
            };

            var expectedListItem = new TemplateVersionListItem()
            {
                TemplateVersionId = version.TemplateVersionId,
                CreatedAt = version.CreatedAt,
                Version = version.Version,
                IsActive = version.IsActive
            };

            var mapped = _mapper.Map<TemplateVersionListItem>(version);
            mapped.Should().BeEquivalentTo(expectedListItem);
        }

        [Test]
        public void Will_Map_TemplateVersion_To_TemplateVersionDetails()
        {
            var version = new TemplateVersion()
            {
                TemplateVersionId = Guid.NewGuid(),
                Content = "Lorem ipsun, dolor sit amet",
                CreatedAt = DateTimeOffset.UtcNow,
                Version = 1,
                IsActive = true
            };

            var expectedVersionDetails = new TemplateVersionDetails()
            {
                TemplateVersionId = version.TemplateVersionId,
                CreatedAt = version.CreatedAt,
                Version = version.Version,
                IsActive = version.IsActive,
                Content = version.Content
            };

            var mapped = _mapper.Map<TemplateVersionDetails>(version);
            mapped.Should().BeEquivalentTo(expectedVersionDetails);
        }
    }
}