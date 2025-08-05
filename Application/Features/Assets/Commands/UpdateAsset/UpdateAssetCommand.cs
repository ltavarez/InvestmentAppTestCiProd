using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace InvestmentApp.Core.Application.Features.Assets.Commands.UpdateAsset
{
    /// <summary>
    /// Parameters required to update an existing asset
    /// </summary>
    public class UpdateAssetCommand : IRequest<Unit>
    {
        /// <example>5</example>
        [SwaggerParameter(Description = "The unique identifier of the asset to update")]
        public int Id { get; set; }

        /// <example>Bitcoin</example>
        [SwaggerParameter(Description = "The updated name of the asset")]
        public string? Name { get; set; }

        /// <example>Criptomoneda descentralizada líder en el mercado</example>
        [SwaggerParameter(Description = "The updated description of the asset (optional)")]
        public string? Description { get; set; }

        /// <example>BTC</example>
        [SwaggerParameter(Description = "The updated trading symbol of the asset")]
        public string? Symbol { get; set; }

        /// <example>1</example>
        [SwaggerParameter(Description = "The ID of the associated asset type (foreign key)")]
        public int AssetTypeId { get; set; }
    }
    public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, Unit>
    {
        private readonly IAssetRepository _assetRepository;       

        public UpdateAssetCommandHandler(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        public async Task<Unit> Handle(UpdateAssetCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Asset entity = new()
            {
                Id = command.Id,
                Name = command.Name ?? "",
                Description = command.Description,
                AssetTypeId = command.AssetTypeId,
                Symbol = command.Symbol ?? ""
            };

            Domain.Entities.Asset? getEntity = await _assetRepository.GetById(command.Id);
            if (getEntity == null) throw new ApiException("Asset not found with this id", (int)HttpStatusCode.NotFound);

            await _assetRepository.UpdateAsync(command.Id, entity);

            return Unit.Value;
        }
    }
}
