using InvestmentApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace InvestmentApp.Infrastructure.Persistence.Contexts
{
    public class InvestmentAppContext : DbContext
    {
        public InvestmentAppContext(DbContextOptions<InvestmentAppContext> options) : base(options) { }
        public DbSet<InvestmentPortfolio> InvestmentPortfolios { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<AssetHistory> AssetHistories { get; set; }
        public DbSet<InvestmentAssets> InvestmentAssets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); //Liskov-substitution

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
