using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Core.Domain.Interfaces;
using InvestmentApp.Infrastructure.Persistence.Contexts;

namespace InvestmentApp.Infrastructure.Persistence.Repositories
{
    public class AssetTypeRepository : GenericRepository<AssetType>, IAssetTypeRepository
    {
        public AssetTypeRepository(InvestmentAppContext context) : base(context)
        {            
        }
    }
}