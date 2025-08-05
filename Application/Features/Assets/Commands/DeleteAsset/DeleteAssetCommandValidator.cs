using FluentValidation;

namespace InvestmentApp.Core.Application.Features.Assets.Commands.DeleteAsset
{
    public class DeleteAssetCommandValidator : AbstractValidator<DeleteAssetCommand>
    {    
        public DeleteAssetCommandValidator()
        {      
            RuleFor(a => a.Id)
                .NotNull().WithMessage("Asset ID is required.")
                .GreaterThan(0).WithMessage("Asset ID must be greater than 0.")
                .WithMessage("Asset ID is required.");
        }
    }
}
