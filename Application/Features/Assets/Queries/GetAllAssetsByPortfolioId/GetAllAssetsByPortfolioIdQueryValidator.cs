using FluentValidation;
using InvestmentApp.Core.Domain.Interfaces;

namespace InvestmentApp.Core.Application.Features.Assets.Queries.GetAllAssetsByPortfolioId
{
    public class GetAllAssetsByPortfolioIdQueryValidator : AbstractValidator<GetAllAssetsByPortfolioIdQuery>
    {
        private readonly IInvestmentPortfolioRepository _investmentPortfolioRepository;   
        public GetAllAssetsByPortfolioIdQueryValidator(IInvestmentPortfolioRepository investmentPortfolioRepository)
        {
            _investmentPortfolioRepository = investmentPortfolioRepository;

            RuleFor(a => a.PortfolioId).NotNull().WithMessage("Portfolio ID is required.")
                .GreaterThan(0).WithMessage("Portfolio ID must be greater than 0.")
                .MustAsync(async (id, cancellation) => 
                {
                    // Assuming you have a method to check if the Portfolio exists
                    var portfolioExists = await _investmentPortfolioRepository.GetById(id);
                    return portfolioExists != null;  
                }).WithMessage("Portfolio does not exist.");
        }
    }
}