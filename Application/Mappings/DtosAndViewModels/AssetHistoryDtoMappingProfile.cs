using AutoMapper;
using InvestmentApp.Core.Application.Dtos.AssetHistory;
using InvestmentApp.Core.Application.ViewModels.AssetHistory;

namespace InvestmentApp.Core.Application.Mappings.DtosAndViewModels
{
    public class AssetHistoryDtoMappingProfile : Profile
    {
        public AssetHistoryDtoMappingProfile() {
            
            CreateMap<AssetHistoryDto, AssetHistoryViewModel>()
                .ForMember(dest => dest.Asset,
                           opt => opt.MapFrom(src => src.Asset))
                .ReverseMap()
                .ForMember(dest => dest.Asset, opt => opt.Ignore());

     
            CreateMap<AssetHistoryDto, SaveAssetHistoryViewModel>()           
                .ReverseMap()      
                .ForMember(dest => dest.Asset, opt => opt.Ignore());

            CreateMap<AssetHistoryDto, DeleteAssetHistoryViewModel>()
                .ForMember(dest => dest.HistoryValueDate,
                           opt => opt.MapFrom(src => src.HistoryValueDate.ToString("yyyy-MM-dd")))
                .ReverseMap()              
                .ForMember(dest => dest.HistoryValueDate, opt => opt.Ignore())
                .ForMember(dest => dest.Value, opt => opt.Ignore())
                .ForMember(dest => dest.AssetId, opt => opt.Ignore())
                .ForMember(dest => dest.Asset, opt => opt.Ignore());

        }
    }
}
