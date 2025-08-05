namespace InvestmentApp.Core.Domain.Entities
{
    public class AssetHistory
    {
        public required int Id { get; set; }
        public DateTime HistoryValueDate { get; set; }
        public required decimal Value { get; set; }
        public required int AssetId { get; set; } // FK

        //Navigation property
        public Asset? Asset { get; set; }
    }
}
