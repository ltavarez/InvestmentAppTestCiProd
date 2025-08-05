using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestmentApp.Core.Application.Dtos.AssetType;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Services
{
    public class AssetTypeService : GenericService<AssetType, AssetTypeDto>, IAssetTypeService
    {
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IMapper _mapper;
        public AssetTypeService(IAssetTypeRepository assetTypeRepository, IMapper mapper) : base(assetTypeRepository, mapper)
        {
            _assetTypeRepository = assetTypeRepository;
            _mapper = mapper;
        }
        public async Task<List<AssetTypeDto>> GetAllWithInclude()
        {
            try
            {
                var listEntitiesQuery = _assetTypeRepository.GetAllQueryWithInclude(["Assets"]);

                var listEntityDtos = await listEntitiesQuery.ProjectTo<AssetTypeDto>(_mapper.ConfigurationProvider).ToListAsync();

                return listEntityDtos;
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}
