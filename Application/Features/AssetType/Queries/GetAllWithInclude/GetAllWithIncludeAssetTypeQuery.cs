using AutoMapper;
using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.Dtos.AssetType;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Features.AssetType.Queries.GetAllWithInclude
{
    /// <summary>
    /// Query parameters for retrieving all asset types with related entities included
    /// </summary>
    public class GetAllWithIncludeAssetTypeQuery : IRequest<IList<AssetTypeResponseDto>>
    {
    }
    public class GetAllWithIncludeAssetTypeQueryHandler : IRequestHandler<GetAllWithIncludeAssetTypeQuery, IList<AssetTypeResponseDto>>
    {
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IMapper _mapper;

        public GetAllWithIncludeAssetTypeQueryHandler(IAssetTypeRepository assetTypeRepository, IMapper mapper)
        {
            _assetTypeRepository = assetTypeRepository;
            _mapper = mapper;
        }

        public async Task<IList<AssetTypeResponseDto>> Handle(GetAllWithIncludeAssetTypeQuery query, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _assetTypeRepository.GetAllQueryWithInclude(["Assets"]);

            var listEntityDtos = await listEntitiesQuery.Select(s => new AssetTypeResponseDto()
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Assets = s.Assets != null && s.Assets.Count > 0
                ? _mapper.Map<List<AssetBasicDto>>(s.Assets)
                : new List<AssetBasicDto>()

            }).ToListAsync();

            return listEntityDtos;
        }
    }
}
