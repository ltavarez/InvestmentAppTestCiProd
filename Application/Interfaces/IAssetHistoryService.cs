using InvestmentApp.Core.Application.Dtos.AssetHistory;

namespace InvestmentApp.Core.Application.Interfaces
{
    public interface IAssetHistoryService : IGenericService<AssetHistoryDto>
    {
        Task<List<AssetHistoryDto>> GetAllWithInclude();
    }
}