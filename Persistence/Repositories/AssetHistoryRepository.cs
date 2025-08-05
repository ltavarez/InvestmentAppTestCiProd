using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Core.Domain.Interfaces;
using InvestmentApp.Infrastructure.Persistence.Contexts;

namespace InvestmentApp.Infrastructure.Persistence.Repositories
{
    public class AssetHistoryRepository : GenericRepository<AssetHistory>, IAssetHistoryRepository
    {      
        public AssetHistoryRepository(InvestmentAppContext context) : base(context)
        {            
        }
    }
}