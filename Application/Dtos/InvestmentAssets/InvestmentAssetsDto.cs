using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.Dtos.InvestmentPortfolio;

namespace InvestmentApp.Core.Application.Dtos.InvestmentAssets
{
    public class InvestmentAssetsDto
    {
        public required int Id { get; set; }
        public required int AssetId { get; set; }
        public AssetDto? Asset { get; set; }
        public required int InvestmentPortfolioId { get; set; }
        public InvestmentPortfolioDto? InvestmentPortfolio { get; set; }
        public DateTime AssociationDate { get; set; } = DateTime.UtcNow;
    }
}
