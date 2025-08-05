using InvestmentApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentApp.Infrastructure.Persistence.EntityConfigurations
{
    public class AssetTypeEntityConfiguration : IEntityTypeConfiguration<AssetType>
    {
        public void Configure(EntityTypeBuilder<AssetType> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("AssetTypes");
            #endregion

            #region Property configurations
            builder.Property(u => u.Name).IsRequired().HasMaxLength(255);
            #endregion

            #region relationships

            builder.HasMany<Asset>(u => u.Assets)
                .WithOne(a => a.AssetType)
                .HasForeignKey(a => a.AssetTypeId)
                .OnDelete(DeleteBehavior.Cascade);//lambda
            #endregion
        }
    }
}
