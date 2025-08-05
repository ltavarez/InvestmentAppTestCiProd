using FluentValidation;

namespace InvestmentApp.Core.Application.Features.InvestmentAssets.Commands.DeleteInvestmentAsset
{
    public class DeleteInvestmentAssetCommandValidator : AbstractValidator<DeleteInvestmentAssetCommand>
    {
        public DeleteInvestmentAssetCommandValidator()
        {
            RuleFor(a => a.Id).NotNull().WithMessage("Investment Asset ID is required.")
                .GreaterThan(0).WithMessage("Investment Asset ID must be greater than 0.");
        }
    }
}
