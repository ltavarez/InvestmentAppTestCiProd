namespace InvestmentApp.Core.Application.Dtos.InvestmentPortfolio
{
    public class InvestmentPortfolioDto : BasicDto<int>
    {
        public required string UserId { get; set; } // FK      
    }
}
