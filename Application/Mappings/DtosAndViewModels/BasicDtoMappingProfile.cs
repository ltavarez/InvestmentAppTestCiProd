using AutoMapper;
using InvestmentApp.Core.Application.Dtos;
using InvestmentApp.Core.Application.ViewModels;

namespace InvestmentApp.Core.Application.Mappings.DtosAndViewModels
{
    public class BasicDtoMappingProfile : Profile
    {
        public BasicDtoMappingProfile() {

            CreateMap(typeof(BasicDto<>), typeof(BasicViewModel<>)).ReverseMap();
        }
    }
}
