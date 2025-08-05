namespace InvestmentApp.Core.Application.ViewModels.InvestmentAssets
{
    public class DeleteInvestmentAssetViewModel
    {      
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public int AssetId { get; set; }
        public string? AssetName { get; set; }
    }
}
