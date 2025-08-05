using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace InvestmentApp.Core.Application.Features.AssetType.Commands.DeleteAssetType
{
    /// <summary>
    /// Parameters for deleting an asset type
    /// </summary>
    public class DeleteAssetTypeCommand : IRequest<Unit>
    {
        /// <example>5</example>
        [SwaggerParameter(Description = "The unique identifier of the asset type to delete")]
        public int Id { get; set; }
    }

    public class DeleteAssetTypeCommandHandler : IRequestHandler<DeleteAssetTypeCommand, Unit>
    {
        private readonly IAssetTypeRepository _assetTypeRepository;       

        public DeleteAssetTypeCommandHandler(IAssetTypeRepository assetTypeRepository)
        {
            _assetTypeRepository = assetTypeRepository;
        }

        public async Task<Unit> Handle(DeleteAssetTypeCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.AssetType? entity = await _assetTypeRepository.GetById(command.Id);
            if (entity == null) throw new ApiException("Asset type not found with this id", (int)HttpStatusCode.NotFound);

            await _assetTypeRepository.DeleteAsync(command.Id);            

            return Unit.Value;
        }
    }
}
