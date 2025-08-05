using InvestmentApp.Core.Domain.Interfaces;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InvestmentApp.Infrastructure.Persistence
{
    public static class ServicesRegistration
    {
        //Extension method - Decorator pattern
        public static void AddPersistenceLayerIoc(this IServiceCollection services, IConfiguration config)
        {
            #region Contexts
            if (config.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<InvestmentAppContext>(opt =>
                                              opt.UseInMemoryDatabase("AppDb"));
            }
            else
            {
                var connectionString = config.GetConnectionString("DefaultConnection");
                services.AddDbContext<InvestmentAppContext>(
                  (serviceProvider, opt) =>
                    {
                        opt.EnableSensitiveDataLogging();
                        opt.UseSqlServer(connectionString,
                        m => m.MigrationsAssembly(typeof(InvestmentAppContext).Assembly.FullName));
                    },
                    contextLifetime: ServiceLifetime.Scoped,
                    optionsLifetime: ServiceLifetime.Scoped
                 );

                #endregion

                #region Repositories IOC
                services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
                services.AddScoped<IAssetRepository, AssetRepository>();
                services.AddScoped<IAssetTypeRepository, AssetTypeRepository>();  
                services.AddScoped<IInvestmentPortfolioRepository, InvestmentPortfolioRepository>();
                services.AddScoped<IInvestmentAssetRepository, InvestmentAssetRepository>();
                services.AddScoped<IAssetHistoryRepository, AssetHistoryRepository>();
                #endregion
            }
        }
    }
}
