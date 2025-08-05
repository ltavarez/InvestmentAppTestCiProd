using AutoMapper;
using InvestmentApp.Core.Application.Dtos.AssetType;
using InvestmentApp.Core.Application.ViewModels.AssetType;

namespace InvestmentApp.Core.Application.Mappings.DtosAndViewModels
{
    public class AssetTypeDtoMappingProfile : Profile
    {
        public AssetTypeDtoMappingProfile() {
     
            CreateMap<AssetTypeDto, AssetTypeViewModel>()
                .ReverseMap();
            
            CreateMap<AssetTypeDto, SaveAssetTypeViewModel>()                
                .ReverseMap()                
                .ForMember(dest => dest.AssetQuantity, opt => opt.Ignore());
            
            CreateMap<AssetTypeDto, DeleteAssetTypeViewModel>()           
                .ReverseMap()                
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.AssetQuantity, opt => opt.Ignore());
        }
    }
}
