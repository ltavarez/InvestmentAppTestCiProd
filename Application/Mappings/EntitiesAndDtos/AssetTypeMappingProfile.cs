using AutoMapper;
using InvestmentApp.Core.Application.Dtos.AssetType;
using InvestmentApp.Core.Domain.Entities;

namespace InvestmentApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class AssetTypeMappingProfile : Profile
    {
        public AssetTypeMappingProfile() {

            CreateMap<AssetType, AssetTypeDto>()
                .ForMember(dest => dest.AssetQuantity, 
                           opt => opt.MapFrom(src => src.Assets != null ? src.Assets.Count : 0))
                .ReverseMap()
                .ForMember(dest=> dest.Assets,opt=>opt.Ignore());
        }
    }
}
