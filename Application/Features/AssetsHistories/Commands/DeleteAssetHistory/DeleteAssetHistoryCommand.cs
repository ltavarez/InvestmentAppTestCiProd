using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using System.Net;

namespace InvestmentApp.Core.Application.Features.AssetsHistories.Commands.DeleteAssetHistory
{
    public class DeleteAssetHistoryCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
    public class DeleteAssetHistoryCommandHandler : IRequestHandler<DeleteAssetHistoryCommand, Unit>
    {
        private readonly IAssetHistoryRepository _assetHistoryRepository;

        public DeleteAssetHistoryCommandHandler(IAssetHistoryRepository assetHistoryRepository)
        {
            _assetHistoryRepository = assetHistoryRepository;
        }
        public async Task<Unit> Handle(DeleteAssetHistoryCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.AssetHistory? entity = await _assetHistoryRepository.GetById(command.Id);
            if (entity == null) throw new ApiException("Asset history not found with this id", (int)HttpStatusCode.NotFound);

            await _assetHistoryRepository.DeleteAsync(command.Id);            

            return Unit.Value;
        }
    }
}
