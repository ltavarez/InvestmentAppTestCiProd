using AutoMapper;
using InvestmentApp.Core.Application.Dtos.InvestmentPortfolio;
using InvestmentApp.Core.Application.ViewModels.InvestmentPortfolio;

namespace InvestmentApp.Core.Application.Mappings.DtosAndViewModels
{
    public class InvestmentPortfolioDtoMappingProfile : Profile
    {
        public InvestmentPortfolioDtoMappingProfile() {
           
            CreateMap<InvestmentPortfolioDto, SaveInvestmentPortfolioViewModel>()  
                .ReverseMap()   
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<InvestmentPortfolioDto, InvestmentPortfolioViewModel>()
                .ReverseMap();

            CreateMap<InvestmentPortfolioDto, DeleteInvestmentPortfolioViewModel>()
                .ReverseMap()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());              
        }
    }
}