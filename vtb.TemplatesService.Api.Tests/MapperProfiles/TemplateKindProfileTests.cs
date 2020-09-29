using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using vtb.TemplatesService.Api.MapperProfiles;
using vtb.TemplatesService.Api.Responses;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.Tests.MapperProfiles
{
    public class TemplateKindProfileTests
    {
        private IMapper _mapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TemplateKindsProfile>();
            });

            _mapper = mapperConfig.CreateMapper();
        }

        [Test]
        public void Maps_TemplateKind_To_TemplateKindListItem()
        {
            var input = new TemplateKind() { TemplateKindKey = "tk-1" };
            var expectedOutput = new TemplateKindListItem() { TemplateKindKey = "tk-1" };

            var actualOutput = _mapper.Map<TemplateKindListItem>(input);
            actualOutput.Should().BeEquivalentTo(expectedOutput);
        }
    }
}