using FluentValidation;

namespace InvestmentApp.Core.Application.Features.InvestmentAssets.Queries.GetAllWithInclude
{
    public class GetAllWithIncludeInvestmentAssetQueryValidator : AbstractValidator<GetAllWithIncludeInvestmentAssetQuery>
    {
        public GetAllWithIncludeInvestmentAssetQueryValidator()
        {
            RuleFor(a => a.UserId)
                .NotNull().WithMessage("User ID is required.")
                .NotEmpty().WithMessage("User ID cannot be empty.");
        }
    }
}
