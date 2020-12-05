using AutoMapper;
using vtb.TemplatesService.Api.Responses;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.MapperProfiles
{
    public class TemplateVersionsProfile : Profile
    {
        public TemplateVersionsProfile()
        {
            CreateMap<TemplateVersion, TemplateVersionListItem>();
            CreateMap<TemplateVersion, TemplateVersionDetails>();
        }
    }
}