using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestmentApp.Core.Application.Dtos.AssetHistory;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Services
{
    public class AssetHistoryService : GenericService<AssetHistory, AssetHistoryDto>, IAssetHistoryService
    {
        private readonly IAssetHistoryRepository _assetHistoryRepository;
        private readonly IMapper _mapper;
        public AssetHistoryService(IAssetHistoryRepository assetHistoryRepository, IMapper mapper) : base(assetHistoryRepository, mapper)
        {
            _assetHistoryRepository = assetHistoryRepository;
            _mapper = mapper;
        }

        public override async Task<AssetHistoryDto?> UpdateAsync(AssetHistoryDto dto, int id)
        {
            try
            {
                var entity = await GetById(id);
                if (entity == null)
                {
                    return null;
                }

                dto.HistoryValueDate = entity.HistoryValueDate;
                return await base.UpdateAsync(dto, id);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public override async Task<AssetHistoryDto?> GetById(int id)
        {
            try
            {
                var listEntitiesQuery = _assetHistoryRepository.GetAllQueryWithInclude(["Asset"]);

                var entity = await listEntitiesQuery.FirstOrDefaultAsync(a => a.Id == id);

                if (entity == null)
                {
                    return null;
                }

                var dto = _mapper.Map<AssetHistoryDto>(entity);

                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<AssetHistoryDto>> GetAllWithInclude()
        {
            try
            {
                var listEntitiesQuery = _assetHistoryRepository.GetAllQueryWithInclude(["Asset"]);

                var listEntityDtos = await listEntitiesQuery.ProjectTo<AssetHistoryDto>(_mapper.ConfigurationProvider).ToListAsync();

                return listEntityDtos;
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}