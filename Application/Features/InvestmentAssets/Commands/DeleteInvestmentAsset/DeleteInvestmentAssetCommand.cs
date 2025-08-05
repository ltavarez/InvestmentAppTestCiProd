using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using System.Net;

namespace InvestmentApp.Core.Application.Features.InvestmentAssets.Commands.DeleteInvestmentAsset
{
    public class DeleteInvestmentAssetCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
    }
    public class DeleteInvestmentAssetCommandHandler : IRequestHandler<DeleteInvestmentAssetCommand, Unit>
    {
        private readonly IInvestmentAssetRepository _investmentAssetRepository;

        public DeleteInvestmentAssetCommandHandler(IInvestmentAssetRepository investmentAssetRepository)
        {
            _investmentAssetRepository = investmentAssetRepository;
        }
        public async Task<Unit> Handle(DeleteInvestmentAssetCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.InvestmentAssets? entity = await _investmentAssetRepository.GetById(command.Id);
            if (entity == null) throw new ApiException("Investment Asset not found with this id", (int)HttpStatusCode.NotFound);

            await _investmentAssetRepository.DeleteAsync(command.Id);            

            return Unit.Value;
        }
    }
}
