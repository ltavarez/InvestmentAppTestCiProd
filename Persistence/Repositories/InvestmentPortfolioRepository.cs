using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Core.Domain.Interfaces;
using InvestmentApp.Infrastructure.Persistence.Contexts;

namespace InvestmentApp.Infrastructure.Persistence.Repositories
{
    public class InvestmentPortfolioRepository : GenericRepository<InvestmentPortfolio>, IInvestmentPortfolioRepository
    {      
        public InvestmentPortfolioRepository(InvestmentAppContext context) : base(context)
        {            
        }
    }
}