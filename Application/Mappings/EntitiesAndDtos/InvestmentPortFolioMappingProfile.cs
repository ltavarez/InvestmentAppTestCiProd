using AutoMapper;
using InvestmentApp.Core.Application.Dtos.InvestmentPortfolio;
using InvestmentApp.Core.Domain.Entities;

namespace InvestmentApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class InvestmentPortFolioMappingProfile : Profile
    {
        public InvestmentPortFolioMappingProfile() {

            CreateMap<InvestmentPortfolio, InvestmentPortfolioDto>()       
           .ReverseMap() 
           .ForMember(dest => dest.InvestmentAssets, opt => opt.Ignore());
        }
    }
}
