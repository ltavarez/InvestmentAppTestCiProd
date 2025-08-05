using InvestmentApp.Core.Application.Dtos.InvestmentAssets;

namespace InvestmentApp.Core.Application.Interfaces
{
    public interface IInvestmentAssetsService : IGenericService<InvestmentAssetsDto>
    { 
        Task<List<InvestmentAssetsDto>> GetAllWithInclude(string userId);
        Task<InvestmentAssetsDto?> GetById(int id, string userId);
        Task<InvestmentAssetsDto?> GetByAssetAndPortfolioAsync(int assetId, int portfolioId, string userId);
    }
}