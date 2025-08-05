using AutoMapper;
using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.Dtos.AssetHistory;
using InvestmentApp.Core.Domain.Entities;

namespace InvestmentApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class AssetMappingProfile : Profile
    {
        public AssetMappingProfile() {

         CreateMap<Asset, AssetDto>()           
          .ForMember(dest => dest.AssetType,
                     opt => opt.MapFrom(src => src.AssetType))          
          .ForMember(dest => dest.AssetHistories,
                     opt => opt.MapFrom(src => src.AssetHistories == null
                    ? new List<AssetHistoryDto>()
                    : src.AssetHistories
                    .OrderByDescending(ah => ah.HistoryValueDate)
                    .Select(s => new AssetHistoryDto()
                    {
                        AssetId = s.AssetId,
                        Id = s.Id,
                        HistoryValueDate = s.HistoryValueDate,
                        Value = s.Value
                    }).ToList() ))
          .ReverseMap()          
          .ForMember(dest => dest.AssetType, opt => opt.Ignore())
          .ForMember(dest => dest.AssetHistories, opt => opt.Ignore())
          .ForMember(dest => dest.InvestmentAssets, opt => opt.Ignore());

          CreateMap<Asset, AssetForPortfolioDto>()    
           .ForMember(dest => dest.AssetType,
               opt => opt.MapFrom(src => src.AssetType))  
           .ForMember(dest => dest.CurrentValue,
               opt => opt.MapFrom(src => 
                   src.AssetHistories != null && src.AssetHistories.Any() ?
                   src.AssetHistories.OrderByDescending(ah => ah.HistoryValueDate)
                   .First().Value : 0m
                ))
           .ReverseMap()
                .ForMember(dest => dest.AssetType, opt => opt.Ignore())
                .ForMember(dest => dest.AssetHistories, opt => opt.Ignore())
                .ForMember(dest => dest.InvestmentAssets, opt => opt.Ignore());

            CreateMap<Asset, AssetBasicDto>()
            .ReverseMap()
            .ForMember(dest => dest.AssetType, opt => opt.Ignore())
            .ForMember(dest => dest.AssetHistories, opt => opt.Ignore())
            .ForMember(dest => dest.InvestmentAssets, opt => opt.Ignore());
        }
    }
}
