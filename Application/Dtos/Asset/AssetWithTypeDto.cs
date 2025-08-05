using InvestmentApp.Core.Application.Dtos.AssetType;

namespace InvestmentApp.Core.Application.Dtos.Asset
{
    public class AssetWithTypeDto : BasicDto<int>
    {
        public required string Symbol { get; set; }
        public required int AssetTypeId { get; set; }
        public AssetTypeDto? AssetType { get; set; }
    }
}
