using InvestmentApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentApp.Infrastructure.Persistence.EntityConfigurations
{
    public class InvestmentAssetEntityConfiguration : IEntityTypeConfiguration<InvestmentAssets>
    {
        public void Configure(EntityTypeBuilder<InvestmentAssets> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("InvestmentAssets");
            #endregion

            #region Property configurations
            #endregion

            #region relationships

            builder.HasOne(ia => ia.Asset)
                .WithMany(a => a.InvestmentAssets)
                .HasForeignKey(ia => ia.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ia => ia.InvestmentPortfolio)
                    .WithMany(ip => ip.InvestmentAssets)
                    .HasForeignKey(ia => ia.InvestmentPortfolioId)
                    .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
