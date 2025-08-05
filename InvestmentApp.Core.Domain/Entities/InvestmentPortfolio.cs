using InvestmentApp.Core.Domain.Common;

namespace InvestmentApp.Core.Domain.Entities
{
    public class InvestmentPortfolio : BasicEntity<int>
    {
        public required string UserId { get; set; } // FK
        //navigation property  
        public ICollection<InvestmentAssets>? InvestmentAssets { get; set; }
    }
}
