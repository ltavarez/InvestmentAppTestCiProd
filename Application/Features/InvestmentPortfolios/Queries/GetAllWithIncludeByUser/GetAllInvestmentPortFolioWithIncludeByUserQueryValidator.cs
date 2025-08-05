using FluentValidation;

namespace InvestmentApp.Core.Application.Features.InvestmentPortfolios.Queries.GetAllWithIncludeByUser
{
    public class GetAllInvestmentPortFolioWithIncludeByUserQueryValidator : AbstractValidator<GetAllInvestmentPortFolioWithIncludeByUserQuery>
    {
        public GetAllInvestmentPortFolioWithIncludeByUserQueryValidator()
        {
            RuleFor(a => a.UserId)
            .NotNull().WithMessage("User ID is required.")
            .NotEmpty().WithMessage("User ID cannot be empty.");
        }
    }
}
