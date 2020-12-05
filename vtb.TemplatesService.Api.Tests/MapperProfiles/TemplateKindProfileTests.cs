using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using vtb.TemplatesService.Api.MapperProfiles;
using vtb.TemplatesService.Api.Responses;
using vtb.TemplatesService.DataAccess.DTOs;

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
        public void Maps_TemplateKindWithCount_To_TemplateKindListItem()
        {
            var input = new TemplateKindWithCount("tk-1", int.MaxValue);
            var expectedOutput = new TemplateKindListItem() { TemplateKindKey = "tk-1", Uses = int.MaxValue };

            var actualOutput = _mapper.Map<TemplateKindListItem>(input);
            actualOutput.Should().BeEquivalentTo(expectedOutput);
        }
    }
}