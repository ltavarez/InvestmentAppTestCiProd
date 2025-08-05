using InvestmentApp.Core.Domain.Common;

namespace InvestmentApp.Core.Domain.Entities
{
    public class AssetType : BasicEntity<int>
    {
        //navigation property
        public ICollection<Asset>? Assets { get; set; }
    }
}
