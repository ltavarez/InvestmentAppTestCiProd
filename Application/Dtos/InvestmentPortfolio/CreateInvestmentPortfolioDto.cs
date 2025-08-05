namespace InvestmentApp.Core.Application.Dtos.InvestmentPortfolio
{
    /// <summary>
    /// DTO used to create a new investment portfolio for a user.
    /// </summary>
    public class CreateInvestmentPortfolioDto
    {
        /// <summary>
        /// Name of the investment portfolio.
        /// </summary>
        /// <example>Long-Term Growth Portfolio</example>
        public required string Name { get; set; }

        /// <summary>
        /// Optional description of the portfolio's purpose or contents.
        /// </summary>
        /// <example>This portfolio is focused on long-term investments in tech stocks and ETFs.</example>
        public string? Description { get; set; }
    }

}
