using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using System.Net;

namespace InvestmentApp.Core.Application.Features.Assets.Commands.DeleteAsset
{
    public class DeleteAssetCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }

    public class DeleteAssetCommandHandler : IRequestHandler<DeleteAssetCommand, Unit>
    {
        private readonly IAssetRepository _assetRepository;       

        public DeleteAssetCommandHandler(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        public async Task<Unit> Handle(DeleteAssetCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Asset? entity = await _assetRepository.GetById(command.Id);
            if (entity == null) throw new ApiException("Asset not found with this id", (int)HttpStatusCode.NotFound);

            await _assetRepository.DeleteAsync(command.Id);            

            return Unit.Value;
        }
    }
}
