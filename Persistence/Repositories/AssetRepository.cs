using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Core.Domain.Interfaces;
using InvestmentApp.Infrastructure.Persistence.Contexts;

namespace InvestmentApp.Infrastructure.Persistence.Repositories
{
    public class AssetRepository : GenericRepository<Asset>, IAssetRepository
    {      
        public AssetRepository(InvestmentAppContext context) : base(context)
        {            
        }
    }
}