using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Domain.Common.Enums;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Services
{
    public class AssetService : GenericService<Asset, AssetDto>, IAssetService
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IInvestmentAssetRepository _investmentAssetRepository;
        private readonly IMapper _mapper;
        public AssetService(IAssetRepository assetRepository, IInvestmentAssetRepository investmentAssetRepository, IMapper mapper) : base(assetRepository, mapper)
        {
            _assetRepository = assetRepository;
            _investmentAssetRepository = investmentAssetRepository;
            _mapper = mapper;
        }

        public override async Task<AssetDto?> GetById(int id)
        {
            try
            {
                var listEntitiesQuery = _assetRepository.GetAllQueryWithInclude(["AssetType"]);

                var entity = await listEntitiesQuery.FirstOrDefaultAsync(a => a.Id == id);

                if (entity == null)
                {
                    return null;
                }

                var dto = _mapper.Map<AssetDto>(entity);

                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<AssetDto>> GetAllWithInclude()
        {
            try
            {
                var listEntitiesQuery = _assetRepository.GetAllQueryWithInclude(["AssetType", "AssetHistories"]);

                var listEntityDtos = await listEntitiesQuery.ProjectTo<AssetDto>(_mapper.ConfigurationProvider).ToListAsync();

                return listEntityDtos;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<List<AssetForPortfolioDto>> GetAllAssetsByPortfolioId(int portfolioId,
            string? assetName = null, int? assetTypeId = null, int? assetOrderBy = null)
        {
            try
            {
                var assetIds = await _investmentAssetRepository
                    .GetAllQuery()
                    .Where(ia => ia.InvestmentPortfolioId == portfolioId)
                    .Select(s => s.AssetId).ToListAsync();

                if (assetIds.Count == 0)
                {
                    return [];
                }

                var listEntitiesQuery = _assetRepository
                    .GetAllQueryWithInclude(["AssetType", "AssetHistories"])
                    .Where(w => assetIds.Contains(w.Id));

                var listEntityDtos = listEntitiesQuery.ProjectTo<AssetForPortfolioDto>(_mapper.ConfigurationProvider);

                if (!string.IsNullOrWhiteSpace(assetName))
                {
                    listEntityDtos = listEntityDtos.Where(w => w.Name.Contains(assetName));
                }

                if (assetTypeId.HasValue)
                {
                    listEntityDtos = listEntityDtos.Where(w => w.AssetTypeId == assetTypeId);
                }

                var listDtos = await listEntityDtos.ToListAsync();

                if (assetOrderBy.HasValue)
                {
                    var listOrderDtos = assetOrderBy switch
                    {
                        (int)AssetOrdered.BY_NAME => listDtos.OrderBy(o => o.Name),
                        (int)AssetOrdered.BY_CURRENT_VALUE => listDtos.OrderByDescending(o => o.CurrentValue),
                        _ => listDtos.OrderBy(o => o.Name),
                    };

                    listDtos = listOrderDtos.ToList();
                }
                else
                {
                    listDtos = listDtos.OrderBy(o => o.Name).ToList();
                }

                return listDtos;
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}