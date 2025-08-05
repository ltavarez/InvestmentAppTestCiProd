using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace InvestmentApp.Core.Application.Features.AssetType.Commands.UpdateAssetType
{
    /// <summary>
    /// Parameters for updating an existing asset type
    /// </summary>
    public class UpdateAssetTypeCommand : IRequest<Unit>
    {
        /// <example>5</example>
        [SwaggerParameter(Description = "The unique identifier of the asset type to update")]
        public int Id { get; set; }

        /// <example>Criptomoneda</example>
        [SwaggerParameter(Description = "The updated name of the asset type")]
        public string? Name { get; set; }

        /// <example>Activo digital como Bitcoin o Ethereum</example>
        [SwaggerParameter(Description = "The updated description of the asset type (optional)")]
        public string? Description { get; set; }
    }

    public class UpdateAssetTypeCommandHandler : IRequestHandler<UpdateAssetTypeCommand, Unit>
    {
        private readonly IAssetTypeRepository _assetTypeRepository;       

        public UpdateAssetTypeCommandHandler(IAssetTypeRepository assetTypeRepository)
        {
            _assetTypeRepository = assetTypeRepository;
        }

        public async Task<Unit> Handle(UpdateAssetTypeCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.AssetType entity = new()
            {
                Id = command.Id,
                Name = command.Name ?? "",
                Description = command.Description,
            };

            Domain.Entities.AssetType? getEntity = await _assetTypeRepository.GetById(command.Id);
            if (getEntity == null) throw new ApiException("Asset type not found with this id", (int)HttpStatusCode.NotFound);

            await _assetTypeRepository.UpdateAsync(command.Id, entity);

            return Unit.Value;
        }
    }
}
