using AutoMapper;
using Common.DTOs;
using DataAccess.Entities;

namespace Business.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<JobCreateDto, JobApplication>();
            CreateMap<JobUpdateDto, JobApplication>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<JobApplication, JobResponseDto>();
        }
    }
}
