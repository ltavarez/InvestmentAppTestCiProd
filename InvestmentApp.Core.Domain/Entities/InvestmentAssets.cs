namespace InvestmentApp.Core.Domain.Entities
{
    public class InvestmentAssets
    {
        public required int Id { get; set; }
        public required int AssetId { get; set; } //FK
        public Asset? Asset { get; set; } //Navigation
        public required int InvestmentPortfolioId { get; set; } //FK
        public InvestmentPortfolio? InvestmentPortfolio { get; set; } //Navigation
        public DateTime AssociationDate { get; set; } = DateTime.UtcNow;
    }
}
