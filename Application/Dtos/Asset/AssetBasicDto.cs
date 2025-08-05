namespace InvestmentApp.Core.Application.Dtos.Asset
{
    public class AssetBasicDto : BasicDto<int>
    {
        public required string Symbol { get; set; }
        public required int AssetTypeId { get; set; }      
    }
}
