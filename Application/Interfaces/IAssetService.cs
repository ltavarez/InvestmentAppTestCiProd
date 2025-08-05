using InvestmentApp.Core.Application.Dtos.Asset;

namespace InvestmentApp.Core.Application.Interfaces
{
    public interface IAssetService : IGenericService<AssetDto>
    {    
        Task<List<AssetDto>> GetAllWithInclude();
        Task<List<AssetForPortfolioDto>> GetAllAssetsByPortfolioId(int portfolioId, string? assetName = null, int? assetTypeId = null, int? assetOrderBy = null);
    }
}