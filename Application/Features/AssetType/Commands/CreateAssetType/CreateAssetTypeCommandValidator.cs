using FluentValidation;

namespace InvestmentApp.Core.Application.Features.AssetType.Commands.CreateAssetType
{
    public class CreateAssetTypeCommandValidator : AbstractValidator<CreateAssetTypeCommand>
    {
        public CreateAssetTypeCommandValidator()
        {
            RuleFor(at=> at.Name)
                .NotEmpty().WithMessage("Asset type name is required 2.")
                .MaximumLength(150).WithMessage("Asset type name must not exceed 150 characters.");

            RuleFor(at => at.Description)
                .MaximumLength(250).WithMessage("Asset type description must not exceed 250 characters.");

        }
    }
}
