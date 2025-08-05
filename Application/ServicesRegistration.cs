using FluentValidation;
using InvestmentApp.Core.Application.Behaviors;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace InvestmentApp.Core.Application
{
    public static class ServicesRegistration
    {
        //Extension method - Decorator pattern
        public static void AddApplicationLayerIoc(this IServiceCollection services)
        {
            #region Configurations
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(opt=> opt.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            #endregion
            #region Services IOC
            services.AddScoped<IAssetService, AssetService>();
            services.AddScoped<IAssetTypeService, AssetTypeService>(); 
            services.AddScoped<IInvestmentPortfolioService, InvestmentPortfolioService>();
            services.AddScoped<IAssetHistoryService, AssetHistoryService>();
            services.AddScoped<IInvestmentAssetsService, InvestmentAssetsService>();
            #endregion
        }
    }
}
