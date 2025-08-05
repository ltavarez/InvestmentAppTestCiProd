using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace InvestmentApp.Core.Application.Features.InvestmentAssets.Commands.CreateInvestmentAsset
{
    /// <summary>
    /// Parameters required to create a new investment asset associated with a portfolio
    /// </summary>
    public class CreateInvestmentAssetCommand : IRequest<int>
    {
        /// <example>3</example>
        [SwaggerParameter(Description = "The ID of the asset to associate with a portfolio (foreign key)")]
        public required int AssetId { get; set; }

        /// <example>5</example>
        [SwaggerParameter(Description = "The ID of the investment portfolio to link the asset to (foreign key)")]
        public required int InvestmentPortfolioId { get; set; }

        /// <example>2025-01-01T00:00:00Z</example>
        [SwaggerParameter(Description = "The date when the asset was associated to the portfolio")]
        public DateTime AssociationDate { get; set; } = DateTime.UtcNow;
    }

    public class CreateInvestmentAssetCommandHandler : IRequestHandler<CreateInvestmentAssetCommand, int>
    {
        private readonly IInvestmentAssetRepository _investmentAssetRepository;

        public CreateInvestmentAssetCommandHandler(IInvestmentAssetRepository investmentAssetRepository)
        {
            _investmentAssetRepository = investmentAssetRepository;
        }
        public async Task<int> Handle(CreateInvestmentAssetCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.InvestmentAssets entity = new()
            {
                Id = 0,
                AssetId = command.AssetId,
                InvestmentPortfolioId = command.InvestmentPortfolioId,
                AssociationDate = command.AssociationDate
            };

            Domain.Entities.InvestmentAssets? result = await _investmentAssetRepository.AddAsync(entity);

            return result != null ? result.Id : 0;
        }
    }
}
