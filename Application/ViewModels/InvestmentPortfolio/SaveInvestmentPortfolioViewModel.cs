using System.ComponentModel.DataAnnotations;

namespace InvestmentApp.Core.Application.ViewModels.InvestmentPortfolio
{
    public class SaveInvestmentPortfolioViewModel
    {      
        public int Id { get; set; }

        [Required(ErrorMessage = "You must enter the name of investment portfolio")]
        public required string Name { get; set; }
        public string? Description { get; set; }    

    }
}
