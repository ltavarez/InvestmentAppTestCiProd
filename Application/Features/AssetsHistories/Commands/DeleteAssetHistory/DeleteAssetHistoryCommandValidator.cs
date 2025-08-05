using FluentValidation;

namespace InvestmentApp.Core.Application.Features.AssetsHistories.Commands.DeleteAssetHistory
{
    public class DeleteAssetHistoryCommandValidator : AbstractValidator<DeleteAssetHistoryCommand>
    {     
        public DeleteAssetHistoryCommandValidator()
        {
            RuleFor(a => a.Id)
                .GreaterThan(0).WithMessage("Asset history ID must be greater than 0.")
                .NotNull().WithMessage("Asset history ID is required.");                
        }
    }
}
