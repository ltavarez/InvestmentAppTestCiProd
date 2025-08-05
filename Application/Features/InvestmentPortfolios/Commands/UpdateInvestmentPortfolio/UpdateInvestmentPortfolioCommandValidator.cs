using FluentValidation;

namespace InvestmentApp.Core.Application.Features.InvestmentPortfolios.Commands.UpdateInvestmentPortfolio
{
    public class UpdateInvestmentPortfolioCommandValidator : AbstractValidator<UpdateInvestmentPortfolioCommand>
    {
        public UpdateInvestmentPortfolioCommandValidator()
        {
            RuleFor(a=> a.Id)
                .NotNull().WithMessage("Investment Portfolio ID is required.")
                .GreaterThan(0).WithMessage("Investment Portfolio ID must be greater than zero.");

            RuleFor(a => a.UserId)
              .NotNull().WithMessage("User ID is required.")
              .NotEmpty().WithMessage("User ID cannot be empty.");

            RuleFor(at=> at.Name)
                .NotEmpty().WithMessage("Asset type name is required 2.")
                .MaximumLength(150).WithMessage("Asset type name must not exceed 150 characters.");

            RuleFor(at => at.Description)
                .MaximumLength(250).WithMessage("Asset type description must not exceed 250 characters.");
        }
    }
}
