using FluentValidation;

namespace InvestmentApp.Core.Application.Features.InvestmentPortfolios.Commands.CreateInvestmentPortfolio
{
    public class CreateInvestmentPortfolioCommandValidator : AbstractValidator<CreateInvestmentPortfolioCommand>
    {
        public CreateInvestmentPortfolioCommandValidator()
        {
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
