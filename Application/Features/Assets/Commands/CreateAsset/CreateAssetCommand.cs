using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace InvestmentApp.Core.Application.Features.Assets.Commands.CreateAsset
{
    /// <summary>
    /// Parameters required to create a new asset
    /// </summary>
    public class CreateAssetCommand : IRequest<int>
    {
        /// <example>Bitcoin</example>
        [SwaggerParameter(Description = "The name of the asset to be created")]
        public string? Name { get; set; }

        /// <example>Criptomoneda descentralizada líder en el mercado</example>
        [SwaggerParameter(Description = "A brief description of the asset (optional)")]
        public string? Description { get; set; }

        /// <example>BTC</example>
        [SwaggerParameter(Description = "The trading symbol or abbreviation of the asset")]
        public string? Symbol { get; set; }

        /// <example>1</example>
        [SwaggerParameter(Description = "The ID of the asset type (foreign key reference)")]
        public int AssetTypeId { get; set; }
    }

    public class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, int>
    {
        private readonly IAssetRepository _assetRepository;       

        public CreateAssetCommandHandler(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        public async Task<int> Handle(CreateAssetCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Asset entity = new()
            {
                Id = 0,
                Name = command.Name ?? "",
                Description = command.Description,
                AssetTypeId = command.AssetTypeId,
                Symbol = command.Symbol ?? ""                
            };

            Domain.Entities.Asset? result = await _assetRepository.AddAsync(entity);

            return result == null ? throw new ApiException("Error created assets", (int)HttpStatusCode.InternalServerError) : result.Id;
        }
    }
}
