using FluentValidation;

namespace InvestmentApp.Core.Application.Features.Assets.Queries.GetById
{
    public class GetByIdAssetQueryValidator : AbstractValidator<GetByIdAssetQuery>
    {      
        public GetByIdAssetQueryValidator()
        {
            RuleFor(a => a.Id)
                .GreaterThan(0).WithMessage("Asset ID must be greater than 0.")
                .NotNull().WithMessage("Asset ID is required.");                
        }
    }
}