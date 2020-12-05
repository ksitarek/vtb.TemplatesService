using AutoMapper;
using vtb.TemplatesService.Api.Responses;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.Api.MapperProfiles
{
    public class TemplatesProfile : Profile
    {
        public TemplatesProfile()
        {
            CreateMap<Template, TemplateListItem>()
                .ForMember(x => x.CurrentVersion, opt =>
                {
                    opt.PreCondition(x => x.ActiveVersion != null);
                    opt.MapFrom(x => x.ActiveVersion.Version);
                })
                .ForMember(x => x.CurrentVersionCreatedAt, opt =>
                {
                    opt.PreCondition(x => x.ActiveVersion != null);
                    opt.MapFrom(x => x.ActiveVersion.CreatedAt);
                });

            CreateMap<Template, TemplateDetails>()
                .ForMember(x => x.CurrentVersionId, opt =>
                {
                    opt.PreCondition(x => x.ActiveVersion != null);
                    opt.MapFrom(x => x.ActiveVersion.TemplateVersionId);
                })
                .ForMember(x => x.CurrentVersionContent, opt =>
                {
                    opt.PreCondition(x => x.ActiveVersion != null);
                    opt.MapFrom(x => x.ActiveVersion.Content);
                })
                .ForMember(x => x.CurrentVersion, opt =>
                {
                    opt.PreCondition(x => x.ActiveVersion != null);
                    opt.MapFrom(x => x.ActiveVersion.Version);
                })
                .ForMember(x => x.CurrentVersionCreatedAt, opt =>
                {
                    opt.PreCondition(x => x.ActiveVersion != null);
                    opt.MapFrom(x => x.ActiveVersion.CreatedAt);
                })
                .ForMember(x => x.CurrentVersionUpdatedAt, opt =>
                {
                    opt.PreCondition(x => x.ActiveVersion != null);
                    opt.MapFrom(x => x.ActiveVersion.UpdatedAt);
                });
        }
    }
}