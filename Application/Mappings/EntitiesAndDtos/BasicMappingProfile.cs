using AutoMapper;
using InvestmentApp.Core.Application.Dtos;
using InvestmentApp.Core.Domain.Common;

namespace InvestmentApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class BasicMappingProfile : Profile
    {
        public BasicMappingProfile() {

            CreateMap(typeof(BasicDto<>), typeof(BasicEntity<>)).ReverseMap();
        }
    }
}
