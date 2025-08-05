using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace InvestmentApp.Core.Application.Features.AssetsHistories.Commands.UpdateAssetHistory
{
    /// <summary>
    /// Parameters required to update an existing asset history record
    /// </summary>
    public class UpdateAssetHistoryCommand : IRequest<Unit>
    {
        /// <example>10</example>
        [SwaggerParameter(Description = "The unique identifier of the asset history record to update")]
        public int Id { get; set; }

        /// <example>2024-12-31</example>
        [SwaggerParameter(Description = "The date associated with the asset value")]
        public DateTime HistoryValueDate { get; set; }

        /// <example>47850.90</example>
        [SwaggerParameter(Description = "The updated value of the asset on the specified date")]
        public decimal Value { get; set; }

        /// <example>2</example>
        [SwaggerParameter(Description = "The ID of the asset related to this history record")]
        public int AssetId { get; set; }
    }

    public class UpdateAssetHistoryCommandHandler : IRequestHandler<UpdateAssetHistoryCommand, Unit>
    {
        private readonly IAssetHistoryRepository _assetHistoryRepository;

        public UpdateAssetHistoryCommandHandler(IAssetHistoryRepository assetHistoryRepository)
        {
            _assetHistoryRepository = assetHistoryRepository;
        }
        public async Task<Unit> Handle(UpdateAssetHistoryCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.AssetHistory entity = new()
            {
                Id = command.Id,
                AssetId = command.AssetId,
                Value = command.Value,
                HistoryValueDate = command.HistoryValueDate,
            };

            Domain.Entities.AssetHistory? getEntity = await _assetHistoryRepository.GetById(command.Id);
            if (getEntity == null) throw new ApiException("Asset history not found with this id", (int)HttpStatusCode.NotFound);

            await _assetHistoryRepository.UpdateAsync(command.Id, entity);

            return Unit.Value;
        }
    }
}
