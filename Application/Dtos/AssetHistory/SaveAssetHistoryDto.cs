namespace InvestmentApp.Core.Application.Dtos.AssetHistory
{
    public class SaveAssetHistoryDto
    {
        public required int Id { get; set; }
        public DateTime HistoryValueDate { get; set; }
        public required decimal Value { get; set; }
        public required int AssetId { get; set; }
    }
}
