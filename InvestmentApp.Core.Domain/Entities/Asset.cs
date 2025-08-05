using InvestmentApp.Core.Domain.Common;

namespace InvestmentApp.Core.Domain.Entities
{
    public class Asset : BasicEntity<int>
    {
        public required string Symbol { get; set; }
        public required int AssetTypeId { get; set; } // FK
                                                      
        //navigation property
        public AssetType? AssetType { get; set; }

        public ICollection<AssetHistory>? AssetHistories { get; set; }
        public ICollection<InvestmentAssets>? InvestmentAssets { get; set; }
    }
}
