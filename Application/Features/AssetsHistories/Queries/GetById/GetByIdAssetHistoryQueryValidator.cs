using FluentValidation;

namespace InvestmentApp.Core.Application.Features.AssetsHistories.Queries.GetById
{
    public class GetByIdAssetHistoryQueryValidator : AbstractValidator<GetByIdAssetHistoryQuery>
    {     
        public GetByIdAssetHistoryQueryValidator()
        {
            RuleFor(a => a.Id)
                .GreaterThan(0).WithMessage("Asset history ID must be greater than 0.")
                .NotNull().WithMessage("Asset history ID is required.");    
        }
    }
}
