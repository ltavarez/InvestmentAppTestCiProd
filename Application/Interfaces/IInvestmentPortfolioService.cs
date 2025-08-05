using InvestmentApp.Core.Application.Dtos.InvestmentPortfolio;

namespace InvestmentApp.Core.Application.Interfaces
{
    public interface IInvestmentPortfolioService : IGenericService<InvestmentPortfolioDto>
    {
        Task<List<InvestmentPortfolioDto>> GetAllWithIncludeByUser(string userId);

        Task<InvestmentPortfolioDto?> GetById(int id, string userId);
    }
}