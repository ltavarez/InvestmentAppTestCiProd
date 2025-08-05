using InvestmentApp.Core.Application.Dtos.AssetHistory;
using InvestmentApp.Core.Application.Dtos.AssetType;

namespace InvestmentApp.Core.Application.Dtos.Asset
{
    public class AssetDto : BasicDto<int>
    {
        public required string Symbol { get; set; }
        public required int AssetTypeId { get; set; }
        public AssetTypeDto? AssetType { get; set; }
        public ICollection<AssetHistoryDto>? AssetHistories { get; set; }
    }
}
