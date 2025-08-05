using InvestmentApp.Core.Application.ViewModels.AssetHistory;
using InvestmentApp.Core.Application.ViewModels.AssetType;

namespace InvestmentApp.Core.Application.ViewModels.Asset
{
    public class AssetViewModel : BasicViewModel<int>
    {
        public required string Symbol { get; set; }
        public required int AssetTypeId { get; set; }
        public AssetTypeViewModel? AssetType { get; set; }
        public ICollection<AssetHistoryViewModel>? AssetHistories { get; set; }
    }
}
