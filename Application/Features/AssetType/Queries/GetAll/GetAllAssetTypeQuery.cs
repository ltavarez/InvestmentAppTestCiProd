using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestmentApp.Core.Application.Dtos.AssetType;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Features.AssetType.Queries.GetAll
{
    /// <summary>
    /// Query parameters for retrieving all asset types
    /// </summary>
    public class GetAllAssetTypeQuery : IRequest<IList<AssetTypeDto>>
    {
    }

    public class GetAllAssetTypeQueryHandler : IRequestHandler<GetAllAssetTypeQuery, IList<AssetTypeDto>>
    {
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IMapper _mapper;

        public GetAllAssetTypeQueryHandler(IAssetTypeRepository assetTypeRepository, IMapper mapper)
        {
            _assetTypeRepository = assetTypeRepository;
            _mapper = mapper;
        }

        public async Task<IList<AssetTypeDto>> Handle(GetAllAssetTypeQuery query, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _assetTypeRepository.GetAllQueryWithInclude(["Assets"]);

            var listEntityDtos = await listEntitiesQuery.ProjectTo<AssetTypeDto>(_mapper.ConfigurationProvider).ToListAsync();

            return listEntityDtos;
        }
    }
}
