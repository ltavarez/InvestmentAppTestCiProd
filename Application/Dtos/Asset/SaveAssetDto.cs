namespace InvestmentApp.Core.Application.Dtos.Asset
{
    public class SaveAssetDto 
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Symbol { get; set; }
        public required int AssetTypeId { get; set; }        
    }
}
