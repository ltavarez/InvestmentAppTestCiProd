namespace InvestmentApp.Core.Application.Dtos.InvestmentPortfolio
{
    public class SaveInvestmentPortfolioDto 
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string UserId { get; set; } // FK      
    }
}
