using InvestmentApp.Core.Application.ViewModels.Asset;
using InvestmentApp.Core.Application.ViewModels.InvestmentPortfolio;

namespace InvestmentApp.Core.Application.ViewModels.InvestmentAssets
{
    public class InvestmentAssetsViewModel 
    {
        public required int Id { get; set; }
        public required int AssetId { get; set; } 
        public AssetViewModel? Asset { get; set; }
        public required int InvestmentPortfolioId { get; set; }
        public InvestmentPortfolioViewModel? InvestmentPortfolio { get; set; } 
        public DateTime AssociationDate { get; set; } = DateTime.UtcNow;
    }
}
