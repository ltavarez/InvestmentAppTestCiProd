using AutoMapper;
using InvestmentApp.Core.Application.Dtos.InvestmentAssets;
using InvestmentApp.Core.Domain.Entities;

namespace InvestmentApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class InvestmentAssetsMappingProfile : Profile
    {
        public InvestmentAssetsMappingProfile() {

            CreateMap<InvestmentAssets, InvestmentAssetsDto>()             
             .ForMember(dest => dest.Asset,
                        opt => opt.MapFrom(src => src.Asset))
             .ForMember(dest => dest.InvestmentPortfolio,
                        opt => opt.MapFrom(src => src.InvestmentPortfolio))
             .ReverseMap()      
             .ForMember(dest => dest.Asset, opt => opt.Ignore())
             .ForMember(dest => dest.InvestmentPortfolio, opt => opt.Ignore());
        }
    }
}
