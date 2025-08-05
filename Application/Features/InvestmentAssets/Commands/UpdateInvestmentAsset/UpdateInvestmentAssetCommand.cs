using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace InvestmentApp.Core.Application.Features.InvestmentAssets.Commands.UpdateInvestmentAsset
{
    /// <summary>
    /// Parameters required to update an existing investment asset associated with a portfolio
    /// </summary>
    public class UpdateInvestmentAssetCommand : IRequest<Unit>
    {
        /// <example>10</example>
        [SwaggerParameter(Description = "The unique identifier of the investment asset to update")]
        public int Id { get; set; }

        /// <example>3</example>
        [SwaggerParameter(Description = "The ID of the asset to associate (foreign key)")]
        public int AssetId { get; set; }

        /// <example>5</example>
        [SwaggerParameter(Description = "The ID of the investment portfolio to link the asset to (foreign key)")]
        public int InvestmentPortfolioId { get; set; }

        /// <example>2025-01-01T00:00:00Z</example>
        [SwaggerParameter(Description = "The date when the asset was (re)associated with the portfolio")]
        public DateTime AssociationDate { get; set; } = DateTime.UtcNow;
    }

    public class UpdateInvestmentAssetCommandHandler : IRequestHandler<UpdateInvestmentAssetCommand, Unit>
    {
        private readonly IInvestmentAssetRepository _investmentAssetRepository;

        public UpdateInvestmentAssetCommandHandler(IInvestmentAssetRepository investmentAssetRepository)
        {
            _investmentAssetRepository = investmentAssetRepository;
        }
        public async Task<Unit> Handle(UpdateInvestmentAssetCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.InvestmentAssets entity = new()
            {
                Id = command.Id,
                AssetId = command.AssetId,
                InvestmentPortfolioId = command.InvestmentPortfolioId,
                AssociationDate = command.AssociationDate
            };

            Domain.Entities.InvestmentAssets? getEntity = await _investmentAssetRepository.GetById(command.Id);
            if (getEntity == null) throw new ApiException("Investment Assets not found with this id", (int)HttpStatusCode.NotFound);

            await _investmentAssetRepository.UpdateAsync(command.Id, entity);

            return Unit.Value;
        }
    }
}
