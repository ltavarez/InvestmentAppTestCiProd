using InvestmentApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentApp.Infrastructure.Persistence.EntityConfigurations
{
    public class InvestmentPortfolioEntityConfiguration : IEntityTypeConfiguration<InvestmentPortfolio>
    {
        public void Configure(EntityTypeBuilder<InvestmentPortfolio> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("InvestmentPortfolios");
            #endregion

            #region Property configurations
            builder.Property(u => u.Name).IsRequired().HasMaxLength(255);
            #endregion

            #region relationships
            #endregion
        }
    }
}
