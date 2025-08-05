using InvestmentApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentApp.Infrastructure.Persistence.EntityConfigurations
{
    public class AssetHistoryEntityConfigurationEntityConfiguration : IEntityTypeConfiguration<AssetHistory>
    {
        public void Configure(EntityTypeBuilder<AssetHistory> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("AssetHistorical");
            #endregion

            #region Property configurations
            builder.Property(u => u.Value).IsRequired().HasDefaultValue(0);
            #endregion

            #region relationships
            #endregion
        }
    }
}
