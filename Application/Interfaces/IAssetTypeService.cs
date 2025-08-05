using InvestmentApp.Core.Application.Dtos.AssetType;

namespace InvestmentApp.Core.Application.Interfaces
{
    public interface IAssetTypeService : IGenericService<AssetTypeDto>
    {
        Task<List<AssetTypeDto>> GetAllWithInclude();
    }
}