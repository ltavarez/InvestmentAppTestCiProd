using AutoMapper;
using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.ViewModels.Asset;

namespace InvestmentApp.Core.Application.Mappings.DtosAndViewModels
{
    public class AssetsDtoMappingProfile : Profile
    {
        public AssetsDtoMappingProfile() {
        
            CreateMap<AssetDto, AssetViewModel>()        
                .ForMember(dest => dest.AssetType,
                           opt => opt.MapFrom(src => src.AssetType))
                .ForMember(dest => dest.AssetHistories,
                           opt => opt.MapFrom(src => src.AssetHistories))
                .ReverseMap()                
                .ForMember(dest => dest.AssetType, opt => opt.Ignore())
                .ForMember(dest => dest.AssetHistories, opt => opt.Ignore());
            
            CreateMap<AssetDto, SaveAssetViewModel>()
                .ReverseMap()
                  .ForMember(dest => dest.AssetType, opt => opt.Ignore())
                .ForMember(dest => dest.AssetHistories, opt => opt.Ignore());

            CreateMap<AssetDto, DeleteAssetViewModel>()
            .ReverseMap()
                .ForMember(dest => dest.Symbol, opt => opt.Ignore())
                .ForMember(dest => dest.AssetTypeId, opt => opt.Ignore())
                .ForMember(dest => dest.AssetType, opt => opt.Ignore())
                .ForMember(dest => dest.AssetHistories, opt => opt.Ignore());

            CreateMap<AssetForPortfolioDto, AssetForPortfolioViewModel>()          
          .ForMember(dest => dest.AssetType,
                     opt => opt.MapFrom(src => src.AssetType))
          .ForMember(dest => dest.CurrentValue,
                     opt => opt.MapFrom(src => src.CurrentValue))          
          .ReverseMap()          
          .ForMember(dest => dest.AssetType, opt => opt.Ignore())
          .ForMember(dest => dest.CurrentValue, opt => opt.Ignore());
        }
    }
}
