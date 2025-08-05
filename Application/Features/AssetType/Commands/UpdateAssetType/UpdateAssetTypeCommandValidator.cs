using FluentValidation;

namespace InvestmentApp.Core.Application.Features.AssetType.Commands.UpdateAssetType
{
    public class UpdateAssetTypeCommandValidator : AbstractValidator<UpdateAssetTypeCommand>
    {
        public UpdateAssetTypeCommandValidator()
        {
            RuleFor(at => at.Id)
                .NotNull().WithMessage("The asset type ID must be provided.")
                .GreaterThan(0).WithMessage("The asset type ID must be greater than 0.");

            RuleFor(at=> at.Name)
                .NotEmpty().WithMessage("Asset type name is required 2.")
                .MaximumLength(150).WithMessage("Asset type name must not exceed 150 characters.");

            RuleFor(at => at.Description)
                .MaximumLength(250).WithMessage("Asset type description must not exceed 250 characters.");

        }
    }
}
