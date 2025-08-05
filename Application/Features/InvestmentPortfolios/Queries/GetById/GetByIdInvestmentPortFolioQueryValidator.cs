using FluentValidation;

namespace InvestmentApp.Core.Application.Features.InvestmentPortfolios.Queries.GetById
{
    public class GetByIdInvestmentPortFolioQueryValidator : AbstractValidator<GetByIdInvestmentPortFolioQuery>
    {
        public GetByIdInvestmentPortFolioQueryValidator()
        {
            RuleFor(a => a.Id)
                .NotNull().WithMessage("Investment Portfolio ID is required.")
                .GreaterThan(0).WithMessage("Investment Portfolio ID must be greater than zero.");

            RuleFor(a => a.UserId)
                .NotNull().WithMessage("User ID is required.")
                .NotEmpty().WithMessage("User ID cannot be empty.");
        }
    }
}
