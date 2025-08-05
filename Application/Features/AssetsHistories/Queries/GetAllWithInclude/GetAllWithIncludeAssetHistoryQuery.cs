using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestmentApp.Core.Application.Dtos.AssetHistory;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Features.AssetsHistories.Queries.GetAllWithInclude
{
    public class GetAllWithIncludeAssetHistoryQuery : IRequest<IList<AssetHistoryDto>>
    {       
    }
    public class GetAllWithIncludeAssetHistoryQueryHandler : IRequestHandler<GetAllWithIncludeAssetHistoryQuery, IList<AssetHistoryDto>>
    {
        private readonly IAssetHistoryRepository _assetHistoryRepository;
        private readonly IMapper _mapper;
        public GetAllWithIncludeAssetHistoryQueryHandler(IAssetHistoryRepository assetHistoryRepository, IMapper mapper) 
        {
            _assetHistoryRepository = assetHistoryRepository;
            _mapper = mapper;
        }
        public async Task<IList<AssetHistoryDto>> Handle(GetAllWithIncludeAssetHistoryQuery query, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _assetHistoryRepository.GetAllQueryWithInclude(["Asset"]);

            var listEntityDtos = await listEntitiesQuery.ProjectTo<AssetHistoryDto>(_mapper.ConfigurationProvider).ToListAsync();

            return listEntityDtos;
        }
    }
}
