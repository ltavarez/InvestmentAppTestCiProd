using InvestmentApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentApp.Infrastructure.Persistence.EntityConfigurations
{
    public class AssetEntityConfiguration : IEntityTypeConfiguration<Asset>
    {
        public void Configure(EntityTypeBuilder<Asset> builder)
        {
            //Fluent api
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("Assets");
            #endregion

            #region Property configurations
            builder.Property(u => u.Name).IsRequired().HasMaxLength(255);
            builder.Property(u => u.Symbol).IsRequired().HasMaxLength(20);
            #endregion

            #region relationships

            builder.HasMany<AssetHistory>(a => a.AssetHistories)
                .WithOne(ah => ah.Asset)
                .HasForeignKey(ah => ah.AssetId)
                .OnDelete(DeleteBehavior.Cascade);//lambda
            #endregion
        }
    }
}
