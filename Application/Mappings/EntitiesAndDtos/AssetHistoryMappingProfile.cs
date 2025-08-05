using AutoMapper;
using InvestmentApp.Core.Application.Dtos.AssetHistory;
using InvestmentApp.Core.Domain.Entities;

namespace InvestmentApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class AssetHistoryMappingProfile : Profile
    {
        public AssetHistoryMappingProfile() {

            CreateMap<AssetHistory, AssetHistoryDto>()      
           .ForMember(dest => dest.Asset,
                      opt => opt.MapFrom(src => src.Asset))           
           .ReverseMap()           
           .ForMember(dest => dest.Asset, opt => opt.Ignore());

        }
    }
}
