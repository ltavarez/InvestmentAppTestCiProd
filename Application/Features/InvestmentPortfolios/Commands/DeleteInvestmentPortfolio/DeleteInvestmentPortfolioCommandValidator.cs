using FluentValidation;

namespace InvestmentApp.Core.Application.Features.InvestmentPortfolios.Commands.DeleteInvestmentPortfolio
{
    public class DeleteInvestmentPortfolioCommandValidator : AbstractValidator<DeleteInvestmentPortfolioCommand>
    {
        public DeleteInvestmentPortfolioCommandValidator()
        {
            RuleFor(a => a.Id)
                .NotNull().WithMessage("Investment Portfolio ID is required.")
                .GreaterThan(0).WithMessage("Investment Portfolio ID must be greater than zero.");
        }
    }
}
