using AutoMapper;
using vtb.TemplatesService.Api.Responses;
using vtb.TemplatesService.DataAccess.DTOs;

namespace vtb.TemplatesService.Api.MapperProfiles
{
    public class TemplateKindsProfile : Profile
    {
        public TemplateKindsProfile()
        {
            //CreateMap<TemplateKind, TemplateKindListItem>();
            CreateMap<TemplateKindWithCount, TemplateKindListItem>();
        }
    }
}