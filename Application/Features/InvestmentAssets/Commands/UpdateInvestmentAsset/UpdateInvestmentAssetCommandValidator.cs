using FluentValidation;
using InvestmentApp.Core.Domain.Interfaces;

namespace InvestmentApp.Core.Application.Features.InvestmentAssets.Commands.UpdateInvestmentAsset
{
    public class UpdateInvestmentAssetCommandValidator : AbstractValidator<UpdateInvestmentAssetCommand>
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IInvestmentPortfolioRepository _investmentPortfolioRepository;
        public UpdateInvestmentAssetCommandValidator(IAssetRepository assetRepository, IInvestmentPortfolioRepository investmentPortfolioRepository)
        {
            _assetRepository = assetRepository;
            _investmentPortfolioRepository = investmentPortfolioRepository;

            RuleFor(a => a.Id).NotNull().WithMessage("Investment Asset ID is required.")
                .GreaterThan(0).WithMessage("Investment Asset ID must be greater than 0.");         

            RuleFor(a => a.AssociationDate)
                .NotEmpty().WithMessage("Association date is required.")
                .Must(date => date.Kind == DateTimeKind.Utc).WithMessage("Association date must be in UTC format.");

            RuleFor(a => a.InvestmentPortfolioId)
                .NotNull().WithMessage("Investment Portfolio ID is required.")
                .GreaterThan(0).WithMessage("Investment Portfolio ID must be greater than 0.")
                .MustAsync(async (id, cancellation) => 
                {
                    // Check if the Investment Portfolio exists
                    var portfolioExists = await _investmentPortfolioRepository.GetById(id);
                    return portfolioExists != null;  
                }).WithMessage("Investment Portfolio does not exist.");

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
