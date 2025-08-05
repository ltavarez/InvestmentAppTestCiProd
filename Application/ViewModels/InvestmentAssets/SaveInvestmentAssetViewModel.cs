using System.ComponentModel.DataAnnotations;

namespace InvestmentApp.Core.Application.ViewModels.InvestmentAssets
{
    public class SaveInvestmentAssetViewModel
    {
        public required int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "You must enter the valid asset")]
        public required int AssetId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "You must enter the valid investment portfolio")]
        public required int InvestmentPortfolioId { get; set; }   
    }
}
