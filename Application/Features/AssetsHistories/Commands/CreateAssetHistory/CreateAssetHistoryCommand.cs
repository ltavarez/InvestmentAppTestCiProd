using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace InvestmentApp.Core.Application.Features.AssetsHistories.Commands.CreateAssetHistory
{
    /// <summary>
    /// Parameters required to create a new asset history record
    /// </summary>
    public class CreateAssetHistoryCommand : IRequest<int>
    {
        /// <example>2024-12-31</example>
        [SwaggerParameter(Description = "The date for which the asset value is recorded")]
        public DateTime HistoryValueDate { get; set; }

        /// <example>45320.75</example>
        [SwaggerParameter(Description = "The value of the asset on the specified date")]
        public decimal Value { get; set; }

        /// <example>2</example>
        [SwaggerParameter(Description = "The ID of the asset to which this history belongs")]
        public int AssetId { get; set; }
    }

    public class CreateAssetHistoryCommandHandler : IRequestHandler<CreateAssetHistoryCommand, int>
    {
        private readonly IAssetHistoryRepository _assetHistoryRepository;       

        public CreateAssetHistoryCommandHandler(IAssetHistoryRepository assetHistoryRepository)
        {
            _assetHistoryRepository = assetHistoryRepository;
        }
        public async Task<int> Handle(CreateAssetHistoryCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.AssetHistory entity = new()
            {
                Id = 0,
                Value = command.Value,
                AssetId = command.AssetId,
                HistoryValueDate = command.HistoryValueDate                
            };

            Domain.Entities.AssetHistory? result = await _assetHistoryRepository.AddAsync(entity);

            return result != null ? result.Id : 0;
        }
    }
}
