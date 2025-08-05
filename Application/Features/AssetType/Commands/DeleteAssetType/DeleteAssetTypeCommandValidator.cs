using FluentValidation;

namespace InvestmentApp.Core.Application.Features.AssetType.Commands.DeleteAssetType
{
    public class DeleteAssetTypeCommandValidator : AbstractValidator<DeleteAssetTypeCommand>
    {
        public DeleteAssetTypeCommandValidator()
        {
            RuleFor(at=> at.Id)
                .NotNull().WithMessage("The asset type ID must be provided.")
                .Must(id => id > 0).WithMessage("The asset type ID must be greather than 0.");
        }
    }
}
