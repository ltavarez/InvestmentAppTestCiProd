using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace InvestmentApp.Core.Application.Features.AssetType.Commands.CreateAssetType
{
    /// <summary>
    /// Parameters for creating a asset type
    /// </summary>
    public class CreateAssetTypeCommand : IRequest<int>
    {
        /// <example>Criptomoneda</example>
        [SwaggerParameter(Description = "The asset type name")]
        public string? Name { get; set; }

        [SwaggerParameter(Description = "A description of the asset type")]
        public string? Description { get; set; }
    }

    public class CreateAssetTypeCommandHandler : IRequestHandler<CreateAssetTypeCommand, int>
    {
        private readonly IAssetTypeRepository _assetTypeRepository;

        public CreateAssetTypeCommandHandler(IAssetTypeRepository assetTypeRepository)
        {
            _assetTypeRepository = assetTypeRepository;
        }

        public async Task<int> Handle(CreateAssetTypeCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.AssetType entity = new()
            {
                Id = 0,
                Name = command.Name ?? "",
                Description = command.Description,
            };

            Domain.Entities.AssetType? result = await _assetTypeRepository.AddAsync(entity);

            return result == null ? throw new ApiException("Error created assets type", (int)HttpStatusCode.InternalServerError) : result.Id;
        }
    }
}
