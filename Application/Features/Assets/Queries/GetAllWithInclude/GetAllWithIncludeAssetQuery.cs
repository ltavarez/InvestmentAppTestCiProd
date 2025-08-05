using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Features.Assets.Queries.GetAllWithInclude
{
    public class GetAllWithIncludeAssetQuery : IRequest<IList<AssetDto>>
    {       
    }
    public class GetAllWithIncludeAssetQueryHandler : IRequestHandler<GetAllWithIncludeAssetQuery, IList<AssetDto>>
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IMapper _mapper;

        public GetAllWithIncludeAssetQueryHandler(IAssetRepository assetRepository, IMapper mapper)
        {
            _assetRepository = assetRepository;
            _mapper = mapper;
        }

        public async Task<IList<AssetDto>> Handle(GetAllWithIncludeAssetQuery query, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _assetRepository.GetAllQueryWithInclude(["AssetType", "AssetHistories"]);

            var listEntityDtos = await listEntitiesQuery.ProjectTo<AssetDto>(_mapper.ConfigurationProvider).ToListAsync();

            return listEntityDtos;
        }
    }
}
