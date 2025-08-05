using AutoMapper;
using InvestmentApp.Core.Application.Dtos.InvestmentAssets;
using InvestmentApp.Core.Application.ViewModels.InvestmentAssets;

namespace InvestmentApp.Core.Application.Mappings.DtosAndViewModels
{
    public class InvestmentAssetsDtoMappingProfile : Profile
    {
        public InvestmentAssetsDtoMappingProfile() {
           
            CreateMap<InvestmentAssetsDto, SaveInvestmentAssetViewModel>()
                .ReverseMap()                
                .ForMember(dest => dest.Asset, opt => opt.Ignore())
                .ForMember(dest => dest.InvestmentPortfolio, opt => opt.Ignore())
                .ForMember(dest => dest.AssociationDate, opt => opt.Ignore());
            
            CreateMap<InvestmentAssetsDto, InvestmentAssetsViewModel>()
                .ForMember(dest => dest.Asset,
                           opt => opt.MapFrom(src => src.Asset))
                .ForMember(dest => dest.InvestmentPortfolio,
                           opt => opt.MapFrom(src => src.InvestmentPortfolio))
                .ReverseMap()                
                .ForMember(dest => dest.Asset, opt => opt.Ignore())
                .ForMember(dest => dest.InvestmentPortfolio, opt => opt.Ignore());
            
            CreateMap<InvestmentAssetsDto, DeleteInvestmentAssetViewModel>()
                .ForMember(dest => dest.PortfolioId,
                           opt => opt.MapFrom(src => src.InvestmentPortfolioId))
                .ForMember(dest => dest.AssetName,
                           opt => opt.MapFrom(src => src.Asset != null ? src.Asset.Name : null))
                .ReverseMap()                
                .ForMember(dest => dest.Asset, opt => opt.Ignore())
                .ForMember(dest => dest.InvestmentPortfolio, opt => opt.Ignore())
                .ForMember(dest => dest.AssociationDate, opt => opt.Ignore());
        }
    }
}