using FluentValidation;

namespace InvestmentApp.Core.Application.Features.AssetType.Queries.GetById
{
    public class GetByIdAssetTypeQueryValidator : AbstractValidator<GetByIdAssetTypeQuery>
    {
        public GetByIdAssetTypeQueryValidator()
        {
            RuleFor(at => at.Id)
                .NotNull().WithMessage("The asset type ID must be provided.")
                .GreaterThan(0).WithMessage("The asset type ID must be greater than 0.");
        }
    }
}
