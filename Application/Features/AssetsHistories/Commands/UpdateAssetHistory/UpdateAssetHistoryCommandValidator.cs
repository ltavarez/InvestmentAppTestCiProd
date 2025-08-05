using FluentValidation;
using InvestmentApp.Core.Domain.Interfaces;

namespace InvestmentApp.Core.Application.Features.AssetsHistories.Commands.UpdateAssetHistory
{
    public class UpdateAssetHistoryCommandValidator : AbstractValidator<UpdateAssetHistoryCommand>
    {
        private readonly IAssetRepository _assetRepository;   
        public UpdateAssetHistoryCommandValidator(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;

            RuleFor(a => a.Id)
                .GreaterThan(0).WithMessage("Asset history ID must be greater than 0.")
                .NotNull().WithMessage("Asset history ID is required.");           

            RuleFor(a => a.HistoryValueDate)
                .NotEmpty().WithMessage("History value date is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("History value date cannot be in the future.");

            RuleFor(a => a.Value)
                .GreaterThan(0).WithMessage("Asset value must be greater than 0.")
                .WithMessage("Asset value is required.");

            RuleFor(a => a.AssetId)
                .GreaterThan(0).WithMessage("Asset ID must be greater than 0.")
                .WithMessage("Asset ID is required.")
                .MustAsync(async (id, cancellation) => 
                {
                    // Check if the Asset exists
                    var assetExists = await _assetRepository.GetById(id);
                    return assetExists != null;  
                }).WithMessage("Asset does not exist.");
        }
    }
}
