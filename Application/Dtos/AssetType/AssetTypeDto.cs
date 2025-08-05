namespace InvestmentApp.Core.Application.Dtos.AssetType
{
    public class AssetTypeDto : BasicDto<int>
    {
        public int? AssetQuantity { get; set; }
    }
}
