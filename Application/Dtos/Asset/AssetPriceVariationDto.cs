namespace InvestmentApp.Core.Application.Dtos.Asset
{
    public class AssetPriceVariationDto
    {
        public AssetDto Asset { get; set; } = default!;
        public decimal TodayPrice { get; set; }
        public decimal YesterdayPrice { get; set; }
        public decimal Change { get; set; }
        public string Direction { get; set; } = string.Empty;
    }
}