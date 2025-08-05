using FluentValidation;

namespace InvestmentApp.Core.Application.Features.InvestmentAssets.Queries.GetById
{
    public class GetByIdInvestmentAssetQueryValidator : AbstractValidator<GetByIdInvestmentAssetQuery>
    {  
        public GetByIdInvestmentAssetQueryValidator()
        {  
            RuleFor(a => a.Id)
                .NotNull().WithMessage("Investment Asset ID is required.")
                .GreaterThan(0).WithMessage("Investment Asset ID must be greater than 0.");

            RuleFor(a => a.UserId)
                .NotNull().WithMessage("User ID is required.")
                .NotEmpty().WithMessage("User ID cannot be empty.");
        }
    }
}
