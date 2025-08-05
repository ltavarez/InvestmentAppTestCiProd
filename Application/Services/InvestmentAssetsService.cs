using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestmentApp.Core.Application.Dtos.InvestmentAssets;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Services
{
    public class InvestmentAssetsService : GenericService<InvestmentAssets, InvestmentAssetsDto>, IInvestmentAssetsService
    {
        private readonly IInvestmentAssetRepository _investmentAssetRepository;
        private readonly IMapper _mapper;

        public InvestmentAssetsService(IInvestmentAssetRepository investmentAssetRepository, IMapper mapper) : base(investmentAssetRepository, mapper)
        {
            _investmentAssetRepository = investmentAssetRepository;
            _mapper = mapper;
        }
        public async Task<InvestmentAssetsDto?> GetByAssetAndPortfolioAsync(int assetId, int portfolioId, string userId)
        {
            try
            {
                var investmentAsset = await _investmentAssetRepository
                    .GetAllQueryWithInclude(["Asset"])
                    .FirstOrDefaultAsync(ia => ia.AssetId == assetId
                    && ia.InvestmentPortfolioId == portfolioId
                     && ia.InvestmentPortfolio != null
                && ia.InvestmentPortfolio.UserId == userId);

                if (investmentAsset == null)
                {
                    return null;
                }

                InvestmentAssetsDto dto = _mapper.Map<InvestmentAssetsDto>(investmentAsset);

                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<InvestmentAssetsDto?> GetById(int id, string userId)
        {
            try
            {
                var listEntitiesQuery = _investmentAssetRepository.GetAllQueryWithInclude(["Asset", "InvestmentPortfolio"]);

                var entity = await listEntitiesQuery.FirstOrDefaultAsync(
                fd => fd.Id == id
                && fd.InvestmentPortfolio != null
                && fd.InvestmentPortfolio.UserId == userId);

                if (entity == null)
                {
                    return null;
                }

                var dto = _mapper.Map<InvestmentAssetsDto>(entity);

                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<InvestmentAssetsDto>> GetAllWithInclude(string userId)
        {
            try
            {
                var listEntitiesQuery = _investmentAssetRepository.GetAllQueryWithInclude(["Asset", "InvestmentPortfolio"]);
                listEntitiesQuery = listEntitiesQuery
                        .Where(w => w.InvestmentPortfolio != null && w.InvestmentPortfolio.UserId == userId);

                var listEntityDtos = await listEntitiesQuery.ProjectTo<InvestmentAssetsDto>(_mapper.ConfigurationProvider).ToListAsync();

                return listEntityDtos;
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}