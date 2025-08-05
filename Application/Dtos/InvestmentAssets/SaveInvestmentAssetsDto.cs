namespace InvestmentApp.Core.Application.Dtos.InvestmentAssets
{
    public class SaveInvestmentAssetsDto
    {
        public required int AssetId { get; set; }
        public required int InvestmentPortfolioId { get; set; }
        public DateTime AssociationDate { get; set; } = DateTime.UtcNow;
    }
}
