using InvestmentApp.Core.Application.Dtos.Asset;

namespace InvestmentApp.Core.Application.Dtos.AssetType
{
    public class AssetTypeResponseDto : BasicDto<int>
    {
        public List<AssetBasicDto>? Assets { get; set; }
    }
}
